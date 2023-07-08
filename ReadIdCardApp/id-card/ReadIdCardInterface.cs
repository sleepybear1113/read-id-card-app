using System.Runtime.InteropServices;

namespace ReadIdCard.id_card;

public class ReadIdCardInterface {
    [DllImport("sdtapi.dll")]
    public static extern int SDT_OpenPort(int iPortID);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_ClosePort(int iPortID);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_PowerManagerBegin(int iPortID, int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_AddSAMUser(int iPortID, string pcUserName, int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_SAMLogin(
        int iPortID,
        string pcUserName,
        string pcPasswd,
        int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_SAMLogout(int iPortID, int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_UserManagerOK(int iPortID, int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_ChangeOwnPwd(
        int iPortID,
        string pcOldPasswd,
        string pcNewPasswd,
        int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_ChangeOtherPwd(
        int iPortID,
        string pcUserName,
        string pcNewPasswd,
        int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_DeleteSAMUser(int iPortID, string pcUserName, int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_StartFindIDCard(int iPortID, ref int pucIIN, int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_SelectIDCard(int iPortID, ref int pucSN, int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_ReadBaseMsg(
        int iPortID,
        byte[] pucCHMsg,
        ref int puiCHMsgLen,
        byte[] pucPHMsg,
        ref int puiPHMsgLen,
        int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_ReadBaseMsgToFile(
        int iPortID,
        string fileName1,
        ref int puiCHMsgLen,
        string fileName2,
        ref int puiPHMsgLen,
        int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_WriteAppMsg(
        int iPortID,
        ref byte pucSendData,
        int uiSendLen,
        ref byte pucRecvData,
        ref int puiRecvLen,
        int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_WriteAppMsgOK(
        int iPortID,
        ref byte pucData,
        int uiLen,
        int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_CancelWriteAppMsg(int iPortID, int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_ReadNewAppMsg(
        int iPortID,
        ref byte pucAppMsg,
        ref int puiAppMsgLen,
        int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_ReadAllAppMsg(
        int iPortID,
        ref byte pucAppMsg,
        ref int puiAppMsgLen,
        int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_UsableAppMsg(int iPortID, ref byte ucByte, int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_GetUnlockMsg(int iPortID, ref byte strMsg, int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_GetSAMID(int iPortID, ref byte StrSAMID, int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_SetMaxRFByte(int iPortID, byte ucByte, int iIfOpen);

    [DllImport("sdtapi.dll")]
    public static extern int SDT_ResetSAM(int iPortID, int iIfOpen);

    [DllImport("WltRS.dll")]
    public static extern int GetBmp(string file_name, int intf);
}