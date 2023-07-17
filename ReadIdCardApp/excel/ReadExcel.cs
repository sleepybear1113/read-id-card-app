using System.Collections.Generic;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ReadIdCardApp.excel; 

public class ReadExcel {

    public static ExcelData? Read(string filePath) {
        using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        IWorkbook workbook;
        // 根据文件类型选择适当的工作簿类
        if (Path.GetExtension(filePath) == ".xlsx") {
            workbook = new XSSFWorkbook(stream);
        } else if (Path.GetExtension(filePath) == ".xls") {
            workbook = new HSSFWorkbook(stream);
        } else {
            return null;
        }

        // 选择第一个工作表
        ISheet sheet = workbook.GetSheetAt(0);

        int totalRows = sheet.PhysicalNumberOfRows;
        // 获取标题行
        IRow headerRow = sheet.GetRow(0);
        int columnCount = headerRow.LastCellNum;

        var excelData = new ExcelData();
        List<string?> header = new List<string?>();
        foreach (var cell in headerRow) {
            header.Add(cell.ToString());
        }
        excelData.Header = header;

        // 获取该列的数据
        // 从第二行开始读取数据
        List<List<string?>> list = new List<List<string?>>();
        for (int row = 1; row < totalRows; row++) {
            IRow excelRow = sheet.GetRow(row);

            List<string?> rowData = new List<string?>();
            for (int j = 0; j < columnCount; j++) {
                ICell cell = excelRow.GetCell(j);
                if (cell != null) {
                    rowData.Add(cell.ToString());
                }
            }

            list.Add(rowData);
        }

        excelData.DataList = list;
        return excelData;
    }
}