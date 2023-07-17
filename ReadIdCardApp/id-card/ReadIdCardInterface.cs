using System.Runtime.InteropServices;

namespace ReadIdCardApp.id_card;

public static class ReadIdCardInterface {
    private const string SdtApi = "./libs/sdtapi.dll";
    private const string WltRs = "./libs/WltRS.dll";

    [DllImport(SdtApi)]
    public static extern int SDT_OpenPort(int port);

    [DllImport(SdtApi)]
    public static extern int SDT_ClosePort(int port);

    [DllImport(SdtApi)]
    public static extern int SDT_PowerManagerBegin(int port, int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_AddSAMUser(int port, string pcUserName, int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_SAMLogin(
        int port,
        string pcUserName,
        string pcPasswd,
        int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_SAMLogout(int port, int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_UserManagerOK(int port, int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_ChangeOwnPwd(
        int port,
        string pcOldPasswd,
        string pcNewPasswd,
        int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_ChangeOtherPwd(
        int port,
        string pcUserName,
        string pcNewPasswd,
        int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_DeleteSAMUser(int port, string pcUserName, int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_StartFindIDCard(int port, ref int pucIIN, int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_SelectIDCard(int port, ref int pucSN, int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_ReadBaseMsg(
        int port,
        byte[] pucCHMsg,
        ref int puiCHMsgLen,
        byte[] pucPHMsg,
        ref int puiPHMsgLen,
        int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_ReadBaseMsgToFile(
        int port,
        string fileName1,
        ref int puiCHMsgLen,
        string fileName2,
        ref int puiPHMsgLen,
        int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_WriteAppMsg(
        int port,
        ref byte pucSendData,
        int uiSendLen,
        ref byte pucRecvData,
        ref int puiRecvLen,
        int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_WriteAppMsgOK(
        int port,
        ref byte pucData,
        int uiLen,
        int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_CancelWriteAppMsg(int port, int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_ReadNewAppMsg(
        int port,
        ref byte pucAppMsg,
        ref int puiAppMsgLen,
        int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_ReadAllAppMsg(
        int port,
        ref byte pucAppMsg,
        ref int puiAppMsgLen,
        int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_UsableAppMsg(int port, ref byte ucByte, int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_GetUnlockMsg(int port, ref byte strMsg, int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_GetSAMID(int port, ref byte StrSAMID, int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_SetMaxRFByte(int port, byte ucByte, int iIfOpen);

    [DllImport(SdtApi)]
    public static extern int SDT_ResetSAM(int port, int iIfOpen);

    [DllImport(WltRs)]
    public static extern int GetBmp(string file_name, int intf);
}