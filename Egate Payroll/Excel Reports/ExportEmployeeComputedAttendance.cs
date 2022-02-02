using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using Microsoft.Win32;
using System.IO;
using Egate_Payroll.Objects;

namespace Egate_Payroll.Excel_Reports
{
    public static class ExportEmployeeComputedAttendance
    {
        public static void ExportList(IEnumerable<EmployeeComputedPayrollViewModel> list)
        {
            byte[] excelData = null;
            using (MemoryStream ms = new MemoryStream())
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("attendance");

                //create header
                IRow headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("ID");
                headerRow.CreateCell(1).SetCellValue("Employee Name");
                headerRow.CreateCell(2).SetCellValue("Type");
                headerRow.CreateCell(3).SetCellValue("Date");
                headerRow.CreateCell(4).SetCellValue("Absent");
                headerRow.CreateCell(5).SetCellValue("Time-In");
                headerRow.CreateCell(6).SetCellValue("Time-Out");
                headerRow.CreateCell(7).SetCellValue("Holiday");
                headerRow.CreateCell(8).SetCellValue("Work Hours");
                headerRow.CreateCell(9).SetCellValue("Overtime");
                headerRow.CreateCell(10).SetCellValue("Holiday Hours");
                headerRow.CreateCell(11).SetCellValue("Holiday Overtime");
                headerRow.CreateCell(12).SetCellValue("Adjustment");
                headerRow.CreateCell(13).SetCellValue("Work Total");
                headerRow.CreateCell(14).SetCellValue("Holiday Total");

                int i = 1;
                foreach (var computed in list)
                {
                    IRow row = sheet.CreateRow(i);
                    row.CreateCell(0).SetCellValue(computed.EmployeeNumber);
                    row.CreateCell(1).SetCellValue(computed.EmployeeName);
                    row.CreateCell(2).SetCellValue(computed.EmployeeType.ToString());
                    row.CreateCell(3).SetCellValue(computed.WorkDate.ToString("yyyy-MM-dd"));
                    row.CreateCell(4).SetCellValue(computed.IsAbsent);
                    row.CreateCell(5).SetCellValue(computed.TimeIn == null ? "" : computed.TimeIn.Value.ToString("hhh:mm"));
                    row.CreateCell(6).SetCellValue(computed.TimeOut == null ? "" : computed.TimeOut.Value.ToString("hhh:mm"));
                    row.CreateCell(7).SetCellValue(computed.HolidayFullName);
                    row.CreateCell(8).SetCellValue(computed.ActualWorkTime);
                    row.CreateCell(9).SetCellValue(computed.ActualWorkOvertime);
                    row.CreateCell(10).SetCellValue(computed.HolidayRegularTime);
                    row.CreateCell(11).SetCellValue(computed.HolidayOvertime);
                    row.CreateCell(12).SetCellValue(computed.AdjustmentTime == null ? "" : new Converters.AdjustedHoursModeTextConverter().Convert(computed.AdjustmentTimeMode, null, null, null).ToString() + new Converters.TimeSpanTotalHoursDisplayConverter().Convert(computed.AdjustmentTime, null, null, null).ToString());
                    row.CreateCell(13).SetCellValue(computed.TotalWorkTime);
                    row.CreateCell(14).SetCellValue(computed.HolidayTotalTime);

                    i++;
                }

                workbook.Write(ms);
                excelData = ms.ToArray();
            }

            SaveFileDialog save = new SaveFileDialog();
            save.Title = "Export Time Adjustment";
            save.Filter = "Excel File|*.xlsx";
            save.FileName = "payroll_time adjustment_" + DateTime.Now.ToString("MM-dd-yyyy");
            if (save.ShowDialog() == true)
            {
                using (var fs = new FileStream(save.FileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(excelData, 0, excelData.Length);
                }
                Classes.FileHelper.Select(save.FileName);
            }
        }
    }
}
