using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Core.ExcelGenerator
{
    public class ExportDataToExcel
    {
        public static byte[] GenerateExcel<T>(IEnumerable<T> plist, string sExcelSheetName)
        {
            string reportName = sExcelSheetName;
            System.Data.DataTable datatable = new System.Data.DataTable();
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                datatable.Columns.Add(prop.Name);
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
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(sExcelSheetName);
                //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                //FOR THE HEADER FORMATING   worksheet.Cells[FromRow, FromColumn, ToRow, ToColumn].Merge = true;                                  
                ws.Cells[1, 1, 1, datatable.Columns.Count].Merge = true;
                ws.Cells[1, 1, 1, datatable.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[1, 1, 1, datatable.Columns.Count].Style.Font.Bold = true;
                ws.Cells[1, 1, 1, datatable.Columns.Count].Value = reportName;
                ws.Cells[1, 1, 1, datatable.Columns.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, 1, 1, datatable.Columns.Count].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(47, 79, 79));//Set color to dark blue
                ws.Cells[1, 1, 1, datatable.Columns.Count].Style.Font.Color.SetColor(System.Drawing.Color.White);
                ws.Cells["A3"].LoadFromDataTable(datatable, true);
                return pck.GetAsByteArray();
            }

        }

    }
}
