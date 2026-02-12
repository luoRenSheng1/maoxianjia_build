using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Engine;
using EngineBase;
using System;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ThirdParty.Wrapper;

public class SDKInterface : MonoBehaviour
{
    // SDKInterface
    static SDKInterface _instance = null;

    static public SDKInterface Instance
    {
        get { return _instance; }
    }

    private enum EN_OS_TYPE
    {
        NONE = 0, // 未知系统
        
        Windows = 101, // PC
        Windows_Editor = 102,
        OSX = 111,
        OSX_Editor = 112,
        Linux = 121,
        Linux_Editor = 122,
        Android = 201, // Android
        IOS = 301, // IOS
        WebGL = 401, // WebGL
        WebGL_WX_NONE = 410,
        WebGL_WX_Windows = 411,
        WebGL_WX_Mac = 412,
        WebGL_WX_Android = 413,
        WebGL_WX_IOS = 414,
        WebGL_WX_DevTools = 415,
    }
    
    public const int PLATFORM_CODE_GENERAL = 100; // 通用平台-内服
    public const int PLATFORM_CODE_WEIXIN = 500; // 微信小游戏
    public const int PLATFORM_CODE_DOUYIN = 600; // 抖音小游戏
    public const int PLATFORM_CODE_COMBO = 700; // 世游android
    public const int PLATFORM_CODE_COMBO_IOS = 800; // 世游ios
    public const int PLATFORM_TAPTAP_SDK = 1200; // TapSDK

    public const int LOGIN_RESULT_SUCCESS = 1;
    public const int LOGIN_RESULT_FAIL = 2;
    public const int LOGIN_RESULT_CANCEL = 3;

    public const int BIND_RESULT_SUCCESS = 20;
    public const int BIND_RESULT_FAIL = 21;
    public const int BIND_RESULT_CANCEL = 22;

    public const int LOGOUT_SUCCESS = 23;
    public const int LOGOUT_FORCE = 24;
    public const int LOGOUT_FAIL = 25;

    public const int SHARE_RESULT_SUCCESS = 26;
    public const int SHARE_RESULT_FAIL = 27;
    public const int SHARE_RESULT_CANCEL = 28;

    public const int PAY_RESULT_SUCCESS = 31;
    public const int PAY_RESULT_FAIL = 32;
    public const int PAY_RESULT_CANCEL = 33;
    
    public const int PRELOAD_AD_RESULT_SUCCESS = 41;
    public const int PRELOAD_AD_RESULT_FAIL = 42;
    
    public const int SHOW_AD_RESULT_SUCCESS = 45;
    public const int SHOW_AD_RESULT_FAIL = 46;

    private int nPlatformCode = 0;
    private int nFullPlatformCode = 0;
    private string strUIN = "";
    private string strSession = "";
    private string strPlatformOpts = "";

    private List<string> lstExceptionLogLast = new List<string>();

    public bool NotchFit => ThirdPartyWrapper.Instance.NotchFit;
    public int NotchFitWidth => ThirdPartyWrapper.Instance.NotchFitWidth;
    public int NotchFitHeight => ThirdPartyWrapper.Instance.NotchFitHeight;
    
    public static SDKInterface CreateNew(int nFullPlatform)
    {
        GameObject go = new GameObject();
        go.name = "SDKInterface";
        DontDestroyOnLoad(go);

        _instance = go.AddComponent<SDKInterface>();
        _instance.nFullPlatformCode = nFullPlatform;
        
        return _instance;
    }

    public static void CleanUp()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }

    void Awake()
    {
        EnableExceptionHandler();

#if PF_WEIXIN
        SDKInterfaceWeiXin.Instance.DoAwake();
#elif PF_DOUYIN
        SDKInterfaceDouYin.Instance.DoAwake();
#elif !UNITY_EDITOR && PF_TAPTAPSDK
        TapSDKManager.Instance.InitTapSDK();
#endif
    }

    void OnDestroy()
    {
        DisableExceptionHandler();

#if PF_WEIXIN
        SDKInterfaceWeiXin.Instance.DoDestroy();
#elif PF_DOUYIN
        SDKInterfaceDouYin.Instance.DoDestroy();
#elif !UNITY_EDITOR && PF_TAPTAPSDK
        TapSDKManager.Instance.DoDestroy();
#endif
    }

    void LateUpdate()
    {
#if !UNITY_EDITOR && PF_WEIXIN
        SDKInterfaceWeiXin.Instance.DoLateUpdate();
#endif
    }

    // 获取渠道号
    public int GetPlatform()
    {
        if (nPlatformCode <= 0)
        {
#if !UNITY_EDITOR && PF_WEIXIN
            nPlatformCode = PLATFORM_CODE_WEIXIN;
#elif !UNITY_EDITOR && PF_DOUYIN
            nPlatformCode = PLATFORM_CODE_DOUYIN;
#elif !UNITY_EDITOR && PF_TAPTAPSDK
            nPlatformCode = PLATFORM_TAPTAP_SDK;
#elif !UNITY_EDITOR && UNITY_ANDROID
            nPlatformCode = ThirdPartyWrapper.CallJavaWithReturn<int>("GetPlatform");
#elif !UNITY_EDITOR && UNITY_IOS
            nPlatformCode = ThirdPartyWrapper.SDK_GetPlatform();
#else
            nPlatformCode = PLATFORM_CODE_GENERAL;
#endif
        }

        return nPlatformCode;
    }

    // 获取全渠道号-(含子渠道)
    public int GetFullPlatform()
    {
        int nPlatform = GetPlatform();

        if (nPlatform == PLATFORM_CODE_COMBO)
        {
            return nFullPlatformCode;
        }

        return nPlatform;
    }

    // 是否内部渠道
    public bool IsNormalPlatform()
    {
        int nPlatform = GetPlatform();

        if (nPlatform == PLATFORM_CODE_GENERAL)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 是否微信小游戏
    /// </summary>
    /// <returns></returns>
    public bool IsWeixinPlatform()
    {
        return GetPlatform() == PLATFORM_CODE_WEIXIN;
    }

    /// <summary>
    /// 是否抖音小游戏
    /// </summary>
    /// <returns></returns>
    public bool IsDouyinPlatform()
    {
        return GetPlatform() == PLATFORM_CODE_DOUYIN;
    }
    
    /// <summary>
    /// 是否Combo
    /// </summary>
    /// <returns></returns>
    public bool IsComboPlatform()
    {
        int platform = GetPlatform();
        return platform == PLATFORM_CODE_COMBO || platform == PLATFORM_CODE_COMBO_IOS;
    }

    /// <summary>
    /// 是否 taptapSDK
    /// </summary>
    /// <returns></returns>
    public bool IsTapTapPlatform()
    {
        int platform = GetPlatform();
        return platform == PLATFORM_TAPTAP_SDK;
    }
    
    public bool IsManualChooseServer()
    {
#if USE_MANUALSERVER
        return true;
#else
        return false;
#endif
    }

    public bool IsAutoLogin()
    {
        var platform = GetPlatform();
        return platform == PLATFORM_CODE_WEIXIN
               || platform == PLATFORM_CODE_DOUYIN
               || platform == PLATFORM_CODE_COMBO
               || platform == PLATFORM_CODE_COMBO_IOS;
    }
    
    // 获取渠道ID
    public string GetChannelID()
    {
        return GetChannelName();
    }

    // 获取渠道名
    public string GetChannelName()
    {
        return ThirdPartyWrapper.Instance.GetChannelName();
    }
    
    // 获取分包标识
    public string GetVariant()
    {
        return ThirdPartyWrapper.Instance.GetVariant();
    }

    // 获取设备名
    public string GetDevice()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        return ThirdPartyWrapper.CallJavaWithReturn<string>("GetDeviceName");
#elif !UNITY_EDITOR && UNITY_IOS
        return ThirdPartyWrapper.SDK_GetDeviceName();
#else
        return "pc";
#endif
    }

    // 获取设备ID
    public string GetDeviceCode()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        string strDeviceID = ThirdPartyWrapper.CallJavaWithReturn<string>("GetDeviceID");
        return string.Format("{0}", strDeviceID);
#elif !UNITY_EDITOR && UNITY_IOS
        return string.Format("{0}", ThirdPartyWrapper.SDK_GetDeviceID());
#else
        return string.Format("{0}", Guid.NewGuid().ToString());
#endif
    }

    // 获取操作系统
    public int GetOS()
    {
#if !UNITY_EDITOR && PF_WEIXIN
        if (IsMinigamePlatform(MinigamePlatform.ios))
        {
            return (int)EN_OS_TYPE.WebGL_WX_IOS;
        }
        else if (IsMinigamePlatform(MinigamePlatform.android))
        {
            return (int)EN_OS_TYPE.WebGL_WX_Android;
        }
        else if (IsMinigamePlatform(MinigamePlatform.mac))
        {
            return (int)EN_OS_TYPE.WebGL_WX_Mac;
        }
        else if (IsMinigamePlatform(MinigamePlatform.windows))
        {
            return (int)EN_OS_TYPE.WebGL_WX_Windows;
        }
        else if (IsMinigamePlatform(MinigamePlatform.devtools))
        {
            return (int)EN_OS_TYPE.WebGL_WX_DevTools;
        }
        else
        {
            return (int)EN_OS_TYPE.WebGL_WX_NONE;
        }
#else
        var osType = EN_OS_TYPE.NONE;
        
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
                osType = EN_OS_TYPE.Windows;
                break;
            
            case RuntimePlatform.WindowsEditor:
                osType = EN_OS_TYPE.Windows_Editor;
                break;
            
            case RuntimePlatform.OSXPlayer:
                osType = EN_OS_TYPE.OSX;
                break;
            
            case RuntimePlatform.OSXEditor:
                osType = EN_OS_TYPE.OSX_Editor;
                break;

            case RuntimePlatform.LinuxPlayer:
                osType = EN_OS_TYPE.Linux;
                break;
            
            case RuntimePlatform.LinuxEditor:
                osType = EN_OS_TYPE.Linux_Editor;
                break;
            
            case RuntimePlatform.Android:
                osType = EN_OS_TYPE.Android;
                break;

            case RuntimePlatform.IPhonePlayer:
                osType = EN_OS_TYPE.IOS;
                break;
            
            case RuntimePlatform.WebGLPlayer:
                osType = EN_OS_TYPE.WebGL;
                break;
            
            default:
                break;
        }
        
        return (int)osType;
#endif
    }

    // 获取操作系统版本
    public string GetSystemVersion()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        string strValue = ThirdPartyWrapper.CallJavaWithReturn<string>("GetSystemVersion");
        return string.Format("{0}", strValue);
#elif !UNITY_EDITOR && UNITY_IOS
        return string.Format("{0}", ThirdPartyWrapper.SDK_GetSystemVersion());
#else
        return string.Format("{0}", "");
#endif
    }

    // 获取cpu Name
    public string GetSOCs()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        return ThirdPartyWrapper.CallJavaWithReturn<string>("GetCPUName");
#else
        return SystemInfo.processorType;
#endif
    }

    public int touchCount
    {
#if !UNITY_EDITOR && PF_WEIXIN
        get { return SDKInterfaceWeiXin.Instance.touchCount; }
#else
        get { return Input.touchCount; }
#endif
    }

    public Touch GetTouch(int index)
    {
#if !UNITY_EDITOR && PF_WEIXIN
        return SDKInterfaceWeiXin.Instance.GetTouch(index);
#else
        return Input.GetTouch(index);
#endif
    }

    public bool InputAnyKey
    {
#if !UNITY_EDITOR && PF_WEIXIN
        get { return SDKInterfaceWeiXin.Instance.InputAnyKey; }
#else
        get { return Input.anyKey; }
#endif
    }

    public void HttpGet(string strUrl, Action<string> actionSuccess, Action actionFail)
    {
        VersionManager.Instance.HttpGet(this, strUrl, actionSuccess, actionFail);
    }

    public string GetProductItems()
    {
        LogUtils.LogWarning("GetProductItems");

#if !UNITY_EDITOR && UNITY_ANDROID
        return ThirdPartyWrapper.CallJavaWithReturn<string>("GetProductItems");
#elif !UNITY_EDITOR && UNITY_IOS
        return ThirdPartyWrapper.SDK_GetProductItems();
#endif

        return "";
    }

    // 获取操作系统语言
    public string GetSystemLanguage()
    {
        string strValue = "";
        string strLanguage = "";

#if !UNITY_EDITOR && UNITY_ANDROID
        strValue = ThirdPartyWrapper.CallJavaWithReturn<string>("GetSystemLanguage");
#elif !UNITY_EDITOR && UNITY_IOS
        strValue = ThirdPartyWrapper.SDK_GetSystemLanguage();
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
        strValue = "zh-Hans";
#endif

        LogUtils.LogWarningFormat("GetSystemLanguage {0}", strValue);

        if (!string.IsNullOrEmpty(strValue))
        {
            string strLanguageName = "";
            string strLanguageArea = "";

            if (!Utils.GetSplitStringByLast(strValue, '-', ref strLanguageName, ref strLanguageArea))
            {
                // 分割失败，默认为语言名
                strLanguageName = strValue;
            }

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                {
                    if (String.Equals(strLanguageName, "zh", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // 中文
                        if (String.Equals(strLanguageArea, "CN", StringComparison.CurrentCultureIgnoreCase))
                        {
                            strLanguage = "zh-Hans";
                        }
                        else if (String.Equals(strLanguageArea, "TW", StringComparison.CurrentCultureIgnoreCase))
                        {
                            strLanguage = "zh-Hant";
                        }
                        else if (String.Equals(strLanguageArea, "HK", StringComparison.CurrentCultureIgnoreCase))
                        {
                            strLanguage = "zh-Hant";
                        }
                        else
                        {
                            strLanguage = "zh-Hans";
                        }
                    }
                    else if (String.Equals(strLanguageName, "en", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // 英文
                        strLanguage = "en";
                    }
                    else if (String.Equals(strLanguageName, "ja", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // 日语
                        strLanguage = "ja";
                    }
                    else
                    {
                        // 英文
                        strLanguage = "en";
                    }
                }
                    break;

                case RuntimePlatform.IPhonePlayer:
                {
                    if (String.Equals(strLanguageName, "zh-Hans", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // 简体中文
                        strLanguage = "zh-Hans";
                    }
                    else if (String.Equals(strLanguageName, "zh-Hant", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // 繁体中文
                        strLanguage = "zh-Hant";
                    }
                    else if (String.Equals(strLanguageName, "en", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // 英文
                        strLanguage = "en";
                    }
                    else if (String.Equals(strLanguageName, "ja", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // 日语
                        strLanguage = "ja";
                    }
                    else
                    {
                        // 英文
                        strLanguage = "en";
                    }
                }
                    break;

                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                {
                    strLanguage = "zh-Hans";
                }
                    break;

                default:
                    break;
            }
        }
        else
        {
            // 英文
            strLanguage = "en";
        }

        return strLanguage;
    }

    // 当前操作系统版本是否大于等于目标版本
    public bool IsNewerSystemVersion(string strBaseVersion)
    {
        string strSystemVersion = GetSystemVersion();

        int nResult = CompareVersion(strBaseVersion, strSystemVersion);

        return nResult >= 0;
    }

    public int CompareVersion(string strVersionBase, string strVersionTarget)
    {
        try
        {
            string[] strLineBase = strVersionBase.Split('.');
            string[] strLineTarget = strVersionTarget.Split('.');

            for (int i = 0; i < strLineBase.Length || i < strLineTarget.Length; ++i)
            {
                // 自动补齐0
                int nBaseVersion = 0;
                int nTargetVersion = 0;

                if (i < strLineBase.Length)
                {
                    nBaseVersion = Utils.GetInt(strLineBase[i]);
                }

                if (i < strLineTarget.Length)
                {
                    nTargetVersion = Utils.GetInt(strLineTarget[i]);
                }

                if (nBaseVersion > nTargetVersion)
                {
                    return -1;
                }
                else if (nBaseVersion < nTargetVersion)
                {
                    return 1;
                }
            }
        }
        catch (Exception ex)
        {
            LogUtils.LogError(ex.Message);
        }

        return 0;
    }

    // 获取IGG-GameID
    public string GetGameID()
    {
        string strValue = "";
#if !UNITY_EDITOR && UNITY_ANDROID
        strValue = ThirdPartyWrapper.CallJavaWithReturn<string>("GetPlatformParam1");
#elif !UNITY_EDITOR && UNITY_IOS
        strValue = ThirdPartyWrapper.SDK_GetPlatformParam1();
#endif

        if (string.IsNullOrEmpty(strValue))
        {
            return "";
        }

        return strValue;
    }

    // 跳转到应用商店
    public void JumpToAppStore(string url, string marketPkg)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        ThirdPartyWrapper.CallJava("JumpToAppStore", url, marketPkg);
#elif !UNITY_EDITOR && UNITY_IOS
        ThirdPartyWrapper.SDK_JumpToAppStore(url);
#endif
    }

    //////////////////////////////////////////////////////////////////// 
    ///  SDK交互-关键流程
    ////////////////////////////////////////////////////////////////////
    List<string> UnPackageParam(string strParam)
    {
        List<string> lstParam = new List<string>();

        if (strParam.Length <= 0)
        {
            return lstParam;
        }

        int nPos = 0;
        int nLength = strParam.Length;

        int nParamLength = (int)Utils.GetUInt(strParam.Substring(nPos, 2));

        nPos += 2;

        for (int nIndex = 0; nIndex < nParamLength; ++nIndex)
        {
            if (nPos + 4 > nLength)
            {
                break;
            }

            int nParamLengthCur = (int)Utils.GetUInt(strParam.Substring(nPos, 4));

            nPos += 4;

            if (nPos + nParamLengthCur > nLength)
            {
                break;
            }

            string strParamCur = "";

            if (nParamLengthCur != 0)
            {
                strParamCur = strParam.Substring(nPos, nParamLengthCur);
            }

            nPos += nParamLengthCur;

            lstParam.Add(strParamCur);
        }

        return lstParam;
    }

    public void UnInitSDK()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        ThirdPartyWrapper.CallJava("UnInitSDK");
#elif !UNITY_EDITOR && UNITY_IOS
        ThirdPartyWrapper.SDK_UnInitSDK();
#endif
    }

    public void LogSDK(string strLog)
    {
        LogUtils.LogWarning(strLog);
    }
    
    public string GetUserUin()
    {
#if !UNITY_EDITOR && PF_TAPTAPSDK
        // TapSDK用户ID
        if (TapSDKManager.Instance.IsLoggedIn())
        {
            return TapSDKManager.Instance.GetUserId();
        }
#endif
        return strUIN;
    }

    public string GetUserSession()
    {
        return strSession;
    }

    public string GetPlatformOpts()
    {
        return strPlatformOpts;
    }

    /// <summary>
    /// 获取用户昵称
    /// </summary>
    public string GetUserName()
    {
#if !UNITY_EDITOR && PF_TAPTAPSDK
        // TapSDK用户昵称
        if (TapSDKManager.Instance.IsLoggedIn())
        {
            return TapSDKManager.Instance.GetUserName();
        }
#endif
        return ""; // 其他平台可能需要从其他地方获取用户名
    }

    /// <summary>
    /// 获取用户头像
    /// </summary>
    public string GetUserAvatar()
    {
#if !UNITY_EDITOR && PF_TAPTAPSDK
        // TapSDK用户头像
        if (TapSDKManager.Instance.IsLoggedIn())
        {
            return TapSDKManager.Instance.GetUserAvatar();
        }
#endif
        return ""; // 其他平台可能需要从其他地方获取用户头像
    }

    public bool CheckUserLogin()
    {
#if !UNITY_EDITOR && PF_TAPTAPSDK
        // TapSDK登录状态检查
        return TapSDKManager.Instance.IsLoggedIn();
#elif !UNITY_EDITOR && PF_WEIXIN
        // 微信渠道 code为一次性验证，不能重复使用
        return false;
#else
        if (!string.IsNullOrEmpty(strUIN) && !string.IsNullOrEmpty(strSession))
        {
            return true;
        }

        return false;
#endif
    }

    public void ClearLoginState()
    {
        strUIN = "";
        strSession = "";
        strPlatformOpts = "";
    }

    //todo 登录
    public void Login()
    {
        LogUtils.LogFormat("SDKInterface.Login");

        strUIN = "";
        strSession = "";
        strPlatformOpts = "";

        DataAnalyticsWrapper.Instance.OnLogin();

#if PF_WEIXIN
        SDKInterfaceWeiXin.Instance.OnLogin();
#elif PF_DOUYIN
        SDKInterfaceDouYin.Instance.OnLogin();
#elif !UNITY_EDITOR && PF_TAPTAPSDK
        // TapSDK登录
        TapSDKManager.Instance.OnLoginResult += OnTapSDKLoginResult;
        TapSDKManager.Instance.TapSDKLogin();
#elif !UNITY_EDITOR && UNITY_ANDROID
        ThirdPartyWrapper.CallJava("Login");
#elif !UNITY_EDITOR && UNITY_IOS
        ThirdPartyWrapper.SDK_Login();
#endif
    }

    public void OnLoginSuccess(string uid, string userName, string token, string authTime = "")
    {
        LogUtils.LogFormat("SDKInterface.OnLoginSuccess: uid:{0}, userName：{1}, token：{2}, authTime:{3} \n", uid, userName, token, authTime);

        strUIN = uid;
        strSession = token;

        if (!string.IsNullOrEmpty(authTime))
        {
            JsonObject json = new JsonObject();
            json["verifyTimestamp"] = authTime;
            strPlatformOpts = SimpleJson.SerializeObjectInHeap(json);
        }
        else
        {
            strPlatformOpts = "";
        }

        DataAnalyticsWrapper.Instance.OnLoginSuccess();

        EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_SDK_LOGIN_SUCCESS);
    }

    public void OnLoginFailed(string errMsg)
    {
        LogUtils.LogFormat("SDKInterface.OnLoginFailed: {0} \n", errMsg);

        strUIN = "";
        strSession = "";
        strPlatformOpts = "";

        DataAnalyticsWrapper.Instance.OnLoginFail(errMsg);

        EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_SDK_LOGIN_FAIL, errMsg);
    }
    
    #region TapSDK相关

    // 更新 整包 唤起
    public void SDKUpdateGame()
    {
#if !UNITY_EDITOR && PF_TAPTAPSDK
        LogUtils.LogFormat("SDKInterface.SDKUpdateGame");
        TapSDKManager.Instance.TapSDKUpdateGame();
#endif
    }
    
    /// <summary>
    /// TapSDK登录结果回调
    /// </summary>
    private void OnTapSDKLoginResult(bool success)
    {
        TapSDKManager.Instance.OnLoginResult -= OnTapSDKLoginResult; // 移除监听，避免重复调用
        
        if (success)
        {
            var user = TapSDKManager.Instance.GetCurrentUser();
            if (user != null)
            {
                OnLoginSuccess(user.openId, user.name, "", "");
            }
            else
            {
                OnLoginFailed("获取用户信息失败");
            }
        }
        else
        {
            OnLoginFailed("TapSDK登录失败");
        }
    }
    
    /// <summary>
    /// 检测充值金额是否超过账号限制
    /// </summary>
    /// <param name="money"></param>
    /// <returns></returns>
    public void CheckRecharge(float money, Action<bool> callback)
    {
#if !UNITY_EDITOR && PF_TAPTAPSDK
        LogUtils.LogFormat("SDKInterface.CheckRecharge");
        TapSDKManager.Instance.CheckCharge(money, callback);
#endif
    }
    
    /// <summary>
    /// 上报充值金额
    /// </summary>
    /// <param name="money"></param>
    /// <returns></returns>
    public void SubmitPayResult(float money)
    {
#if !UNITY_EDITOR && PF_TAPTAPSDK
        LogUtils.LogFormat("SDKInterface.SubmitPayResult");
        TapSDKManager.Instance.SubmitPayResult(money);
#endif
    }

    /// <summary>
    /// 数据分析 上报数据
    /// </summary>
    /// <param name="money"></param>
    /// <returns></returns>
    public void ReportTapSDKEventData(string eventName, string data)
    {
#if !UNITY_EDITOR && PF_TAPTAPSDK
        LogUtils.LogFormat("SDKInterface.ReportTapSDKEventData");
        TapSDKManager.Instance.ReportTapSDKEventData(eventName, data);
#endif
    }
    
    #endregion
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="roleId"></param>
    /// <param name="account"></param>
    /// <param name="createTime">0不是新创角，1新创角</param>
    public void LoginServerSuccess(string token, string roleId, string account, int createTime)
    {
        LogUtils.LogWarning("LoginServerSuccess");

#if PF_WEIXIN
        SDKInterfaceWeiXin.Instance.OnLoginLobbyServer(token, roleId, account);
#elif PF_DOUYIN
        SDKInterfaceDouYin.Instance.OnLoginLobbyServer(token, roleId, account);
#endif

        DataAnalyticsWrapper.Instance.OnLoginLobbyServer(roleId, account, createTime);
    }
    
    public void Pay(string gameOrderId, string productID, int goodsID, string goodsName, string sdkInfo, string purchaseOptions = "", bool bWait = false)
    {
        LogUtils.LogWarningFormat("SDKInterface.Pay Item({0},{1},{2})", productID, sdkInfo, purchaseOptions);

        DataAnalyticsWrapper.Instance.OnPay(productID, goodsID, gameOrderId);

#if PF_WEIXIN || PF_DOUYIN
        MiniGamePay(gameOrderId, goodsName, purchaseOptions);
        return;
#endif

        int result = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
        result = ThirdPartyWrapper.CallJavaWithReturn<int>("Pay", productID, purchaseOptions);
#elif !UNITY_EDITOR && UNITY_IOS
        result = ThirdPartyWrapper.SDK_Pay(productID, purchaseOptions);
#endif
    }

    public void OnPaySuccess(string sdkOrderId = "", string gameOrderId = "", string extraParam = "")
    {
        LogUtils.LogFormat("SDKInterface.OnPaySuccess: sdkOrderId:{0}, gameOrderId:{1}, extraParam:{2} \n", sdkOrderId, gameOrderId, extraParam);
        
        DataAnalyticsWrapper.Instance.OnPaySuccess(sdkOrderId, gameOrderId, extraParam);
    }

    public void OnPayCancel(string sdkOrderId = "", string gameOrderId = "", string extraParam = "")
    {
        LogUtils.LogFormat("SDKInterface.OnPayCancel: sdkOrderId:{0}, gameOrderId:{1}, extraParam:{2} \n", sdkOrderId, gameOrderId, extraParam);
        DataAnalyticsWrapper.Instance.OnPayCancel(sdkOrderId, gameOrderId, extraParam);
    }

    public void OnPayFailed(string sdkOrderId = "", string gameOrderId = "", string extraParam = "")
    {
        LogUtils.LogFormat("SDKInterface.OnPayFailed: sdkOrderId:{0}, gameOrderId:{1}, extraParam:{2} \n", sdkOrderId, gameOrderId, extraParam);

        DataAnalyticsWrapper.Instance.OnPayFailed(sdkOrderId, gameOrderId, extraParam);
    }

    public void Logout()
    {
        LogUtils.LogWarning("SDKInterface.Logout");

        DataAnalyticsWrapper.Instance.OnLogout();

        bool bLogoutRightNow = false;

#if !UNITY_EDITOR && UNITY_ANDROID
        bLogoutRightNow = ThirdPartyWrapper.CallJavaWithReturn<bool>("Logout");
#elif !UNITY_EDITOR && UNITY_IOS
        bLogoutRightNow = ThirdPartyWrapper.SDK_Logout() != 0;
#else
        bLogoutRightNow = true;
#endif
        
        LogUtils.LogWarning($"SDKInterface.Logout bRightNow:{bLogoutRightNow}");
        
        if (bLogoutRightNow)
        {
            LogoutSuccess("");
        }
    }

    public void LogoutSuccess(string strParam)
    {
        LogUtils.Log("SDKInterface.LogoutSuccess");

        DataAnalyticsWrapper.Instance.OnLogoutSuccess();

        ClearLoginState();

        GameManager.Instance.ReLoginGame();
    }

    /// <summary>
    /// 渠道有退出确认框使用渠道，没有使用自身
    /// </summary>
    /// <returns></returns>
    public bool IsChannelHasExitDialog()
    {
        return ThirdPartyWrapper.Instance.IsChannelHasExitDialog();
    }

    public void RequestQuitGame()
    {
        LogUtils.Log("SDKInterface.RequestQuitGame");

        if (IsChannelHasExitDialog())
        {
            // 调用渠道退出框
            ThirdPartyWrapper.Instance.QuitGame();
        }
        else
        {
        }
    }

    public void ConfirmQuitGame()
    {
        LogUtils.Log("SDKInterface.ConfirmQuitGame");

#if !UNITY_EDITOR && PF_WEIXIN
        SDKInterfaceWeiXin.Instance.ExitMiniProgram();
#elif !UNITY_EDITOR && PF_DOUYIN
        SDKInterfaceDouYin.Instance.ExitMiniProgram();
#else
        Application.Quit();
#endif
    }

    public void OnExitSuccess()
    {
    }

    // 读取DataFile(仅仅Android)
    public byte[] LoadDataFile(string strPath)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        return ThirdPartyWrapper.CallJavaWithReturn<byte[]>("LoadFile", strPath);
#else
        return null;
#endif
    }

    //获取系统的屏幕亮度值
    public int GetScreenBrightness()
    {
        float value = 0.8f;
#if !UNITY_EDITOR && UNITY_ANDROID
        value = ThirdPartyWrapper.CallJavaWithReturn<float>("GetScreenBrightness");
#elif !UNITY_EDITOR && UNITY_IOS
        // Ios
        // float value = [UIScreen mainScreen].brightness
        value = ThirdPartyWrapper.SDK_GetScreenBrightness();
#endif
        return (int)(value * 100);
    }

    //修改系统屏幕亮度(仅当前应用
    public void SetScreenBrightness(int iValue)
    {
        float value = (float)(iValue * 0.01);
#if !UNITY_EDITOR && UNITY_ANDROID
        ThirdPartyWrapper.CallJava("SetScreenBrightness", value);
#elif !UNITY_EDITOR && UNITY_IOS
        // Ios
        // [[UIScreen mainScreen] setBrightness:value];
        ThirdPartyWrapper.SDK_SetScreenBrightness(value);
#endif
    }

    public bool CheckIsLowPhone()
    {
#if PF_WEIXIN
        return SDKInterfaceWeiXin.Instance.CheckIsLowPhone();
#elif PF_DOUYIN
        return SDKInterfaceDouYin.Instance.CheckIsLowPhone();
#else
        return false;
#endif
    }

    public bool OpenUrlScheme(string strScheme)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        return ThirdPartyWrapper.CallJavaWithReturn<bool>("OpenUrlScheme", strScheme);
#elif !UNITY_EDITOR && UNITY_IOS
        return ThirdPartyWrapper.SDK_OpenUrlScheme(strScheme) != 0;
#else
        return false;
#endif
    }
    
    public bool IsMinigamePlatform(string platform)
    {
#if PF_WEIXIN
        return SDKInterfaceWeiXin.Instance.IsMinigamePlatform(platform);
#elif PF_DOUYIN
        return SDKInterfaceDouYin.Instance.IsMinigamePlatform(platform);
#else
        return false;
#endif
    }

    public string GetSystemLanguageWithMinigame()
    {
#if PF_WEIXIN
        return SDKInterfaceWeiXin.Instance.GetSystemLanguageWithMinigame();
#elif PF_DOUYIN
        return SDKInterfaceDouYin.Instance.GetSystemLanguageWithMinigame();
#else
        return "";
#endif
    }

    public int GetSystemLanguageIndex()
    {
        int index = 0;

#if UNITY_WEBGL && !UNITY_EDITOR && PF_WEIXIN
        switch (SDKInterface.Instance.GetSystemLanguageWithMinigame())
        {
            case "ko":
                index = defLanguage.Korean;
                break;
            case "ja":
                index = defLanguage.Japanese;
                break;
            case "zh_CN":
            case "zh":
                index = defLanguage.ChineseSimplified;
                break;
            case "zh_HK":
            case "zh_TW":
                index = defLanguage.ChineseTraditional;
                break;
            case "de":
                index = defLanguage.German;
                break;
            case "fr":
                index = defLanguage.French;
                break;
            case "es":
                index = defLanguage.Spanish;
                break;
            case "pt":
                index = defLanguage.Portuguese;
                break;
            case "ru":
                index = defLanguage.Russian;
                break;
            case "id":
                index = defLanguage.Indonesian;
                break;
            case "it":
                index = defLanguage.Italian;
                break;
            case "th":
                index = defLanguage.Thai;
                break;
            case "vi":
                index = defLanguage.Vietnamese;
                break;
            case "tr":
                index = defLanguage.Turkish;
                break;
            case "ar":
                index = defLanguage.Arabic;
                break;

            default:
                index = defLanguage.English;
                break;
        }
#else
        switch (Application.systemLanguage)
        {
            default:
                index = defLanguage.English;
                break;
            case SystemLanguage.Korean:
                index = defLanguage.Korean;
                break;
            case SystemLanguage.Japanese:
                index = defLanguage.Japanese;
                break;
            case SystemLanguage.ChineseSimplified:
                index = defLanguage.ChineseSimplified;
                break;
            case SystemLanguage.ChineseTraditional:
                index = defLanguage.ChineseTraditional;
                break;
            case SystemLanguage.Dutch:
                index = defLanguage.German;
                break;
            case SystemLanguage.French:
                index = defLanguage.French;
                break;
            case SystemLanguage.Spanish:
                index = defLanguage.Spanish;
                break;
            case SystemLanguage.Portuguese:
                index = defLanguage.Portuguese;
                break;
            case SystemLanguage.Russian:
                index = defLanguage.Russian;
                break;
            case SystemLanguage.Indonesian:
                index = defLanguage.Indonesian;
                break;
            case SystemLanguage.Italian:
                index = defLanguage.Italian;
                break;
            case SystemLanguage.Thai:
                index = defLanguage.Thai;
                break;
            case SystemLanguage.Vietnamese:
                index = defLanguage.Vietnamese;
                break;
            case SystemLanguage.Turkish:
                index = defLanguage.Turkish;
                break;
            case SystemLanguage.Arabic:
                index = defLanguage.Arabic;
                break;
        }
#endif

        return index;
    }

    /// <summary>
    /// 尝试GC
    /// </summary>
    public void TriggerGCBySDK()
    {
#if !UNITY_EDITOR && PF_WEIXIN
        WeChatWASM.WX.TriggerGC();
#else
        GC.Collect();
#endif
    }

    public void GetSystemInfoByTDLogin(Action<string, string> action)
    {
#if PF_WEIXIN && !UNITY_EDITOR
       SDKInterfaceWeiXin.Instance.GetSystemInfoByTDLogin(action);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetupShareParam()
    {
#if !UNITY_EDITOR && PF_WEIXIN
        SDKInterfaceWeiXin.Instance.SetupShareParam();
#elif !UNITY_EDITOR && PF_DOUYIN
        SDKInterfaceDouYin.Instance.SetupShareParam();
#endif
    }
    
    /// <summary>
    /// 分享（支持微信小游戏平台）
    /// </summary>
    public void ShareApp(string imageUrl = "", string imageUrlId = "", Action<bool, string> callback = null)
    {
#if PF_WEIXIN && !UNITY_EDITOR
        SDKInterfaceWeiXin.Instance.ShareApp(imageUrl, imageUrlId);
        TimerManagerEx.Instance.SetTimer(0.1f, () => { callback?.Invoke(true, ""); });
#elif !UNITY_EDITOR && PF_DOUYIN
        SDKInterfaceDouYin.Instance.ShareApp(imageUrl, imageUrlId, callback);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="px"></param>
    /// <param name="py"></param>
    /// <param name="pwidth"></param>
    /// <param name="pheight"></param>
    public void ScreenshotToTempFilePath(float px, float py, float pwidth, float pheight, bool save)
    {
#if PF_WEIXIN && !UNITY_EDITOR
        SDKInterfaceWeiXin.Instance.ScreenshotToTempFilePath(px, py, pwidth, pheight, save);
#endif
    }

    /// <summary>
    /// 上报场景启动完成
    /// </summary>
    public void ReportScene(int sceneId)
    {
#if PF_WEIXIN && !UNITY_EDITOR
        SDKInterfaceWeiXin.Instance.ReportScene(sceneId);
#endif
    }

    /// <summary>
    /// 创建微信游戏圈按钮
    /// </summary>
    public void ShowGameClubButton(Vector2 screenPos, bool bChanged)
    {
#if PF_WEIXIN && !UNITY_EDITOR
        SDKInterfaceWeiXin.Instance.ShowGameClubButton(screenPos, bChanged);
#endif
    }

    /// <summary>
    /// 隐藏游戏圈
    /// </summary>
    public void HideGameClubButton()
    {
#if PF_WEIXIN && !UNITY_EDITOR
        SDKInterfaceWeiXin.Instance.HideGameClubButton();
#endif
    }

    public void AddTouchEventHandler(int touchEvent, params object[] args)
    {
#if PF_WEIXIN && !UNITY_EDITOR
        SDKInterfaceWeiXin.Instance.AddTouchEventHandler(touchEvent, args);
#endif
    }

    /// <summary>
    /// 添加微信触摸事件逻辑
    /// </summary>
    /// <param name="touchEvent"></param>
    public void RemoveTouchEventHandler(int touchEvent)
    {
#if PF_WEIXIN && !UNITY_EDITOR
        SDKInterfaceWeiXin.Instance.RemoveTouchEventHandler(touchEvent);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public void NavigateToMiniProgram(string id)
    {
#if PF_WEIXIN && !UNITY_EDITOR
        SDKInterfaceWeiXin.Instance.NavigateToMiniProgram(id);
#endif
    }

    /// <summary>
    /// 振动
    /// </summary>
    public void Vibrate()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        Handheld.Vibrate();
#elif !UNITY_EDITOR && PF_WEIXIN
        SDKInterfaceWeiXin.Instance.VibrateLong();
#elif !UNITY_EDITOR && PF_DOUYIN
        SDKInterfaceDouYin.Instance.VibrateLong();
#endif
    }

    /// <summary>
    /// 设置剪贴板
    /// </summary>
    public void SetClipboardData(string content)
    {
#if !UNITY_EDITOR && PF_WEIXIN
        SDKInterfaceWeiXin.Instance.SetClipboardData(content);
#elif !UNITY_EDITOR && PF_DOUYIN
        SDKInterfaceDouYin.Instance.SetClipboardData(content);
#else
        GUIUtility.systemCopyBuffer = content;
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    public void RestartMiniProgram()
    {
#if !UNITY_EDITOR && PF_WEIXIN
        SDKInterfaceWeiXin.Instance.RestartMiniProgram();
#elif !UNITY_EDITOR && PF_DOUYIN
        SDKInterfaceDouYin.Instance.RestartMiniProgram();
#endif
    }

    /// <summary>
    /// 对应平台系统禁用
    /// </summary>
    /// <returns></returns>
    public bool IsPlatformDisabled()
    {
#if !UNITY_EDITOR && PF_WEIXIN
        return true;
#else
        return false;
#endif
    }

    /// <summary>
    /// 订阅相关回调
    /// </summary>
    /// <param name="callback">(Action<success, errMsg, errCode>)</param>
    public void InitSubscriptionsSettingHooks(Action<bool, string, double> callback)
    {
#if !UNITY_EDITOR && PF_WEIXIN
        SDKInterfaceWeiXin.Instance.InitSubscriptionsSettingHooks(callback);
#endif
    }

    /// <summary>
    /// 获取订阅相关信息
    /// </summary>
    /// <param name="callback"></param>
    public void GetSubscriptionsSetting(Action<bool, string, Dictionary<string, string>, bool> callback)
    {
#if !UNITY_EDITOR && PF_WEIXIN
        SDKInterfaceWeiXin.Instance.GetSubscriptionsSetting(callback);
#else
        callback?.Invoke(false, "", null, false);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="title"></param>
    /// <param name="onFinished"></param>
    /// <param name="onError"></param>
    public void OpenURL(string url, string title = "", Action onFinished = null, Action onError = null)
    {
        LogUtils.LogWarning($"OpenURL: url:{url}");

        GameManager.Instance.OpenLinkURL(url, onFinished);
    }

    /// <summary>
    /// 打开隐私协议
    /// </summary>
    /// <returns></returns>
    public void OpenPrivacyContract()
    {
#if PF_WEIXIN && !UNITY_EDITOR
        SDKInterfaceWeiXin.Instance.OpenPrivacyContract();
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    public void OpenSettings()
    {
#if !UNITY_EDITOR && PF_WEIXIN
        SDKInterfaceWeiXin.Instance.OpenSettings();
#elif !UNITY_EDITOR && PF_DOUYIN
        SDKInterfaceDouYin.Instance.OpenSettings();
#else
        NativeGallery.OpenSettings();
#endif
    }

    #region ExceptionHandler

    private bool bInitExceptionHandler = false;

    public void EnableExceptionHandler()
    {
        if (IsNormalPlatform())
        {
            return;
        }

        if (bInitExceptionHandler)
        {
            return;
        }

        try
        {
            Application.logMessageReceived += OnLogCallbackHandlerWithSDK;
            AppDomain.CurrentDomain.UnhandledException += OnUncaughtExceptionHandlerWithSDK;

            bInitExceptionHandler = true;
            lstExceptionLogLast.Clear();
        }
        catch
        {
        }
    }

    public void DisableExceptionHandler()
    {
        if (!bInitExceptionHandler)
        {
            return;
        }

        try
        {
            Application.logMessageReceived -= OnLogCallbackHandlerWithSDK;
            AppDomain.CurrentDomain.UnhandledException -= OnUncaughtExceptionHandlerWithSDK;

            bInitExceptionHandler = false;
            lstExceptionLogLast.Clear();
        }
        catch
        {
        }
    }

    private void OnLogCallbackHandlerWithSDK(string condition, string stackTrace, LogType type)
    {
        if (!bInitExceptionHandler)
        {
            return;
        }

        if (string.IsNullOrEmpty(condition))
        {
            return;
        }

        if (stackTrace == null)
        {
            stackTrace = "";
        }

        if (type == LogType.Exception)
        {
            string conditionNew;
            var firstLineEnd = stackTrace.IndexOf('\n');
            if (firstLineEnd != -1)
            {
                var firstLine = stackTrace.Substring(0, firstLineEnd);
                conditionNew = string.Format("CriticalCatchException\t{0}\t{1}", condition, firstLine);
            }
            else
            {
                conditionNew = string.Format("CriticalCatchException\t{0}", condition);
            }

            ExceptionLog(conditionNew, stackTrace, false);
        }
        else if (type == LogType.Error)
        {
            ExceptionLog(condition, stackTrace, true);
        }
    }

    private void OnUncaughtExceptionHandlerWithSDK(object sender, UnhandledExceptionEventArgs args)
    {
        if (!bInitExceptionHandler)
        {
            return;
        }

        if (args == null || args.ExceptionObject == null)
        {
            return;
        }

        try
        {
            if (args.ExceptionObject.GetType() != typeof(Exception))
            {
                return;
            }

            Exception e = (Exception)args.ExceptionObject;

            if (e == null || e.GetType() == null)
            {
                return;
            }

            string name = e.GetType().Name;

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            string reason = "";

            if (!string.IsNullOrEmpty(e.Message))
            {
                reason = e.Message;
            }

            StringBuilder stackTraceBuilder = new StringBuilder("");

            StackTrace stackTrace = new StackTrace(e, true);
            int count = stackTrace.FrameCount;
            for (int i = 0; i < count; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);

                stackTraceBuilder.AppendFormat("{0}.{1}", frame.GetMethod().DeclaringType.Name, frame.GetMethod().Name);

                MethodBase method = frame.GetMethod();

                if (method == null)
                {
                    stackTraceBuilder.AppendLine();
                    continue;
                }

                ParameterInfo[] parameters = method.GetParameters();

                if (parameters == null || parameters.Length == 0)
                {
                    stackTraceBuilder.Append("()");
                }
                else
                {
                    stackTraceBuilder.Append("(");

                    int pcount = parameters.Length;

                    ParameterInfo param = null;
                    for (int p = 0; p < pcount; p++)
                    {
                        param = parameters[p];
                        stackTraceBuilder.AppendFormat("{0} {1}", param.ParameterType.Name, param.Name);

                        if (p != pcount - 1)
                        {
                            stackTraceBuilder.Append(", ");
                        }
                    }

                    param = null;

                    stackTraceBuilder.Append(")");
                }

                string fileName = frame.GetFileName();

                if (!string.IsNullOrEmpty(fileName) && !fileName.ToLower().Equals("unknown"))
                {
                    fileName = fileName.Replace("\\", "/");

                    int loc = fileName.ToLower().IndexOf("/assets/");
                    if (loc < 0)
                    {
                        loc = fileName.ToLower().IndexOf("assets/");
                    }

                    if (loc > 0)
                    {
                        fileName = fileName.Substring(loc);
                    }

                    stackTraceBuilder.AppendFormat("(at {0}:{1})", fileName, frame.GetFileLineNumber());
                }

                stackTraceBuilder.AppendLine();
            }

            var conditionNew = string.Format("CriticalUncatchException\t{0}", name);
            ExceptionLog(conditionNew, string.Format("{0}\n{1}", reason, stackTraceBuilder.ToString()), false);
        }
        catch
        {
        }
    }

    public void ExceptionLog(string condition, string info, bool checkCondition)
    {
        if (checkCondition && !condition.StartsWith("Critical"))
        {
            return;
        }
        
        // 最近3条上报过，忽视
        if (lstExceptionLogLast.Contains(condition))
        {
            return;
        }

        lstExceptionLogLast.Add(condition);

        if (lstExceptionLogLast.Count >= 5)
        {
            lstExceptionLogLast.RemoveAt(0);
        }

#if !UNITY_EDITOR && UNITY_ANDROID
        ThirdPartyWrapper.CallJava("ExceptionLog", condition, info);
#elif !UNITY_EDITOR && UNITY_IOS
        ThirdPartyWrapper.SDK_ExceptionLog(condition, info);
#endif
    }

    public void EventLog(string strEvent, string iggid, string playerLv, string eMoney, string money, string createTime,
        string isWifi)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        ThirdPartyWrapper.CallJava("EventLog", strEvent, iggid, playerLv, eMoney, money, createTime, isWifi);
#elif !UNITY_EDITOR && UNITY_IOS
        ThirdPartyWrapper.SDK_EventLog(strEvent, iggid, playerLv, eMoney, money, createTime, isWifi);
#endif
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetAndroidID()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var androidId = ThirdPartyWrapper.CallJavaWithReturn<string>("GetAndroidId");
        return androidId;//string.IsNullOrEmpty(androidId) ? Utils.GetDeviceID() : androidId;
#else
        return "Unknown";
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetImei()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var imei = ThirdPartyWrapper.CallJavaWithReturn<string>("GetImei");
        return imei;//string.IsNullOrEmpty(imei) ? Utils.GetDeviceID() : imei;
#else
        return "Unknown";
#endif
    }
    
    /// <summary>
    /// 分享图片
    /// </summary>
    /// <param name="texture">texture（必要参数，微信平台无效）</param>
    /// <param name="callback">非必要参数</param>
    /// <param name="shotType">画布截图类型（仅微信平台有意义）</param>
    public void ShareImageToAppMessage(Texture2D texture, Action<bool, string> callback = null, int shotType = 0)
    {
        LogUtils.LogWarning($"ShareImageToAppMessage shotType：{shotType}");

#if !UNITY_EDITOR && (PF_WEIXIN || PF_DOUYIN)
        if (!CheckPlatformIgnoreByType((defImageWayType)shotType))
        {
            LogUtils.LogWarning("ShareImageToAppMessage Current platform not supported");
        
            callback?.Invoke(false, "Current platform not supported");
            return;
        }
#endif
        
#if !UNITY_EDITOR && PF_WEIXIN
        ShareImageToAppMessage(text:"", smallIcon:"", linkUrl:"", imageUrl:"", imageUrlId:"", callback, shotType);
#elif !UNITY_EDITOR && PF_DOUYIN
        ShareImageToAppMessage(text:"", smallIcon:"", linkUrl:"", imageUrl:"", imageUrlId:"", callback, shotType);
#else
#endif
    }

    /// <summary>
    /// 分享图片
    /// </summary>
    /// <param name="text"></param>
    /// <param name="smallIcon"></param>
    /// <param name="linkUrl"></param>
    /// <param name="imageUrl">图片地址url（微信平台可以带审核的url）</param>
    /// <param name="imageUrlId">图片地址id（仅微信平台有意义）</param>
    /// <param name="callback">非必要参数</param>
    /// <param name="shotType">画布截图类型（仅微信平台有意义，前提是imageUrl空的情况）</param>
    public void ShareImageToAppMessage(string text, string smallIcon, string linkUrl, string imageUrl, string imageUrlId = "", Action<bool, string> callback = null, int shotType = 0)
    {
        LogUtils.LogWarning($"ShareImageToAppMessage imageUrl：{imageUrl}, shotType：{shotType}");
        
        //内部渠道过滤分享，不包含系统分享
        if (IsNormalPlatform())
        {
            LogUtils.LogWarning("ShareImageToAppMessage Current platform not supported");

            callback?.Invoke(false, "Current platform not supported");
            return;
        }

#if !UNITY_EDITOR && UNITY_ANDROID
#elif !UNITY_EDITOR && UNITY_IOS
#elif !UNITY_EDITOR && PF_DOUYIN
        ShareApp("", "", callback);//抖音暂时走这里
#else
        if (!string.IsNullOrEmpty(imageUrl) && !string.IsNullOrEmpty(imageUrlId))
        {
            ShareApp(imageUrl, imageUrlId, callback);
        }
        else
        {
            //pc截图黑屏，过滤
            if (IsMinigamePlatform(MinigamePlatform.windows) 
                || IsMinigamePlatform(MinigamePlatform.devtools))
            {
                ShareApp("", "", callback);
            }
            else
            {
                //分享截图（仅支持微信小游戏平台）
#if PF_WEIXIN && !UNITY_EDITOR
                SDKInterfaceWeiXin.Instance.ShareImageToAppMessage(shotType, callback);
//#elif PF_DOUYIN && !UNITY_EDITOR
                //SDKInterfaceDouYin.Instance.ShareImageToAppMessage(shotType, callback);
#endif
            }
        }
#endif
    }

    /// <summary>
    /// 保存图片到相册
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="filename">图片名（微信平台无效）</param>
    /// <param name="callback"></param>
    /// <param name="type">画布截图类型（仅支持微信平台）</param>
    public void SaveImageToGallery(Texture2D texture, string filename = "catsoup.jpg", Action<bool, string> callback = null, int type = 0)
    {
        LogUtils.LogWarning($"SaveImageToGallery filename：{filename}");

#if !UNITY_EDITOR && (PF_WEIXIN || PF_DOUYIN)
        //pc截图黑屏，过滤
        if (!CheckPlatformIgnoreByType((defImageWayType)type)
            || IsMinigamePlatform(MinigamePlatform.windows)
            || IsMinigamePlatform(MinigamePlatform.devtools))
        {
            LogUtils.LogWarning("SaveImageToGallery Current platform not supported");

            callback?.Invoke(false, "Current platform not supported");
            return;
        }
#endif

#if PF_WEIXIN && !UNITY_EDITOR
        SDKInterfaceWeiXin.Instance.SaveImageToGallery(type, callback);
#elif PF_DOUYIN && !UNITY_EDITOR
        SDKInterfaceDouYin.Instance.SaveImageToGallery(type, callback);
#else
        CheckPermissionAuthorize((res, errorMsg) =>
        {
            if (res)
            {
                LogUtils.LogWarning("NativeGallery.SaveImageToGallery");
                NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(texture, Application.productName, filename, (success, path) =>
                {
                    callback?.Invoke(success, "");
                });
                if (permission != NativeGallery.Permission.Granted)
                {
                    callback?.Invoke(false, permission.ToString());
                }
                LogUtils.LogWarning($"NativeGallery.SaveImageToGallery Permission.{permission}");
            }
            else
            {
                callback?.Invoke(false, errorMsg);
            }
        });
#endif
    }

    private void CheckPermissionAuthorize(Action<bool, string> nextAction)
    {
        NativeGallery.Permission curPermission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);
        if (curPermission == NativeGallery.Permission.ShouldAsk)
        {
        }
        else if (curPermission == NativeGallery.Permission.Granted)
        {
            nextAction?.Invoke(true, "");
        }
        else
        {
            nextAction.Invoke(false, curPermission.ToString());
        }
        LogUtils.LogWarning($"CheckPermissionAuthorize Permission.{curPermission}");
    }

    /// <summary>
    /// 保存图片到应用本地(不支持微信）
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="filename"></param>
    /// <param name="callback"></param>
    /// <param name="compression"></param>
    public void SaveImageToAppData(Texture2D texture, string filename = "catsoup.jpg", Action<bool, string> callback = null, bool compression = false)
    {
#if !UNITY_EDITOR && PF_WEIXIN
        LogUtils.LogWarning("SaveImageToAppData Current platform not supported");
        callback?.Invoke(false, "Current platform not supported");
#elif !UNITY_EDITOR && PF_DOUYIN
        LogUtils.LogWarning("SaveImageToAppData Current platform not supported");
        callback?.Invoke(false, "Current platform not supported");
#else
        GameManager.Instance.StartCoroutine(SaveImageAsync(texture, filename, callback, compression));
#endif
    }
    
    private IEnumerator SaveImageAsync(Texture2D texture, string filename, Action<bool, string> callback = null, bool compression = false)
    {
        if (string.IsNullOrEmpty(Utils.TemporaryImagePath))
        {
            callback?.Invoke(false, "");
            yield break;
        }
        
        var filePath = Path.Combine(Utils.TemporaryImagePath, filename );
        filePath = filePath.NormalizePath(true);
        
        bool isJpeg = filename.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase);
        byte[] bytes = isJpeg ? texture.EncodeToJPG(compression ? 70 : 100) : texture.EncodeToPNG();
        
        var saveTask = File.WriteAllBytesAsync(filePath, bytes);
        
        while (!saveTask.IsCompleted)
        {
            yield return null;
        }
        
        callback?.Invoke(true, filePath);
    }

    /// <summary>
    /// 微信支付
    /// </summary>
    /// <param name="orderID"></param>
    /// <param name="goodsName"></param>
    /// <param name="purchaseOptions"></param>
    public void MiniGamePay(string orderID, string goodsName, string purchaseOptions)
    {
#if !UNITY_EDITOR && PF_WEIXIN
        if (IsMinigamePlatform(MinigamePlatform.ios))
        {
            // 通过客服页面发起支付
            SDKInterfaceWeiXin.Instance.OpenCustomerServiceConversation(goodsName, $"/pages/index/index?orderId={orderID}", purchaseOptions, "https://cs.seayooassets.com/Res/WebGLWX/Common/wxiospay.png", true);
        }
        else if (IsMinigamePlatform(MinigamePlatform.mac))
        {
            //CommonPopupManager.Instance.StartPopup(CommonPopupManager.ePopupType.CONFIRM, Localization.Get("msg_macOsWeixinPay"), null, null, null, "okay");
        }
        else
        {
            SDKInterfaceWeiXin.Instance.MidasPay(orderID);
        }
#elif !UNITY_EDITOR && PF_DOUYIN
        if (IsMinigamePlatform(MinigamePlatform.ios))
        {
            // 通过客服页面发起支付
            SDKInterfaceDouYin.Instance.OpenAwemeCustomerService(orderID);
        }
        else if (IsMinigamePlatform(MinigamePlatform.mac))
        {
            //CommonPopupManager.Instance.StartPopup(CommonPopupManager.ePopupType.CONFIRM, Localization.Get("msg_macOsWeixinPay"), null, null, null, "okay");
        }
        else
        {
            SDKInterfaceDouYin.Instance.MidasPay(orderID);
        }
#endif
    }

    /// <summary>
    /// 打开客服聊天
    /// </summary>
    /// <param name="title">气泡消息标题</param>
    /// <param name="path">气泡消息小程序路径</param>
    /// <param name="cardImg">气泡消息图片</param>
    /// <param name="isCard">是否发送小程序气泡消息</param>
    /// <param name="callback">是否成功，回调结果信息</param>
    public void OpenCustomerServiceChat(string path = "", string title = "", string cardImg = "", bool isCard = false, Action<bool, string> callback = null)
    {
#if !UNITY_EDITOR && PF_WEIXIN
        SDKInterfaceWeiXin.Instance.OpenCustomerServiceChat(title, path, cardImg, isCard, callback);
#elif !UNITY_EDITOR && PF_DOUYIN
        SDKInterfaceDouYin.Instance.OpenCustomerServiceChat(title, path, cardImg, isCard, callback);
#else
        OpenURL(path, title, () => { callback?.Invoke(true, ""); }, () =>
        {
            callback?.Invoke(false, "");
        });
#endif
    }
    
    /// <summary>
    /// 微信设备像素比
    /// </summary>
    /// <returns></returns>
    public double GetPixelRatio()
    {
#if !UNITY_EDITOR && PF_WEIXIN
        return SDKInterfaceWeiXin.Instance.GetPixelRatio();
#elif !UNITY_EDITOR && PF_DOUYIN
        return SDKInterfaceDouYin.Instance.GetPixelRatio();
#endif
        return 1;
    }
    
    /// <summary>
    /// 创建快捷方式
    /// </summary>
    /// <param name="callback"></param>
    public void CreateShortcut(Action<bool> callback)
    {
#if !UNITY_EDITOR && PF_WEIXIN
        //SDKInterfaceWeiXin.Instance.CreateShortcut(callback);
#elif !UNITY_EDITOR && PF_DOUYIN
        SDKInterfaceDouYin.Instance.CreateShortcut(callback);
#endif
    }
    
    /// <summary>
    /// 游戏中途从侧边栏中复访
    /// </summary>
    /// <returns></returns>
    public string GetLaunchFrom()
    {
#if !UNITY_EDITOR && PF_DOUYIN
        return SDKInterfaceDouYin.Instance.GetLaunchFrom();
#else
        return "";
#endif
    }
   
    /// <summary>
    /// 游戏中途从侧边栏中复访
    /// </summary>
    /// <returns></returns>
    public string GetLocation()
    {
#if !UNITY_EDITOR && PF_DOUYIN
        return SDKInterfaceDouYin.Instance.GetLocation();
#else
        return "";
#endif
    }
    
    /// <summary>
    /// 侧边栏启动
    /// </summary>
    /// <returns></returns>
    public bool IsIntoSideBar()
    {
#if !UNITY_EDITOR && PF_DOUYIN
        return SDKInterfaceDouYin.Instance.IsIntoSideBar();
#else
        return false;
#endif
    }

    /// <summary>
    /// 确认当前宿主版本是否支持跳转某个小游戏入口场景。
    /// - 目前仅支持「侧边栏」场景
    /// </summary>
    /// <param name="callback"></param>
    public void CheckScene(Action<bool, string> callback)
    {
#if !UNITY_EDITOR && PF_WEIXIN
        callback?.Invoke(false, "");
#elif !UNITY_EDITOR && PF_DOUYIN
        SDKInterfaceDouYin.Instance.CheckScene(callback);
#else
        callback?.Invoke(false, "");
#endif
    }
    
    /// <summary>
    /// 调用该API可以跳转到某个小游戏入口场景。
    /// - 目前仅支持跳转「侧边栏」场景。
    /// </summary>
    /// <param name="callback"></param>
    public void NavigateToScene(Action<bool, string> callback)
    {
#if !UNITY_EDITOR && PF_WEIXIN

#elif !UNITY_EDITOR && PF_DOUYIN
        SDKInterfaceDouYin.Instance.NavigateToScene(callback);
#endif
    }
    
    /// <summary>
    /// 分享图片到游戏圈
    /// </summary>
    /// <param name="url"></param>
    /// <param name="title">分享标题</param>
    /// <param name="content">分享文案</param>
    /// <param name="callback"></param>
    /// <param name="shotType"></param>
    public void ShareImageToGameCenter(string url = "", string title = "", string content = "", Action<bool, int> callback = null, int shotType = 0)
    {
        LogUtils.LogWarning($"ShareImageToGameCenter shotType：{shotType}");

#if !UNITY_EDITOR && PF_WEIXIN
        if (IsNormalPlatform() 
            || !CheckPlatformIgnoreByType((defImageWayType)shotType) 
            || IsMinigamePlatform(MinigamePlatform.windows) 
            || IsMinigamePlatform(MinigamePlatform.devtools))//pc截图黑屏，过滤
        {
            LogUtils.LogWarning("ShareImageToGameCenter Current platform not supported");

            callback?.Invoke(false, -1);
            return;
        }

        if (!string.IsNullOrEmpty(url))
        {
            SDKInterfaceWeiXin.Instance.ShareImageToGameCenter(url, title, content, callback);
        }
        else
        {
            SDKInterfaceWeiXin.Instance.ShareCanvasToGameCenter(shotType, title, content, callback);
        }
#endif
    }
}