using System;
using System.IO;
using System.Text;

namespace ReadIdCardApp.id_card;

public static class ReadIdCard {
    private static int[] _portList = new[]
        { 0, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1012, 1013, 1014, 1015, 1016, 1, 2 };

    private static bool OpenPort() {
        bool openPort = _portList[0] != 0;

        if (openPort) {
            return true;
        }

        foreach (var p in _portList) {
            if (p <= 0) {
                continue;
            }

            if (ReadIdCardInterface.SDT_OpenPort(p) == 144) {
                _portList[0] = p;
                openPort = true;
                break;
            }
        }

        if (!openPort) {
            Console.WriteLine("端口打开失败，请检测相应的端口或者重新连接读卡器！");
        }

        return openPort;
    }

    public static Person Read(bool readPic) {
        int EdziIfOpen = 1;

        Person person = new Person();
        if (!OpenPort()) {
            person.Msg = "端口打开失败，请检测读卡器是否连接，或者重新进行连接！";
            return person;
        }

        int pucIIN = 0;
        if (ReadIdCardInterface.SDT_StartFindIDCard(_portList[0], ref pucIIN, EdziIfOpen) != 159) {
            person.Msg = "未找到卡，请重新放卡";
            return person;
        }

        int pucSN = 0;
        if (ReadIdCardInterface.SDT_SelectIDCard(_portList[0], ref pucSN, EdziIfOpen) != 144) {
            person.Msg = "选择卡片失败！";
            return person;
        }

        // 准备读卡并保存到下面的两个 bytes 中
        byte[] infoBytes = new byte[512];
        byte[] picBytes = new byte[3024];
        int infoLength = 0;
        int picLength = 0;
        if (ReadIdCardInterface.SDT_ReadBaseMsg(_portList[0], infoBytes, ref infoLength, picBytes, ref picLength,
                EdziIfOpen) != 144) {
            person.Msg = "读卡失败！";
            return person;
        }

        // 解析 bytes 内容
        person.Success = true;
        person.Name = Encoding.Unicode.GetString(infoBytes, 0, 30).Trim();
        person.SexCode = Encoding.Unicode.GetString(infoBytes, 30, 2).Trim();
        person.NationCode = Encoding.Unicode.GetString(infoBytes, 32, 4).Trim();

        string str4 = Encoding.Unicode.GetString(infoBytes, 36, 16).Trim();
        person.Birth = Convert.ToDateTime(str4.Substring(0, 4) + "年" +
                                          str4.Substring(4, 2) + "月" +
                                          str4.Substring(6) + "日");

        person.Address = Encoding.Unicode.GetString(infoBytes, 52, 70).Trim();
        person.Id = Encoding.Unicode.GetString(infoBytes, 122, 36).Trim();
        person.RegDept = Encoding.Unicode.GetString(infoBytes, 158, 30).Trim();

        string str5 = Encoding.Unicode.GetString(infoBytes, 188, infoBytes.GetLength(0) - 188).Trim();
        person.BeginTime = Convert.ToDateTime(str5.Substring(0, 4) + "年" +
                                              str5.Substring(4, 2) + "月" +
                                              str5.Substring(6, 2) + "日");

        string str6 = str5.Substring(8);
        person.EndTime = DateTime.MaxValue;
        if (str6.Trim() != "长期") {
            person.EndTime = Convert.ToDateTime(str6.Substring(0, 4) + "年" +
                                                str6.Substring(4, 2) + "月" +
                                                str6.Substring(6, 2) + "日");
        }

        if (readPic) {
            // 读取 wlt 的 bytes 写本地 wlt 文件，转为 bmp 文件流
            var wltPathPrefix = $"wlt-{person.Id}";
            var wltPath = wltPathPrefix + ".wlt";
            var bmpPath = wltPathPrefix + ".bmp";
            File.WriteAllBytes(wltPath, GetFirstBytes(picBytes, picLength));
            var wltFile = new FileInfo(wltPath);
            if (wltFile.Exists) {
                switch (ReadIdCardInterface.GetBmp(wltPath, 2)) {
                    case -6:
                        person.Msg = "读卡失败！";
                        break;
                    case -4:
                        person.Msg = "加密相片文件格式错误！";
                        break;
                    case -3:
                        person.Msg = "加密相片文件打开错误！";
                        break;
                    case -2:
                        person.Msg = "加密相片文件后缀错误！";
                        break;
                    case -1:
                        person.Msg = "相片解码错误！";
                        break;
                }

                wltFile.Delete();
            } else {
                person.Msg = "未找到加密相片文件";
            }

            var bmpFile = new FileInfo(bmpPath);
            if (bmpFile.Exists) {
                person.PictureBytes = File.ReadAllBytes(bmpPath);
                bmpFile.Delete();
            } else {
                person.PictureBytes = null;
            }
        }

        return person;
    }

    public static Person ReadWithException(bool readPic) {
        try {
            return Read(readPic);
        } catch (Exception e) {
            return new Person() { Msg = e.Message };
        }
    }

    private static byte[] GetFirstBytes(byte[] bytes, int length) {
        byte[] firstBytes = new byte[length];
        Array.Copy(bytes, 0, firstBytes, 0, length);
        return firstBytes;
    }
}