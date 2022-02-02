using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Egate_Payroll.Objects;
using CustomMappingObject;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using Microsoft.Win32;

namespace Egate_Payroll.Excel_Reports
{
    public class CreatePayrollFileFromList
    {
        public double RegularHours { get; set; }
        public DateTime CutoffStart { get; set; }
        public DateTime CutoffEnd { get; set; }
        public IEnumerable<EmployeeWorkTimeObject> EmployeeWorkList { get; set; }

        public const string DEFAULT_FONT_NAME = "Segoe UI";
        public const string HEADER_FONT_NAME = "Century Gothic";
        public const float DEFAULT_FONT_SIZE_IN_POINTS = 10;

        public void Create()
        {
            byte[] excelData = null;
            using (MemoryStream ms = new MemoryStream())
            {
                IWorkbook workbook = new XSSFWorkbook();

                CreateDetailsSheet(workbook);

                workbook.Write(ms);
                excelData = ms.ToArray();
            }
        }

        private void CreateDetailsSheet(IWorkbook wb)
        {
            ISheet sheet = wb.CreateSheet("details");
            sheet.DisplayGridlines = false;
            sheet.CreateFreezePane(0, 1);


        }

        private static ICellStyle GetHeaderCellStyle(IWorkbook wb)
        {
            ICellStyle headerStyle = wb.CreateCellStyle();
            headerStyle.Alignment = HorizontalAlignment.Center;
            headerStyle.VerticalAlignment = VerticalAlignment.Center;
            headerStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            headerStyle.FillPattern = FillPattern.SolidForeground;
            headerStyle.SetBorder(BorderStyle.Thin, HSSFColor.Grey80Percent.Index);
            headerStyle.WrapText = true;
            return headerStyle;
        }
    }
}
