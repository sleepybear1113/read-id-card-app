using System.Collections.Generic;

namespace ReadIdCardApp.excel;

public class ExcelData {
    public List<string?> Header { get; set; }
    public List<List<string?>> DataList { get; set; }

    public List<List<string?>> Get(List<string?>? colNames, List<int>? rows) {
        List<List<string?>> list = new List<List<string?>>();

        List<int> colIndexes = new List<int>();
        if (colNames == null) {
            colNames = Header;
        }

        foreach (var colName in colNames) {
            colIndexes.Add(GetColIndex(colName));
        }

        int size = DataList.Count;
        if (rows == null) {
            rows = new List<int>();
            for (int i = 0; i < DataList.Count; i++) {
                rows.Add(i);
            }
        }

        for (var i = 0; i < rows.Count; i++) {
            if (i >= size) {
                continue;
            }

            List<string?> row = new List<string?>();
            foreach (var colIndex in colIndexes) {
                row.Add(colIndex < 0 || colIndex >= DataList[i].Count ? null : DataList[i][colIndex]);
            }

            list.Add(row);
        }

        return list;
    }

    public string? Get(string colName, int row) {
        var list = Get(new List<string?> { colName }, new List<int> { row });
        if (list.Count == 0) {
            return null;
        }

        return list[0][0];
    }

    public int GetColIndex(string colName) {
        for (var i = 0; i < Header.Count; i++) {
            if (Header[i] == colName) {
                return i;
            }
        }

        return -1;
    }
}