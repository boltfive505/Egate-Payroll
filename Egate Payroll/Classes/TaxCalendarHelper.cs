using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.IO;
using Egate_Payroll.Objects.TaxCalendar;
using Egate_Payroll.Tax_Calendar.Model;

namespace Egate_Payroll.Classes
{
    public static class TaxCalendarHelper
    {
        public const string BIR_FILE = @"uploads\bir files";

        public static async Task<IEnumerable<TaxFilingCategoryViewModel>> GetCategoryListAsync()
        {
            using (var context = new TaxCalendarModel())
            {
                var list = await context.filing_category.ToListAsync();
                return list.Select(i => new TaxFilingCategoryViewModel(i));
            }
        }

        public static async Task<IEnumerable<TaxFilingCompanyViewModel>> GetCompanyListAsync()
        {
            using (var context = new TaxCalendarModel())
            {
                var list = await context.filing_company.ToListAsync();
                return list.Select(i => new TaxFilingCompanyViewModel(i));
            }
        }

        public static async Task<IEnumerable<TaxFilingPeriodViewModel>> GetPeriodListAsync()
        {
            using (var context = new TaxCalendarModel())
            {
                var query = from p in context.filing_form
                            join cat in context.filing_category on p.FilingCategoryId equals cat.Id into periodCategory
                            from p_cat in periodCategory.DefaultIfEmpty()
                            join incl in context.period_inclusion on p.Id equals incl.PeriodId into periodInclusion
                            from p_incl in periodInclusion.DefaultIfEmpty()
                            select new
                            {
                                Period = p,
                                Category = p_cat,
                                Inclusion = p_incl
                            };
                var list = await query.ToListAsync();
                return list.GroupBy(i => i.Period)
                    .Select(g =>
                    {
                        var vm = new TaxFilingPeriodViewModel();
                        filing_form period = g.Key;
                        filing_category cat = g.First().Category;
                        IEnumerable<period_inclusion> inclusionList = g.Where(i => i.Inclusion != null).Select(i => i.Inclusion);
                        //period
                        vm.Id = (int)period.Id;
                        vm.FormName = period.FormName;
                        vm.FormTitle = period.FormTitle;
                        vm.Description = period.Description;
                        vm.PeriodType = period.PeriodType.ToEnum<FilingPeriodType>();
                        vm.DueDateStart = period.DueDateStart.ToUnixDate();
                        vm.DueDateEnd = period.DueDateEnd.ToUnixDate();
                        vm.DueMonth = (int?)period.DueMonth;
                        vm.DueDays = (int?)period.DueDays;
                        vm.IsActive = period.IsActive.ToBool();
                        vm.Category = cat == null ? null : new TaxFilingCategoryViewModel(cat);
                        //inclusion
                        if (inclusionList != null && inclusionList.Count() > 0)
                        {
                            //always ensure that inclusion list is complete (months: 12, quarters: 4)
                            //if missing, default to included
                            //month
                            var monthInclusionList = inclusionList.Where(i => i.InclusionType.ToEnum<FilingPeriodType>() == FilingPeriodType.Monthly);
                            if (monthInclusionList != null && monthInclusionList.Count() > 0)
                            {
                                foreach (var monthInclVm in vm.MonthInclusionList)
                                {
                                    period_inclusion incl = monthInclusionList.FirstOrDefault(i => i.Value == monthInclVm.Value);
                                    if (incl != null)
                                    {
                                        monthInclVm.Id = (int)incl.Id;
                                        monthInclVm.IsIncluded = incl.IsIncluded.ToBool();
                                    }
                                }
                            }
                            //quarter
                            var quarterInclusionList = inclusionList.Where(i => i.InclusionType.ToEnum<FilingPeriodType>() == FilingPeriodType.EndOfQuarter);
                            if (quarterInclusionList != null && quarterInclusionList.Count() > 0)
                            {
                                foreach (var quarterInclVm in vm.QuarterInclusionList)
                                {
                                    period_inclusion incl = quarterInclusionList.FirstOrDefault(i => i.Value == quarterInclVm.Value);
                                    if (incl != null)
                                    {
                                        quarterInclVm.Id = (int)incl.Id;
                                        quarterInclVm.IsIncluded = incl.IsIncluded.ToBool();
                                    }
                                }
                            }
                        }
                        return vm;
                    });
            }
        }

        public static async Task<IEnumerable<TaxFilingPeriodViewModel>> GetPeriodListSimpleAsync()
        {
            using (var context = new TaxCalendarModel())
            {
                var query = from p in context.filing_form
                            join cat in context.filing_category on p.FilingCategoryId equals cat.Id into periodCategory
                            from p_cat in periodCategory.DefaultIfEmpty()
                            select new
                            {
                                Period = p,
                                Category = p_cat,
                            };
                var list = await query.ToListAsync();
                return list.Select(i => {
                    var vm = new TaxFilingPeriodViewModel();
                    vm.Id = (int)i.Period.Id;
                    vm.FormName = i.Period.FormName;
                    vm.FormTitle = i.Period.FormTitle;
                    vm.Description = i.Period.Description;
                    vm.PeriodType = i.Period.PeriodType.ToEnum<FilingPeriodType>();
                    vm.DueDateStart = i.Period.DueDateStart.ToUnixDate();
                    vm.DueDateEnd = i.Period.DueDateEnd.ToUnixDate();
                    vm.DueMonth = (int?)i.Period.DueMonth;
                    vm.DueDays = (int?)i.Period.DueDays;
                    vm.IsActive = i.Period.IsActive.ToBool();
                    vm.Category = i.Category == null ? null : new TaxFilingCategoryViewModel(i.Category);
                    return vm;
                });
            }
        }

        public static async Task<IEnumerable<TaxFilingPaymentViewModel>> GetPaymentListAsync()
        {
            using (var context = new TaxCalendarModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var query = from history in context.filing_history
                            join comp in context.filing_company on history.CompanyId equals comp.Id into tempComp
                            from comp_history in tempComp.DefaultIfEmpty()
                            join form in context.filing_form on history.PeriodId equals form.Id
                            join cat in context.filing_category on form.FilingCategoryId equals cat.Id into form_cat
                            from formCategory in form_cat.DefaultIfEmpty()
                            orderby history.FilingDate descending
                            select new
                            {
                                history.Id,
                                PeriodId = form.Id,
                                form.FormName,
                                Company = comp_history,
                                CategoryName = formCategory == null ? null : formCategory.CategoryName,
                                history.PeriodDate,
                                history.FilingDate,
                                history.Amount,
                                history.FormFileAttachment,
                                history.PaymentFileAttachment,
                                history.ProvisionFileAttachment,
                                history.Notes,
                                history.UpdatedDate
                            };
                var list = await query.ToListAsync();
                var periodList = await TaxCalendarHelper.GetPeriodListSimpleAsync();
                return list.Select(i => new TaxFilingPaymentViewModel()
                    {
                        Id = (int)i.Id,
                        Period = periodList.FirstOrDefault(p => p.Id == i.PeriodId),
                        FormName = i.FormName,
                        CategoryName = i.CategoryName,
                        PeriodDate = i.PeriodDate.ToUnixDate(),
                        FilingDate = i.FilingDate.ToUnixDate(),
                        Amount = i.Amount,
                        Company = i.Company == null ? null : new TaxFilingCompanyViewModel(i.Company),
                        FormFileName = i.FormFileAttachment,
                        PaymentFileName = i.PaymentFileAttachment,
                        ProvisionFileName = i.ProvisionFileAttachment,
                        Notes = i.Notes,
                        UpdatedDate = i.UpdatedDate.ToUnixDate()
                    });
            }
        }

        public static async Task AddCompanyAsync(TaxFilingCompanyViewModel companyVm)
        {
            using (var context = new TaxCalendarModel())
            {
                var company = await context.filing_company.FirstOrDefaultAsync(i => i.Id == companyVm.Id);
                if (company == null)
                {
                    company = new filing_company();
                    context.filing_company.Add(company);
                }
                company.CompanyName = companyVm.CompanyName;
                company.Description = companyVm.Description;
                company.IsActive = companyVm.IsActive.ToLong();

                await context.SaveChangesAsync();
                companyVm.Id = company.Id;
            }
        }

        public static async Task AddUpdatePeriodAsync(TaxFilingPeriodViewModel period)
        {
            using (var context = new TaxCalendarModel())
            {
                var filingPeriod = await context.filing_form.FirstOrDefaultAsync(i => i.Id == period.Id);
                if (filingPeriod == null)
                {
                    filingPeriod = new filing_form();
                    context.filing_form.Add(filingPeriod);
                }

                //period
                filingPeriod.FormName = period.FormName;
                filingPeriod.FormTitle = period.FormTitle;
                filingPeriod.Description = period.Description;
                filingPeriod.PeriodType = period.PeriodType.ToString();
                filingPeriod.DueDateStart = period.DueDateStart.ToUnixLong();
                filingPeriod.DueDateEnd = period.DueDateEnd.ToUnixLong();
                filingPeriod.DueMonth = period.DueMonth;
                filingPeriod.DueDays = period.DueDays;
                filingPeriod.IsActive = period.IsActive.ToLong();
                filingPeriod.FilingCategoryId = period.Category?.Id;
                //inclusion
                var inclusionList = await (from incl in context.period_inclusion
                                           where incl.PeriodId == period.Id
                                           select incl)
                                           .ToListAsync();
                //inclusion month
                foreach (var inclVm in period.MonthInclusionList)
                {
                    period_inclusion incl = inclusionList.FirstOrDefault(i => i.Id == inclVm.Id);
                    if (incl == null)
                    {
                        incl = new period_inclusion();
                        incl.filing_form = filingPeriod;
                        context.period_inclusion.Add(incl);
                    }
                    incl.InclusionType = FilingPeriodType.Monthly.ToString();
                    incl.Value = inclVm.Value;
                    incl.IsIncluded = inclVm.IsIncluded.ToLong();
                }
                //quarter month
                foreach (var inclVm in period.QuarterInclusionList)
                {
                    period_inclusion incl = inclusionList.FirstOrDefault(i => i.Id == inclVm.Id);
                    if (incl == null)
                    {
                        incl = new period_inclusion();
                        incl.filing_form = filingPeriod;
                        context.period_inclusion.Add(incl);
                    }
                    incl.InclusionType = FilingPeriodType.EndOfQuarter.ToString();
                    incl.Value = inclVm.Value;
                    incl.IsIncluded = inclVm.IsIncluded.ToLong();
                }

                await context.SaveChangesAsync();

                if (period.Id != (int)filingPeriod.Id)
                    period.Id = (int)filingPeriod.Id;
            }
        }

        public static async Task AddFormPaymentAsync(TaxFilingPaymentViewModel payment)
        {
            using (var context = new TaxCalendarModel())
            {
                var filingHistory = await context.filing_history.FirstOrDefaultAsync(i => i.Id == payment.Id);
                if (filingHistory == null)
                {
                    filingHistory = new filing_history();
                    context.filing_history.Add(filingHistory);
                }
                filingHistory.PeriodId = payment.Period.Id;
                filingHistory.FormName = payment.FormName;
                filingHistory.PeriodDate = payment.PeriodDate.ToUnixLong();
                filingHistory.FilingDate = payment.FilingDate.ToUnixLong();
                filingHistory.Amount = payment.Amount ?? 0;
                filingHistory.CompanyId = payment.Company?.Id;
                filingHistory.Notes = payment.Notes;
                filingHistory.UpdatedDate = payment.UpdatedDate.ToUnixLong();
                //attachment files
                filingHistory.FormFileAttachment = ProcessTaxPaymentFile(filingHistory.FormFileAttachment, payment.FormFileName, payment.Company.CompanyName, GetTaxPaymentFileRename(payment, "Form"));
                filingHistory.PaymentFileAttachment = ProcessTaxPaymentFile(filingHistory.PaymentFileAttachment, payment.PaymentFileName, payment.Company.CompanyName, GetTaxPaymentFileRename(payment, "Payment"));
                filingHistory.ProvisionFileAttachment = ProcessTaxPaymentFile(filingHistory.ProvisionFileAttachment, payment.ProvisionFileName, payment.Company.CompanyName, GetTaxPaymentFileRename(payment, "Provision"));
                await context.SaveChangesAsync();

                payment.Id = (int)filingHistory.Id;
                payment.FormFileName = filingHistory.FormFileAttachment;
                payment.PaymentFileName = filingHistory.PaymentFileAttachment;
                payment.ProvisionFileName = filingHistory.ProvisionFileAttachment;
            }
        }

        public static async Task DeleteFormPaymentAsync(TaxFilingPaymentViewModel payment)
        {
            using (var context = new TaxCalendarModel())
            {
                var filingHistory = await context.filing_history.FirstOrDefaultAsync(i => i.Id == payment.Id);
                if (filingHistory != null)
                {
                    context.filing_history.Remove(filingHistory);
                    //also delete files
                    DeleteTaxPaymentFiles(payment.FormFileName);
                    DeleteTaxPaymentFiles(payment.PaymentFileName);
                    DeleteTaxPaymentFiles(payment.ProvisionFileName);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static string ProcessTaxPaymentFile(string oldFileName, string newFileName, string subdirectory, string rename)
        {
            if (oldFileName == newFileName) return oldFileName; //no file change
            //file has changed
            //upload new file
            string fileName = "";
            if (!string.IsNullOrEmpty(newFileName))
            {
                string newFile = FileHelper.Upload(newFileName, Path.Combine(TaxCalendarHelper.BIR_FILE, subdirectory), rename, FileHelper.FileNameRenameMode.FullRename);
                fileName = Path.Combine(Directory.GetParent(newFile).Name, Path.GetFileName(newFile));
            }
            //delete old file
            DeleteTaxPaymentFiles(oldFileName);
            return fileName;
        }

        private static void DeleteTaxPaymentFiles(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string file = FileHelper.GetFile(fileName, TaxCalendarHelper.BIR_FILE);
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        private static string GetTaxPaymentFileRename(TaxFilingPaymentViewModel payment, string name)
        {
            return string.Format("{0}_{1}_{2:yyyy-mm-dd}_{3}_{4}", payment.Company.CompanyName, payment.CategoryName, payment.FilingDate, payment.FormName, name);
        }
    }
}
