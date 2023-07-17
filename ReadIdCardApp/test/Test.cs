using System;
using System.Collections.Generic;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ReadIdCardApp.excel;

namespace ReadIdCardApp.test;

public class Test {
    public static void T() {
        var excelData = ReadExcel.Read(@"E:\工作文档\Excel\副本学考地理_教师信息-导入签到.xlsx");
        Console.WriteLine();
    }
}