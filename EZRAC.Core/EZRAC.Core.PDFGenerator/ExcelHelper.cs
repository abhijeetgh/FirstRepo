using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace EZRAC.Core.FileGenerator
{
    public class ExcelHelper
    {
        public static byte[] GenerateExcel<T>(IEnumerable<T> plist, string sExcelSheetName)
        {
            string reportName = sExcelSheetName;
            System.Data.DataTable datatable = new System.Data.DataTable();

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                string PropertyName = prop.Name;
                //Setting column names as Property names
                datatable.Columns.Add(PropertyName);
            }
            foreach (T item in plist)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item);
                }
                datatable.Rows.Add(values);
            }

            return GenerateDataTableExcel(datatable, sExcelSheetName);
            
        }
        public static byte[] GenerateDataTableExcel(System.Data.DataTable plist, string sExcelSheetName)
        {
            string reportName = sExcelSheetName;

            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(sExcelSheetName);
                //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                //FOR THE HEADER FORMATING   worksheet.Cells[FromRow, FromColumn, ToRow, ToColumn].Merge = true;                                  
                ws.Cells[1, 1, 1, plist.Columns.Count].Merge = true;
                ws.Cells[1, 1, 1, plist.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[1, 1, 1, plist.Columns.Count].Style.Font.Bold = true;
                ws.Cells[1, 1, 1, plist.Columns.Count].Value = reportName;
                ws.Cells[1, 1, 1, plist.Columns.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, 1, 1, plist.Columns.Count].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(47, 79, 79));//Set color to dark blue
                ws.Cells[1, 1, 1, plist.Columns.Count].Style.Font.Color.SetColor(System.Drawing.Color.White);
                ws.Cells["A3"].LoadFromDataTable(plist, true);
                return pck.GetAsByteArray();
            }
        }
    }
}