using System.Data;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace RestApiReporting.WebApp.Shared;

public static class NpoiExtensions
{
    public static byte[] ConvertToExcel(this ReportDataSet dataSet)
    {
        // xlsx workbook
        using var workbook = new XSSFWorkbook();

        // for any data table a worksheet
        foreach (DataTable table in dataSet.ToDataSet().Tables)
        {
            // empty table
            if (table.Columns.Count == 0)
            {
                continue;
            }

            // sheet
            var sheet = workbook.CreateSheet(table.TableName);

            // header with the table column names
            var headerRow = sheet.CreateRow(0);
            for (var x = 0; x < table.Columns.Count; x++)
            {
                var cell = headerRow.CreateCell(x);
                cell.SetCellValue(table.Columns[x].ColumnName);
            }

            // use header as filter row
            sheet.SetAutoFilter(new CellRangeAddress(0, 0, 0, table.Columns.Count - 1));

            // data rows
            for (var y = 0; y < table.Rows.Count; y++)
            {
                var row = sheet.CreateRow(y + 1);
                var tableRow = table.Rows[y];
                // column value
                for (var x = 0; x < table.Columns.Count; x++)
                {
                    var cell = row.CreateCell(x);
                    var dataValue = tableRow[x];
                    SetCellValue(cell, dataValue);
                }
            }

            // auto size column widths
            foreach (DataColumn column in table.Columns)
            {
                sheet.AutoSizeColumn(column.Ordinal);
            }
        }

        // relations
        if (dataSet.Relations.Any())
        {
            // sheet
            var sheet = workbook.CreateSheet(nameof(dataSet.Relations));

            // header with the relation fields
            var headerRow = sheet.CreateRow(0);
            var nameCell = headerRow.CreateCell(0);
            nameCell.SetCellValue(nameof(ReportDataRelation.Name));
            var parentTableCell = headerRow.CreateCell(1);
            parentTableCell.SetCellValue(nameof(ReportDataRelation.ParentTable));
            var parentColumnCell = headerRow.CreateCell(2);
            parentColumnCell.SetCellValue(nameof(ReportDataRelation.ParentColumn));
            var childTableCell = headerRow.CreateCell(3);
            childTableCell.SetCellValue(nameof(ReportDataRelation.ChildTable));
            var childColumnCell = headerRow.CreateCell(4);
            childColumnCell.SetCellValue(nameof(ReportDataRelation.ChildColumn));
            // use header as filter row
            sheet.SetAutoFilter(new CellRangeAddress(0, 0, 0, 4));

            // data rows
            for (var i = 0; i < dataSet.Relations.Count; i++)
            {
                var relation = dataSet.Relations[i];
                // add header row
                var row = sheet.CreateRow(i + 1);

                nameCell = row.CreateCell(0);
                nameCell.SetCellValue(relation.Name);
                parentTableCell = row.CreateCell(1);
                parentTableCell.SetCellValue(relation.ParentTable);
                parentColumnCell = row.CreateCell(2);
                parentColumnCell.SetCellValue(relation.ParentColumn);
                childTableCell = row.CreateCell(3);
                childTableCell.SetCellValue(relation.ChildTable);
                childColumnCell = row.CreateCell(4);
                childColumnCell.SetCellValue(relation.ChildColumn);
            }

            // auto size column widths
            for (var col = 0; col < 5; col++)
            {
                sheet.AutoSizeColumn(col);
            }
        }

        // result
        using var resultStream = new MemoryStream();
        workbook.Write(resultStream, true);
        resultStream.Seek(0, SeekOrigin.Begin);
        return resultStream.ToArray();
    }

    private static void SetCellValue(ICell cell, object value)
    {
        switch (value)
        {
            case double doubleValue:
                cell.SetCellValue(doubleValue);
                cell.SetCellType(CellType.Numeric);
                break;
            case bool boolValue:
                cell.SetCellValue(boolValue);
                cell.SetCellType(CellType.Boolean);
                break;
            case DateOnly dateOnly:
                cell.SetCellValue(dateOnly.ToShortDateString());
                cell.SetCellType(CellType.String);
                break;
            case DateTime dateTime:
                cell.SetCellValue(
                    $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}");
                cell.SetCellType(CellType.String);
                break;
            default:
                cell.SetCellValue(value.ToString());
                cell.SetCellType(CellType.String);
                break;
        }
    }
}