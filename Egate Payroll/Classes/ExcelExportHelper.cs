using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using Microsoft.Win32;
using System.IO;
using Egate_Payroll.Objects;
using System.Collections;
using System.Globalization;

namespace Egate_Payroll.Classes
{
    public class ExcelExportHelper
    {
        public static void ExportPayrollSummary(IEnumerable<EmployeeWorkSummaryViewModel> list, DateTime periodFrom, DateTime periodTo)
        {
            //prepare fields
            FieldExpressionCollection<EmployeeWorkSummaryViewModel> expressionList = new FieldExpressionCollection<EmployeeWorkSummaryViewModel>();
            expressionList.Add(i => i.EmployeeName, "Employee Name");
            expressionList.Add(i => i.FinalRegularHoursIncome, "Regular Hour Income");
            expressionList.Add(i => i.FinalOvertimeIncome, "Overtime Income");
            expressionList.Add(i => i.GrandTotalAllowance, "Allowance Invome");
            expressionList.Add(i => i.TotalHolidayIncome, "Holiday Income");
            expressionList.Add(i => i.AdjustmentAmount, "Adjustment");
            expressionList.Add(i => i.GrossIncome, "Total Gross Income");
            expressionList.Add(i => i.Deductions.SssEmployeeContribution, "SSS");
            expressionList.Add(i => i.Deductions.PhilhealthEmployeeContribution, "PhilHealth");
            expressionList.Add(i => i.NetPay, "Total Net Pay");

            //prepare excel
            byte[] excelData = null;
            using (MemoryStream ms = new MemoryStream())
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("payroll");

                //create header
                IRow headerRow = sheet.CreateRow(0);
                for (int i = 0; i < expressionList.Count; i++)
                    headerRow.CreateCell(i).SetCellValue(expressionList[i].GetFieldName());
                //create rows
                int r = 1;
                foreach (var i in list)
                {
                    IRow row = sheet.CreateRow(r);
                    for (int a = 0; a < expressionList.Count; a++)
                        row.CreateCell(a).SetCellValue(expressionList[a].GetStringValue(i));
                    r++;
                }

                workbook.Write(ms);
                excelData = ms.ToArray();
            }
            SaveExcelData(excelData, "Save Payroll Summary", string.Format("payroll_{0:MM-dd-yyyy} to {1:MM-dd-yyyy}", periodFrom, periodTo));
        }

        private static void SaveExcelData(byte[] excelData, string title, string fileName)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Title = title;
            save.Filter = "Excel File|*.xlsx";
            save.FileName = fileName;
            if (save.ShowDialog() == true)
            {
                try
                {
                    using (var fs = new FileStream(save.FileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(excelData, 0, excelData.Length);
                    }
                    FileHelper.Open(save.FileName);
                }
                catch (IOException ioEx)
                {
                    Logs.Exception(ioEx);
                    System.Windows.MessageBox.Show(ioEx.Message);
                }
            }
        }

        private class FieldExpressionCollection<T> : List<FieldExpression<T>>
        {
            public void Add(Expression<Func<T, object>> expression)
            {
                this.Add(expression, null, null, null);
            }

            public void Add(Expression<Func<T, object>> expression, string fieldName)
            {
                this.Add(expression, null, fieldName, null);
            }

            public void Add(Expression<Func<T, object>> expression, Func<T, bool> predicate, string fieldName)
            {
                this.Add(expression, predicate, fieldName, null);
            }

            public void Add(Expression<Func<T, object>> expression, string fieldName, string formatString)
            {
                this.Add(expression, null, fieldName, formatString);
            }

            public void Add(Expression<Func<T, object>> expression, Func<T, bool> predicate, string fieldName, string formatString)
            {
                this.Add(new FieldExpression<T>(expression, predicate, fieldName, formatString));
            }
        }

        private class FieldExpression<T>
        {
            private Expression<Func<T, object>> _expression;
            private string _fieldName;
            private string _formatString;
            private Func<T, bool> _predicate;

            public FieldExpression(Expression<Func<T, object>> expression, Func<T, bool> predicate, string fieldName, string formatString)
            {
                this._expression = expression;
                this._predicate = predicate;
                this._fieldName = fieldName;
                this._formatString = formatString;
            }

            public string GetFieldName()
            {
                if (!string.IsNullOrEmpty(_fieldName))
                    return _fieldName;
                else
                {
                    if (_expression.Body is MemberExpression)
                    {
                        return ((MemberExpression)_expression.Body).Member.Name;
                    }
                    else
                    {
                        var op = ((UnaryExpression)_expression.Body).Operand;
                        return ((MemberExpression)op).Member.Name;
                    }
                }
            }

            public string GetStringValue(T parent)
            {
                try
                {
                    if (_predicate != null && !_predicate(parent))
                        return null;
                }
                catch (NullReferenceException)
                { }

                object value = null;
                try
                {
                    var method = _expression.Compile();
                    value = method(parent);
                }
                catch (NullReferenceException)
                { }

                if (value != null)
                {
                    IFormattable formattable = value as IFormattable;
                    if (formattable != null)
                    {
                        string format = !string.IsNullOrEmpty(this._formatString) ? this._formatString : GetFormatStringForType(value.GetType());
                        value = formattable.ToString(format, CultureInfo.CurrentCulture);
                    }
                }
                return Convert.ToString(value);
            }

            private static string GetFormatStringForType(Type type)
            {
                if (type != null)
                {
                    if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
                        return "#.#0";
                    else if (type == typeof(DateTime))
                        return "M/d/yyyy hh:mm:ss tt";
                    else if (type == typeof(TimeSpan))
                        return "c";
                }
                return "";
            }
        }
    }
}
