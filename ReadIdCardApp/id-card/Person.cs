using System;
using System.Collections.Generic;

namespace ReadIdCard.id_card;

public class Person {
    private bool success = false;

    private string name;
    private string sexCode;
    private string sexStr;
    private string id;
    private string nationCode;
    private string nationStr;
    private DateTime birth;
    private string address;
    private string regDept;
    private DateTime beginTime;
    private DateTime endTime;
    private string validCode;
    private string validStr;
    private byte[] pictureBytes;

    private string msg = "";

    public bool Success {
        get => success;
        set => success = value;
    }

    public string Name {
        get => name;
        set => name = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string SexCode {
        get => sexCode;
        set => sexCode = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string SexStr {
        get => sexStr;
        set => sexStr = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Id {
        get => id;
        set => id = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string NationCode {
        get => nationCode;
        set => nationCode = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string NationStr {
        get => nationStr;
        set => nationStr = value ?? throw new ArgumentNullException(nameof(value));
    }

    public DateTime Birth {
        get => birth;
        set => birth = value;
    }

    public string Address {
        get => address;
        set => address = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string RegDept {
        get => regDept;
        set => regDept = value ?? throw new ArgumentNullException(nameof(value));
    }

    public DateTime BeginTime {
        get => beginTime;
        set => beginTime = value;
    }

    public DateTime EndTime {
        get => endTime;
        set => endTime = value;
    }

    public string ValidCode {
        get => validCode;
        set => validCode = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string ValidStr {
        get => validStr;
        set => validStr = value ?? throw new ArgumentNullException(nameof(value));
    }

    public byte[] PictureBytes {
        get => pictureBytes;
        set => pictureBytes = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Msg {
        get => msg;
        set => msg = value ?? throw new ArgumentNullException(nameof(value));
    }

    public static Dictionary<string, string> NationMap = new() {
        { "01", "汉族" },
        { "02", "蒙古族" },
        { "03", "回族" },
        { "04", "藏族" },
        { "05", "维吾尔族" },
        { "06", "苗族" },
        { "07", "彝族" },
        { "08", "壮族" },
        { "09", "布依族" },
        { "10", "朝鲜族" },
        { "11", "满族" },
        { "12", "侗族" },
        { "13", "瑶族" },
        { "14", "白族" },
        { "15", "土家族" },
        { "16", "哈尼族" },
        { "17", "哈萨克族" },
        { "18", "傣族" },
        { "19", "黎族" },
        { "20", "傈僳族" },
        { "21", "佤族" },
        { "22", "畲族" },
        { "23", "高山族" },
        { "24", "拉祜族" },
        { "25", "水族" },
        { "26", "东乡族" },
        { "27", "纳西族" },
        { "28", "景颇族" },
        { "29", "柯尔克孜族" },
        { "30", "土族" },
        { "31", "达翰尔族" },
        { "32", "仫佬族" },
        { "33", "羌族" },
        { "34", "布朗族" },
        { "35", "撒拉族" },
        { "36", "毛南族" },
        { "37", "仡佬族" },
        { "38", "锡伯族" },
        { "39", "阿昌族" },
        { "40", "普米族" },
        { "41", "塔吉克族" },
        { "42", "怒族" },
        { "43", "乌孜别克族" },
        { "44", "俄罗斯族" },
        { "45", "鄂温克族" },
        { "46", "德昂族" },
        { "47", "保安族" },
        { "48", "裕固族" },
        { "49", "京族" },
        { "50", "塔塔尔族" },
        { "51", "独龙族" },
        { "52", "鄂伦春族" },
        { "53", "赫哲族" },
        { "54", "门巴族" },
        { "55", "珞巴族" },
        { "56", "基诺族" },
        { "57", "其它" },
        { "98", "外国人入籍" },
    };
}