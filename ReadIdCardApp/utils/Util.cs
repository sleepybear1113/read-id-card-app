using System;
using System.Text.RegularExpressions;

namespace ReadIdCardApp.utils;

public static class Util {
    public static long? ToUnixMillisecond(DateTime dateTime) {
        DateTime unixEpochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan timeSpan = dateTime.ToUniversalTime() - unixEpochStart;
        return (long)timeSpan.TotalMilliseconds;
    }

    public static DateTime? FormatFromUnixTime(long? time) {
        if (time != null) {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds((long)time);
            return TimeZoneInfo.ConvertTime(dateTimeOffset, TimeZoneInfo.Local).DateTime;
        } else {
            return null;
        }
    }

    public static string FormatTimeBegin(long? timeBegin) {
        if (timeBegin != null) {
            var dateTime = (DateTime)FormatFromUnixTime(timeBegin);
            return dateTime.ToString("yyyy'年'MM'月'dd'日' HH:mm:ss");
        } else {
            return string.Empty;
        }
    }

    public static bool IsPositiveInteger(string str) {
        // 使用正则表达式进行匹配
        Regex regex = new Regex(@"^[1-9]\d*$");
        return regex.IsMatch(str);
    }

    /// <summary>
    /// 校验身份证合法性
    /// </summary>
    /// <param name="idCardNumber"></param>
    /// <returns>-1为空，-2前面17位格式异常，-3校验位错误，1正确</returns>
    public static int ValidateIdCard(string? idCardNumber) {
        if (string.IsNullOrEmpty(idCardNumber))
            return -1;

        // 校验规则
        string pattern = @"^\d{17}(\d|X|x)$";
        if (!Regex.IsMatch(idCardNumber, pattern))
            return -2;

        // 校验最后一位校验码
        int[] factors = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
        string[] parityBits = new string[] { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };

        int sum = 0;
        for (int i = 0; i < 17; i++) {
            sum += int.Parse(idCardNumber[i].ToString()) * factors[i];
        }

        int modulus = sum % 11;
        string lastDigit = idCardNumber.Substring(17, 1);

        return lastDigit.Equals(parityBits[modulus], StringComparison.OrdinalIgnoreCase) ? 1 : -3;
    }
}