using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Data;

namespace RestApp.Common.Utility
{
    public class ExportUtility
    {
        public static MemoryStream ExportToExcel(DataTable data)
        {
            // Create new Excel workbook
            var workbook = new HSSFWorkbook();

            // Create new Excel sheet
            var sheet = workbook.CreateSheet();

            // Create a header row
            var headerRow = sheet.CreateRow(0);

            // Set the column names in the header row
            int columnNumber = 0;
            foreach (DataColumn column in data.Columns)
	        {
                headerRow.CreateCell(columnNumber).SetCellValue(column.Caption);
                columnNumber++;
            }

            // Freeze the header row so it is not scrolled
            sheet.CreateFreezePane(0, 1, 0, 1);

            int rowNumber = 1;
            // Populate the sheet with values from the grid data
            foreach (DataRow row in data.Rows)
            {
                var dataRow = sheet.CreateRow(rowNumber);

                columnNumber = 0;
                foreach (DataColumn column in data.Columns)
                {
                    dataRow.CreateCell(columnNumber).SetCellValue(row[columnNumber].ToString());
                    columnNumber++;
                }

                rowNumber++;
            }

            foreach (DataColumn column in data.Columns)
            {
                sheet.AutoSizeColumn(column.Ordinal);
            }

            // Write the workbook to a memory stream
            MemoryStream output = new MemoryStream();
            workbook.Write(output);

            return output;
        }
    }
}