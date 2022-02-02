using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;

namespace Egate_Payroll
{
    public static class NPOIExt
    {
        public static object GetCellValue(this ICell cell)
        {
            if (cell == null) return null;
            switch (cell.CellType)
            {
                case CellType.String: return cell.StringCellValue;
                case CellType.Numeric:
                    if (HSSFDateUtil.IsCellDateFormatted(cell)) return cell.DateCellValue;
                    else return cell.NumericCellValue;
                case CellType.Boolean: return cell.BooleanCellValue;
                default: return null;
            }
        }

        public static void SetRawCellValue<T>(this ICell cell, T value)
        {
            if (value == null)
            {
                cell.SetCellType(CellType.Blank);
                return;
            }
            cell.SetCellValue(Convert.ToString(value));
        }

        public static void SetCellValue(this ICell cell, TimeSpan value)
        {
            
            cell.SetCellValue(value.ToString("hh\\:mm\\:ss"));
        }

        public static void SetCellValue(this ICell cell, bool? value)
        {
            if (value != null) cell.SetCellValue((bool)value);
        }

        public static void SetCellValue(this ICell cell, double? value)
        {
            if (value != null) cell.SetCellValue((double)value);
        }

        public static void SetCellValue(this ICell cell, DateTime? value)
        {
            if (value != null) cell.SetCellValue((DateTime)value);
        }

        public static void SetCellValue(this ICell cell, TimeSpan? value)
        {
            if (value != null) cell.SetCellValue((TimeSpan)value);
        }


        public static void SetBorder(this ICellStyle cellStyle, BorderStyle borderStyle, short borderColor)
        {
            cellStyle.BorderLeft = borderStyle;
            cellStyle.BorderTop = borderStyle;
            cellStyle.BorderRight = borderStyle;
            cellStyle.BorderBottom = borderStyle;

            cellStyle.LeftBorderColor = borderColor;
            cellStyle.TopBorderColor = borderColor;
            cellStyle.RightBorderColor = borderColor;
            cellStyle.BottomBorderColor = borderColor;
        }
    }
}
