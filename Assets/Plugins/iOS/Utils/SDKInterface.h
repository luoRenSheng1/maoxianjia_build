#ifndef SDKINTERFACE_H
#define SDKINTERFACE_H

extern "C" const char* SDK_GetPlatformParam1();
extern "C" const char* SDK_GetPlatformParam2();
extern "C" const char* SDK_GetPlatformParam3();
extern "C" const char* SDK_GetDeviceName();
extern "C" const char* SDK_GetDeviceID();
extern "C" const char* SDK_GetChannelName();
extern "C" const char* SDK_GetSystemLanguage();
extern "C" const char* SDK_GetSystemVersion();
extern "C" int SDK_CopyToClipboard(const char* strValue);
extern "C" int SDK_JumpToAppStore(const char* strValue);
extern "C" int SDK_GetPlatform();
extern "C" int SDK_InitUtils();
extern "C" int SDK_InitSDK(const char* language, bool bSDKLog);
extern "C" int SDK_InitVersion(bool bReviewPatentVersion, bool bReviewStoreVersion);
extern "C" int SDK_UnInitSDK();
extern "C" int SDK_LoadConfig();
extern "C" int SDK_ChangeLanguage(const char* language);
extern "C" int SDK_Login();
extern "C" int SDK_Logout();
extern "C" int SDK_Pay(const char* strProductID, const char* jsonData);
extern "C" const char* SDK_GetProductItems();
extern "C" int SDK_RequestAgreementSignStatus();
extern "C" int SDK_SignAgreementSign();
extern "C" int SDK_TranslateText(const char* textid, const char* text, const char* destLanguage);
extern "C" int SDK_ExceptionLog(const char* condition, const char* info);
extern "C" int SDK_EventLog(const char* strEvent, const char* iggid, const char* playerLv, const char* eMoney, const char* money, const char* createTime, const char* isWifi);
extern "C" const char* SDK_GetAgreementURL(int nType);
extern "C" int SDK_RequestServiceURL();
extern "C" int SDK_GetComplianceState();
extern "C" int SDK_RealNameVerification();
extern "C" int SDK_LoginWithType(int type);
extern "C" int SDK_ConfirmLoginWithType(int type, bool value);
extern "C" int SDK_BindWithType(int type);
extern "C" int SDK_ConfirmBindWithType(int type, bool value);
extern "C" int SDK_TrackEvent(const char* strEvent, const char* eventValue);
extern "C" int SDK_ShareText(const char* strTitle, const char* strContent);
extern "C" int SDK_GetScreenBrightness();
extern "C" int SDK_SetScreenBrightness(int value);
extern "C" int SDK_OpenUrlScheme(const char* text);
#endif
