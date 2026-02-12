#import <Foundation/Foundation.h>

//////平台参数
static int mPlatformID = 100;
static NSString* mPlatformParam1 = @"";
static NSString* mPlatformParam2 = @"";
static NSString* mPlatformParam3 = @"";

//// 参数配置

@interface SDKUtils : NSObject
{
    NSArray *_consumeItems;
    NSArray *_subItems;
    
    NSString* mstrUin;
    NSString* mstrToken;
    
    NSString* mstrLanguage;
	BOOL isSDKLog;
	BOOL isReviewPatentVersion;
	BOOL isReviewStoreVersion;
    
    BOOL _hasBoundLoginWithDevice;
    BOOL _hasBoundLoginWithIGGPassport;

    BOOL bEnablePreventWallow;                   // 是否启用防沉迷检查
    BOOL bEnableRealNameVerification;             // 是否启用实名认证功能
    int nPreventWallowState;     // 防沉迷状态
    BOOL bRealNameVerificationState;             // 实名认证状态
    BOOL bRealNameVerificationForceModel;        // 是否强制模式，强制模式下试玩时间到达后无法继续游戏
    float fPreventWallowDuration;                    // 防沉迷限制总时长 （nPreventWallowState为0或者1有效）
    BOOL bInPreventWallowTime;                   // 是否在防沉迷限制时间断内
    NSString* strPreventWallowTimeStart;                  // 防沉迷限制时间开始
    NSString* strPreventWallowTimeEnd;                    // 防沉迷限制时间结束

    NSString* strAgreementTermsOfServiceURL;
    NSString* strPrivacyPolicyURL;
}

+ (instancetype) shareInstance;
- (NSString*)getPlatformParam1;
- (NSString*)getPlatformParam2;
- (NSString*)getPlatformParam3;
- (NSString*)getCurrentDevice;
- (NSString*)getCurrentDeviceID;
- (int)getPlatformID;
- (int)CopyToClipboard:(NSString*)strValue;
- (int)JumpToAppStore:(NSString*)strValue;
- (void)initUtils;
- (NSUInteger)application:(UIApplication *)application supportedInterfaceOrientationsForWindow:(UIWindow *)window;
- (BOOL)openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation;
- (BOOL)openURL:(NSURL *)url options:(NSDictionary<NSString*, id> *)options;
- (BOOL)handleOpenURL:(NSURL *)url;
- (void)initSDK:(NSString*)language sdkLog:(bool)sdkLog;
- (void)initVersion:(bool)bReviewPatentVersion reviewStoreVersion:(bool)bReviewStoreVersion;
- (void)loadConfig;
- (void)changeLanguage:(NSString*)language;
- (NSString*)getGameIDWithLanguage;
- (void)initSDKWithLanguage:(NSString*)language;
- (void)InitNotchFitMode;
- (void)login;
- (void)logout;
- (void)initPay;
- (NSString*)getProductItems;
- (int)Pay:(NSString*)strProductID strJsonData:(NSString*)jsonData;
- (void)RequestAgreementSignStatus;
- (void)RequestAssignedAgreements;
- (void)SignAgreementSign;
- (void)TranslateText:(NSString*)textid strText:(NSString*)text strDestLanguage:(NSString*)destLanguage;
- (void)ExceptionLog:(NSString*)condition strInfo:(NSString*)info;
- (void)EventLog:(NSString*)strEvent strIGGID:(NSString*)iggid strPlayerLv:(NSString*)playerLv strEMoney:(NSString*)emoney strMoney:(NSString*)money strCreateTime:(NSString*)createTime strIsWIFI:(NSString*)iswifi;
- (void)RequestServiceURL;
- (void)GetComplianceState;
- (void)RealNameVerification;
- (void)LoginWithDevice;
- (void)ConfirmLoginWithDevice:(bool)value;
- (void)LoginWithIGGPassport;
- (void)ConfirmLoginWithIGGPassport:(bool)value;
- (void)BindWithIGGPassport;
- (void)TrackEvent:(NSString*)strEvent strEventValues:(NSString*)strEventValues;
- (void)ShareText:(NSString*)strTitle strContent:(NSString*)content;
- (int)OpenUrlScheme:(NSString*)text;
@end

extern "C" const char* SDKUtilsGetDeviceName();
extern "C" const char* SDKUtilsGetDeviceID();
extern "C" const char* SDKUtilsGetChannelName();
extern "C" const char* SDKUtilsGetSystemLanguage();
extern "C" const char* SDKUtilsGetSystemVersion();
extern "C" const char* SDKUtilsGetPlatformParam1();
extern "C" const char* SDKUtilsGetPlatformParam2();
extern "C" const char* SDKUtilsGetPlatformParam3();
extern "C" const char* SDKUtilsGetProductItems();
extern "C" const char* SDKUtilsGetAgreementURL(int nType);
