using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Egate_Payroll.Objects;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using Microsoft.Win32;
using Egate_Payroll.Model;
using Egate_Payroll.Deductions.Model;
using System.Data.Entity;
using ExcelDataReader;
using ExcelDataReader.Core;
using System.Data;
using CustomMappingObject;
using System.Windows;

namespace Egate_Payroll.Classes
{
    public static class Helpers
    {
        public static IEnumerable<EmployeeWorkTimeObject> GetWorkTimeListFromFile(string file)
        {
            List<EmployeeWorkTimeObject> items = new List<EmployeeWorkTimeObject>();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                //
                //USING EXCELDATAREADER
                //
                using (var reader = ExcelReaderFactory.CreateReader(fs))
                {
                    var conf = new ExcelDataSetConfiguration() { ConfigureDataTable = _ => new ExcelDataTableConfiguration() { UseHeaderRow = true } };
                    var ds = reader.AsDataSet(conf);
                    var tbl = ds.Tables[0];

                    //get headers
                    var columns = tbl.Columns.OfType<DataColumn>().Select(c => c.ColumnName).ToArray();
                    //prepare mapping
                    var mapping = new MappingObject<EmployeeWorkTimeObject>(columns);
                    //get rows
                    foreach (var row in tbl.Rows.OfType<DataRow>())
                    {
                        EmployeeWorkTimeObject item = new EmployeeWorkTimeObject();
                        mapping.SetValues(ref item, index => row[index]);
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public static IEnumerable<CutoffViewModel> GetCutoffList()
        {
            using (var context = new PayrollModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var query = from cu in context.cutoff
                            select new
                            {
                                cu.Id,
                                cu.StartDate,
                                cu.EndDate,
                                cu.RegularHours,
                                cu.ImportTime,
                                EmployeeCount = cu.attendance.Select(a => a.EmployeeId).Distinct().Count()
                                //TotalWorkTime = (decimal?)cu.attendance.Where(a => a.WorkTimeHours != null).Sum(a => (decimal)a.WorkTimeHours)
                            };
                return query.ToList()
                    .Select(i => new CutoffViewModel()
                    {
                        Id = (int)i.Id,
                        StartDate = i.StartDate.ToUnixDate(),
                        EndDate = i.EndDate.ToUnixDate(),
                        RegularHours = i.RegularHours,
                        ImportTime = i.ImportTime.ToUnixDate(),
                        EmployeeCount = i.EmployeeCount
                        //TotalWorkTime = i.TotalWorkTime == null ? 0 : i.TotalWorkTime.Value
                    })
                    .OrderByDescending(i => i.ImportTime);
            }
        }

        public static IEnumerable<EmployeeComputedPayrollViewModel> GetComputedList(int cutoffId, bool includeInactive = false)
        {
            var tasks = new Task[4];
            //get options
            var taskOptions = Task.Run<options>(async () =>
            {
                using (var context = new PayrollModel())
                {
                    var opt = await context.options.FirstAsync();
                    return opt;
                }
            });
            // get employee list
            var taskEmployeeList = Task.Run<IEnumerable<employee>>(async () =>
            {
                using (var context = new PayrollModel())
                {
                    var query = from emp in context.employee
                                where includeInactive ? true : emp.IsActive == 1
                                select emp;
                    var list = await query.ToListAsync();
                    return list;
                }
            });
            //get holiday list
            var taskHolidayList = Task.Run<IEnumerable<holiday>>(async () =>
            {
                using (var context = new PayrollModel())
                {
                    var list = await context.holiday.ToListAsync();
                    return list;
                }
            });
            //get attendance list
            var taskAttendanceList = Task.Run<IEnumerable<attendance>>(async () =>
            {
                using (var context = new PayrollModel())
                {
                    var query = from att in context.attendance
                                where att.CutoffId == cutoffId
                                select att;
                    var list = await query.ToListAsync();
                    return list;
                }
            });
            Task.WaitAll(taskOptions, taskEmployeeList, taskHolidayList, taskAttendanceList);

            var options = taskOptions.Result;
            var employeeList = taskEmployeeList.Result;
            var holidayList = taskHolidayList.Result;
            var attendanceList = taskAttendanceList.Result;
            //do query
            var attendanceQuery = from att in attendanceList
                                  join hol in holidayList on att.WorkDate equals hol.Date into attendanceHoliday
                                  from att_hol in attendanceHoliday.DefaultIfEmpty()
                                  select new
                                  {
                                      AttendanceId = att.Id,
                                      att.EmployeeId,
                                      att,
                                      att_hol
                                  };
            var employeeAttendanceQuery = from att in attendanceQuery
                                          join emp in employeeList on att.EmployeeId equals emp.Id
                                          select new
                                          {
                                              att.AttendanceId,
                                              emp,
                                              att.att,
                                              att.att_hol
                                          };
            return employeeAttendanceQuery.ToList()
                .Select(i => new EmployeeComputedPayrollViewModel()
                {
                    AttendanceId = (int)i.AttendanceId,
                    EmployeeId = (int)i.emp.Id,
                    EmployeeNumber = (int)i.emp.EmployeeNumber,
                    EmployeeName = i.emp.EmployeeName,
                    EmployeeType = i.emp.EmployeeType.ToEnum<EmployeeType>(),
                    Department = i.emp.Department,
                    MonthlyRate = i.emp.MonthlyRate,
                    HourlyRate = i.emp.HourlyRate,
                    TotalAllowance = ReturnNullIfDefault<decimal>((i.emp.MealAllowance ?? 0) + (i.emp.TransportationAllowance ?? 0) + (i.emp.OtherAllowance ?? 0)),
                    HasDeductions = i.emp.HasDeductions.ToBool(),
                    Notes = i.emp.Notes,
                    WorkDate = i.att.WorkDate.ToUnixDate(),
                    TimeIn = i.att.TimeIn.ToUnixDate(),
                    TimeOut = i.att.TimeOut.ToUnixDate(),
                    ModifyTimeDate = i.att.ModifyTimeDate.ToUnixDate(),
                    Late = i.att.LateHours.ToTimeSpan(),
                    Early = i.att.EarlyHours.ToTimeSpan(),
                    WorkTime = i.att.WorkTimeHours.ToTimeSpan(),
                    Overtime = i.att.OvertimeHours.ToTimeSpan(),
                    ComputedTimeIn = i.att.ComputedTimeIn.ToUnixDate(),
                    ComputedTimeOut = i.att.ComputedTimeOut.ToUnixDate(),
                    ComputedLate = i.att.ComputedLateHours.ToTimeSpan(),
                    ComputedEarly = i.att.ComputedEarlyHours.ToTimeSpan(),
                    ComputedWorkTime = i.att.ComputedWorkTimeHours.ToTimeSpan(),
                    ComputedOvertime = i.att.ComputedOvertimeHours.ToTimeSpan(),
                    HolidayType = i.att_hol == null ? null : (HolidayType?)i.att_hol.HolidayType.ToEnum<HolidayType>(),
                    HolidayName = i.att_hol == null ? null : i.att_hol.OtherName,
                    OtherHolidayRate = i.att_hol == null ? null : i.att_hol.OtherRate,
                    RegularHolidayAllowOvertime = options.RegularHolidayAllowOvertime.ToBool(),
                    SpecialHolidayAllowOvertime = options.SpecialHolidayAllowOvertime.ToBool(),
                    AdjustmentTime = i.att.AdjustedHours.ToTimeSpan(),
                    AdjustmentTimeMode = i.att.AdjustedHoursMode.ToEnum<AdjustedHoursMode>(),
                    AdjustmentTimeNotes = i.att.AdjustedHoursNotes
                })
                .OrderBy(i => i.EmployeeName);
        }

        public static ShiftSettingsViewModel GetShiftSettings(int cutoffId)
        {
            using (var context = new PayrollModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var query = from cu in context.cutoff
                            join shift in context.shift_settings on cu.Id equals shift.CutoffId
                            where cu.Id == cutoffId
                            select shift;
                var shiftSettings = query.FirstOrDefault();
                ShiftSettingsViewModel ssvm = new ShiftSettingsViewModel();
                if (shiftSettings != null)
                {
                    ssvm.ShiftSettingsId = (int)shiftSettings.Id;
                    ssvm.OnDuty = shiftSettings.OnDuty.ToUnixDate();
                    ssvm.OffDuty = shiftSettings.OffDuty.ToUnixDate();
                    ssvm.OvertimeOffsetMinutes = (int)shiftSettings.OvertimeOffset;
                    ssvm.DeductOvertimeOffset = shiftSettings.IsDeductOvertimeOffset.ToBool();
                }
                return ssvm;
            }
        }

        public static IEnumerable<EmployeeWorkTimeObject> GetWorkList(bool includeInactive = false)
        {
            using (var context = new PayrollModel())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var query = from att in context.attendance
                            join emp in context.employee on att.EmployeeId equals emp.Id
                            where includeInactive ? true : emp.IsActive == 1
                            select new
                            {
                                emp.EmployeeNumber,
                                emp.EmployeeName,
                                emp.Department,
                                att.WorkDate,
                                att.OnDuty,
                                att.OffDuty,
                                att.TimeIn,
                                att.TimeOut,
                                att.LateHours,
                                att.EarlyHours,
                                att.OvertimeHours,
                                att.WorkTimeHours,
                                att.IsAbsent,
                                att.CutoffId
                            };
                return query.ToList()
                    .Select(i => new EmployeeWorkTimeObject()
                    {
                        EmployeeNumber = (int)i.EmployeeNumber,
                        EmployeeName = i.EmployeeName,
                        Department = i.Department,
                        WorkDate = i.WorkDate.ToUnixDate(),
                        OnDuty = i.OnDuty.ToUnixDate(),
                        OffDuty = i.OffDuty.ToUnixDate(),
                        TimeIn = i.TimeIn.ToUnixDate(),
                        TimeOut = i.TimeOut.ToUnixDate(),
                        Late = i.LateHours.ToTimeSpan(),
                        Early = i.EarlyHours.ToTimeSpan(),
                        Overtime = i.OvertimeHours.ToTimeSpan(),
                        WorkTime = i.WorkTimeHours.ToTimeSpan(),
                        IsAbsent = i.IsAbsent.ToBool(),
                        CutoffId = (int)i.CutoffId
                    })
                    .OrderBy(i => i.EmployeeName);
            }
        }

        public static IEnumerable<EmployeeWorkSummaryViewModel> GetSummaryList(int cutoffId, decimal? cutoffRegularHours)
        {
            var list = Helpers.GetComputedList(cutoffId);
            //get summary values from database
            IEnumerable<attendance_summary> attSummaryList = null;
            using (var context = new PayrollModel())
            {
                attSummaryList = context.attendance_summary.Where(i => i.CutoffId == cutoffId).ToList();
            }
            //get summary list
            var summaryList = list.GroupBy(i => i.EmployeeNumber)
                .Select(g =>
                {
                    var summary = new EmployeeWorkSummaryViewModel();
                    summary.EmployeeId = g.First().EmployeeId;
                    summary.EmployeeNumber = g.Key;
                    summary.EmployeeName = g.First().EmployeeName;
                    summary.EmployeeType = g.First().EmployeeType;
                    summary.Department = g.First().Department;
                    summary.HasDeductions = g.First().HasDeductions;
                    summary.Notes = g.First().Notes;

                    summary.MonthlyRate = g.First().MonthlyRate ?? 0;
                    summary.HourlyRate = g.First().HourlyRate ?? 0;
                    summary.AllowanceRate = g.First().TotalAllowance ?? 0;

                    //from attendance summary list
                    if (attSummaryList != null && attSummaryList.Count() > 0)
                    {
                        var attSummary = attSummaryList.FirstOrDefault(i => i.EmployeeId == summary.EmployeeId);
                        if (attSummary != null)
                        {
                            summary.AdjustmentAmount = attSummary.AdjustmentAmount ?? 0;
                        }
                    }

                    //work subtotal and totals
                    summary.SubtotalWorkTime = TimeSpan.FromHours(Math.Round(g.Sum(i => i.ActualWorkTime == null ? 0 : i.ActualWorkTime.Value.TotalHours), 2));
                    summary.SubtotalOvertime = TimeSpan.FromHours(Math.Round(g.Sum(i => i.ActualWorkOvertime == null ? 0 : i.ActualWorkOvertime.Value.TotalHours), 2));
                    summary.TotalWorkTime = TimeSpan.FromHours(Math.Round(g.Sum(i => i.TotalWorkTime == null ? 0 : i.TotalWorkTime.Value.TotalHours), 2));

                    //holiday subtotal and totals
                    summary.SubtotalHolidayTime = TimeSpan.FromHours(Math.Round(g.Sum(i => i.HolidayRegularTime == null ? 0 : i.HolidayRegularTime.Value.TotalHours), 2));
                    summary.SubtotalHolidayOvertime = TimeSpan.FromHours(Math.Round(g.Sum(i => i.HolidayOvertime == null ? 0 : i.HolidayOvertime.Value.TotalHours), 2));
                    summary.TotalHolidayTime = TimeSpan.FromHours(Math.Round(g.Sum(i => i.HolidayTotalTime == null ? 0 : i.HolidayTotalTime.Value.TotalHours), 2));

                    //adjustment
                    var totalAdjustedHours = g.Sum(i => i.GetAdjustmentHours());
                    summary.TotalAdjustmentTime = TimeSpan.FromHours(Math.Abs(totalAdjustedHours));
                    if (totalAdjustedHours > 0) summary.FinalAdjustmentHoursMode = AdjustedHoursMode.Add;
                    else if (totalAdjustedHours < 0) summary.FinalAdjustmentHoursMode = AdjustedHoursMode.Deduct;

                    //regular hours, overtime, final time
                    if (cutoffRegularHours != null)
                    {
                        double regularHoursOverall = summary.SubtotalWorkTime.TotalHours + totalAdjustedHours;
                        double hoursDiff = (double)cutoffRegularHours.Value - regularHoursOverall;
                        if (hoursDiff > 0)
                        {
                            //deduct overtime until regular hours is completed
                            summary.FinalRegularHours = TimeSpan.FromHours(Math.Min((double)cutoffRegularHours.Value, regularHoursOverall + summary.SubtotalOvertime.TotalHours));
                            summary.FinalOvertime = TimeSpan.FromHours(Math.Max(0, summary.SubtotalOvertime.TotalHours - hoursDiff));
                        }
                        else
                        {
                            summary.FinalRegularHours = TimeSpan.FromHours(regularHoursOverall);
                            summary.FinalOvertime = summary.SubtotalOvertime;
                        }
                    }
                    return summary;
                })
                .OrderBy(i => i.EmployeeName);
            return summaryList;
        }

        public static async Task SetPayrollDeductionsAsync(IEnumerable<EmployeeWorkSummaryViewModel> summaryList)
        {
            var tasks = new Task[4];
            tasks[0] = GetSssDeductionsAsync(summaryList);
            tasks[1] = GetPhilHealthDeductionsAsync(summaryList);
            tasks[2] = GetPagibigDeductionsAsync(summaryList);
            tasks[3] = GetWithholdingTaxDeductionsAsync(summaryList);
            await Task.WhenAll(tasks);
        }

        private static async Task GetSssDeductionsAsync(IEnumerable<EmployeeWorkSummaryViewModel> summaryList)
        {
            using (var deductions = new PayrollDeductionsModel())
            {
                deductions.Configuration.AutoDetectChangesEnabled = false;
                var query = from i in deductions.sss
                            select new
                            {
                                CompensationFrom = i.CompensationFrom == null ? 0 : i.CompensationFrom.Value,
                                CompensationTo = i.CompensationTo == null ? decimal.MaxValue : i.CompensationTo.Value,
                                i.EmployeeContribution,
                                i.EmployerContribution
                            };
                var sssTable = await query.ToListAsync();
                foreach (var summary in summaryList)
                {
                    if (!summary.HasDeductions) continue;
                    var sss = sssTable.First(s => summary.GrossIncome >= s.CompensationFrom && summary.GrossIncome <= s.CompensationTo);
                    summary.Deductions.SssEmployeeContribution = Math.Round(sss.EmployeeContribution, 2);
                    summary.Deductions.SssEmployerContribution = Math.Round(sss.EmployerContribution, 2);
                }
            }
        }

        private static async Task GetPhilHealthDeductionsAsync(IEnumerable<EmployeeWorkSummaryViewModel> summaryList)
        {
            using (var deductions = new PayrollDeductionsModel())
            {
                deductions.Configuration.AutoDetectChangesEnabled = false;
                var query = from i in deductions.philhealth
                            select new
                            {
                                i.MonthlyBasicSalaryFrom,
                                i.MonthlyBasicSalaryTo,
                                i.PremiumRate
                            };
                var philhealth = await query.FirstOrDefaultAsync();
                foreach (var summary in summaryList)
                {
                    if (!summary.HasDeductions) continue;
                    decimal philhealthContribution = 0;
                    if (summary.GrossIncome <= philhealth.MonthlyBasicSalaryFrom) //minimum
                        philhealthContribution = philhealth.MonthlyBasicSalaryFrom * philhealth.PremiumRate;
                    else if (summary.GrossIncome >= philhealth.MonthlyBasicSalaryTo) //maximum
                        philhealthContribution = philhealth.MonthlyBasicSalaryTo * philhealth.PremiumRate;
                    else //between
                        philhealthContribution = Math.Round(summary.GrossIncome * philhealth.PremiumRate, 2);
                    summary.Deductions.PhilhealthEmployeeContribution = Math.Round(philhealthContribution / 2, 2);
                    summary.Deductions.PhilhealthEmployerContribution = Math.Round(philhealthContribution / 2, 2);
                }
            }
        }

        private static async Task GetPagibigDeductionsAsync(IEnumerable<EmployeeWorkSummaryViewModel> summaryList)
        {
            using (var deductions = new PayrollDeductionsModel())
            {
                deductions.Configuration.AutoDetectChangesEnabled = false;
                var query = from i in deductions.pagibig
                            select new
                            {
                                MonthlyCompensationFrom = i.MonthlyCompensationFrom == null ? 0 : i.MonthlyCompensationFrom.Value,
                                MonthlyCompensationTo = i.MonthlyCompensationTo == null ? decimal.MaxValue : i.MonthlyCompensationTo.Value,
                                i.EmployeeShareRate,
                                i.EmployerShareRate
                            };
                var pagibigTable = await query.ToListAsync();
                foreach (var summary in summaryList)
                {
                    if (!summary.HasDeductions) continue;
                    var pagibig = pagibigTable.First(i => summary.GrossIncome >= i.MonthlyCompensationFrom && summary.GrossIncome <= i.MonthlyCompensationTo);
                    summary.Deductions.PagibigEmployeeContribution = Math.Round((summary.GrossIncome * pagibig.EmployeeShareRate) / 2, 2);
                    summary.Deductions.PagibigEmployerContribution = Math.Round((summary.GrossIncome * pagibig.EmployerShareRate) / 2, 2);
                }
            }
        }

        private static async Task GetWithholdingTaxDeductionsAsync(IEnumerable<EmployeeWorkSummaryViewModel> summaryList)
        {
            using (var deductions = new PayrollDeductionsModel())
            {
                deductions.Configuration.AutoDetectChangesEnabled = false;
                long now = DateTime.Now.ToUnixLong();
                var query = from i in deductions.tax
                            select new
                            {
                                CompensationFrom = i.CompensationFrom == null ? 0 : i.CompensationFrom.Value,
                                CompensationTo = i.CompensationTo == null ? decimal.MaxValue : i.CompensationTo.Value,
                                i.WithholdingTaxFixed,
                                i.WithholdingTaxAdditionalRate
                            };
                var taxTable = await query.ToListAsync();
                foreach (var summary in summaryList)
                {
                    if (!summary.HasDeductions) continue;
                    var tax = taxTable.First(i => summary.GrossIncome >= i.CompensationFrom && summary.GrossIncome <= i.CompensationTo);
                    var contribution = tax.WithholdingTaxFixed;
                    if (summary.GrossIncome - tax.CompensationFrom > 0)
                    {
                        contribution += Math.Round((summary.GrossIncome - tax.CompensationFrom) * (decimal)tax.WithholdingTaxAdditionalRate);
                    }
                    summary.Deductions.TaxEmployeeContribution = Math.Round(contribution, 2);
                }
            }
        }

        public static string GenerateXmlFromEmployeeSummaryList(IEnumerable<EmployeeWorkSummaryViewModel> summaryList, CutoffViewModel period)
        {
            PayslipCollectionForXml payslips = new PayslipCollectionForXml();
            payslips.PayslipItems = summaryList.Select(i =>
            {
                var payslipItem = new PayslipCollectionForXml.PayslipItem()
                {
                    EmployeeName = i.EmployeeName,
                    EmployeeNotes = i.Notes,
                    PayPeriodStart = period.StartDate,
                    PayPeriodEnd = period.EndDate,
                    PayDate = DateTime.Now,
                    GrossPay = i.GrossIncome,
                    TotalDeductions = i.Deductions.TotalEmployeeDeductions,
                    NetPay = i.NetPay
                };
                Converters.TotalHoursConverter totalHoursConverter = new Converters.TotalHoursConverter();
                //add earnings
                payslipItem.Earnings.Add(new PayslipCollectionForXml.Earning() { Name = "Regular", Hours = i.FinalRegularHours ?? TimeSpan.Zero, Rate = i.HourlyRate, Current = i.FinalRegularHoursIncome });
                payslipItem.Earnings.Add(new PayslipCollectionForXml.Earning() { Name = "Overtime", Hours = i.FinalOvertime ?? TimeSpan.Zero, Rate = i.HourlyRate, Current = i.FinalOvertimeIncome });
                if (i.TotalHolidayTime.TotalHours > 0)
                {
                    payslipItem.Earnings.Add(new PayslipCollectionForXml.Earning() { Name = "Holiday", Hours = i.TotalHolidayTime, Rate = i.HourlyRate, Current = i.TotalHolidayIncome });
                }
                if (i.AllowanceRate > 0)
                {
                    payslipItem.Earnings.Add(new PayslipCollectionForXml.Earning() { Name = "Allowance", Hours = i.FinalRegularHours ?? TimeSpan.Zero, Rate = i.AllowanceRate, Current = i.GrandTotalAllowance });
                }
                if (i.AdjustmentAmount > 0)
                {
                    payslipItem.Earnings.Add(new PayslipCollectionForXml.Earning() { Name = "Adjustment", Hours = TimeSpan.Zero, Rate = 0, Current = i.AdjustmentAmount });
                }
                //add deductions
                payslipItem.Deductions.Add(new PayslipCollectionForXml.Deduction() { Name = "SSS", Current = i.Deductions.SssEmployeeContribution });
                payslipItem.Deductions.Add(new PayslipCollectionForXml.Deduction() { Name = "PhilHealth", Current = i.Deductions.PhilhealthEmployeeContribution });
                payslipItem.Deductions.Add(new PayslipCollectionForXml.Deduction() { Name = "Pag-Ibig", Current = i.Deductions.PagibigEmployeeContribution });
                payslipItem.Deductions.Add(new PayslipCollectionForXml.Deduction() { Name = "Tax", Current = i.Deductions.TaxEmployeeContribution });
                return payslipItem;
            }).ToList();
            string xml = payslips.ToXml();
            return xml;
        }

        private static Nullable<T> ReturnNullIfDefault<T>(this Nullable<T> value) where T : struct
        {
            if (value != null)
            {
                if (value.Value.Equals(default(T))) return null;
            }
            return value;
        }
    }
}
