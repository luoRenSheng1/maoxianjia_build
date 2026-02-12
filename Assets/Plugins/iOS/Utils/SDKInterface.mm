#include "SDKInterface.h"
#include "SDKUtils.h"

NSString* CStringToNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}

const char* SDK_GetPlatformParam1()
{
    return SDKUtilsGetPlatformParam1();
}

const char* SDK_GetPlatformParam2()
{
    return SDKUtilsGetPlatformParam2();
}

const char* SDK_GetPlatformParam3()
{
    return SDKUtilsGetPlatformParam3();
}

const char* SDK_GetDeviceName()
{
    return SDKUtilsGetDeviceName();
}

const char* SDK_GetDeviceID()
{
    return SDKUtilsGetDeviceID();
}

const char* SDK_GetChannelName()
{
    return SDKUtilsGetChannelName();
}

const char* SDK_GetSystemLanguage()
{
    return SDKUtilsGetSystemLanguage();
}

const char* SDK_GetSystemVersion()
{
    return SDKUtilsGetSystemVersion();
}

int SDK_CopyToClipboard(const char* strValue)
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils CopyToClipboard:CStringToNSString(strValue)];
    return 0;
}

int SDK_JumpToAppStore(const char* strValue)
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils JumpToAppStore:CStringToNSString(strValue)];
    return 0;
}

int SDK_GetPlatform()
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    return [sdkUtils getPlatformID];
}

int SDK_InitUtils()
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils initUtils];

    return 1;
}

int SDK_InitSDK(const char* language, bool bSDKLog)
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils initSDK:CStringToNSString(language) sdkLog:bSDKLog];

    return 0;
}

int SDK_InitVersion(bool bReviewPatentVersion, bool bReviewStoreVersion)
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils initVersion:bReviewPatentVersion reviewStoreVersion:bReviewStoreVersion];

    return 0;
}

int SDK_UnInitSDK()
{
    return 0;
}

int SDK_LoadConfig()
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils loadConfig];
    
    return 0;
}

int SDK_ChangeLanguage(const char* language)
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils changeLanguage:CStringToNSString(language)];
    
    return 0;
}

int SDK_Login()
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils login];
    return 0;
}

int SDK_Logout()
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils logout];
    return 0;//0：立即完成，1:等待sdk内部回调事件
}

int SDK_Pay(const char* strProductID, const char* jsonData)
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    return [sdkUtils Pay:CStringToNSString(strProductID) strJsonData:CStringToNSString(jsonData)];
}

const char* SDK_GetProductItems()
{
    return SDKUtilsGetProductItems();
}

int SDK_RequestAgreementSignStatus()
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils RequestAgreementSignStatus];
    return 1;
}

int SDK_SignAgreementSign()
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils SignAgreementSign];
    return 1;
}

int SDK_TranslateText(const char* textid, const char* text, const char* destLanguage)
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils TranslateText:CStringToNSString(textid) strText:CStringToNSString(text) strDestLanguage:CStringToNSString(destLanguage)];
    
    return 1;
}

int SDK_ExceptionLog(const char* condition, const char* info)
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils ExceptionLog:CStringToNSString(condition) strInfo:CStringToNSString(info)];
    
    return 1;
}

int SDK_EventLog(const char* strEvent, const char* iggid, const char* playerLv, const char* eMoney, const char* money, const char* createTime, const char* isWifi)
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils EventLog:CStringToNSString(strEvent) strIGGID:CStringToNSString(iggid) strPlayerLv:CStringToNSString(playerLv) strEMoney:CStringToNSString(eMoney) strMoney:CStringToNSString(money) strCreateTime:CStringToNSString(createTime) strIsWIFI:CStringToNSString(isWifi)];
    
    return 1;
}

const char* SDK_GetAgreementURL(int nType)
{
	return SDKUtilsGetAgreementURL(nType);
}

int SDK_RequestServiceURL()
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils RequestServiceURL];
    
    return 1;
}

int SDK_GetComplianceState()
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils GetComplianceState];
    
    return 1;
}

int SDK_RealNameVerification()
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils RealNameVerification];
    
    return 1;
}

int SDK_LoginWithType(int type)
{
    if (type == 1) {
        SDKUtils* sdkUtils = [SDKUtils shareInstance];
        [sdkUtils LoginWithDevice];
    } else if (type == 13) {
        SDKUtils* sdkUtils = [SDKUtils shareInstance];
        [sdkUtils LoginWithIGGPassport];
    }
    return 1;
}

int SDK_ConfirmLoginWithType(int type, bool value)
{
    if (type == 1) {
        SDKUtils* sdkUtils = [SDKUtils shareInstance];
        [sdkUtils ConfirmLoginWithDevice:value];
    } else if (type == 13) {
        SDKUtils* sdkUtils = [SDKUtils shareInstance];
        [sdkUtils ConfirmLoginWithIGGPassport:value];
    }
    return 1;
}

int SDK_BindWithType(int type)
{
    return 1;
}

int SDK_ConfirmBindWithType(int type, bool value)
{
    return 1;
}

int SDK_TrackEvent(const char* strEvent, const char* eventValue)
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils TrackEvent:CStringToNSString(strEvent) strEventValues:CStringToNSString(eventValue)];

    return 1;
}

int SDK_ShareText(const char* title, const char* content)
{
    SDKUtils* sdkUtils = [SDKUtils shareInstance];
    [sdkUtils ShareText:CStringToNSString(title) strContent:CStringToNSString(content)];
    
    return 1;
}

int SDK_GetScreenBrightness()
{
    return 1;
}

int SDK_SetScreenBrightness(int value)
{
    return 1;
}



int SDK_OpenUrlScheme(const char* text)
{
	SDKUtils* sdkUtils = [SDKUtils shareInstance];
    return [sdkUtils OpenUrlScheme:CStringToNSString(text)];
}
