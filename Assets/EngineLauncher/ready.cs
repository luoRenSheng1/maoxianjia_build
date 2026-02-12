#if !UNITY_EDITOR && USE_HYBRIDCLR
#define LOAD_HYBRIDCLR
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Engine;
using EngineLauncher;
using FairyGUI;
using ThirdParty;
using ThirdParty.Wrapper;
using UnityEngine;
using UnityEngine.Networking;
using YooAsset;

#if LOAD_HYBRIDCLR
using HybridCLR;
using System.Runtime.InteropServices;
using System.Text;
#endif

#if PF_WEIXIN
using WeChatWASM;
#endif

#if PF_DOUYIN
using StarkSDKSpace;
#endif

#if !UNITY_EDITOR && UNITY_IOS
using System.Runtime.InteropServices;
#endif

public class ready : MonoBehaviour
{
    public class MinigamePlatform
    {
        public static string ios = "ios"; //ios': iOS微信（包含 iPhone、iPad）;
        public static string android = "android"; //'android': Android微信;
        public static string windows = "windows"; //'windows': Windows微信;
        public static string mac = "mac"; //'mac': macOS微信;
        public static string devtools = "devtools"; //"devtools"
    }

    private enum EN_LOAD_TYPE
    {
        INIT,
        LOADING,
        LOADED,
        LOADERROR,
        LOADWAITCONTINUE,
        TERMINATION
    }
    
    public static class defLanguage
    {
        public const int English = 0;    //英语
        public const int Korean = 1;     //韩语
        public const int Japanese = 2;   //日语
        public const int ChineseSimplified = 3;  // 简体中文
        public const int ChineseTraditional = 4; // 繁体中文
        public const int German = 5;
        public const int French = 6;
        public const int Spanish = 7;
        public const int Portuguese = 8;
        public const int Russian = 9;
        public const int Indonesian = 10;
        public const int Italian = 11;
        public const int Thai = 12;
        public const int Vietnamese = 13;
        public const int Turkish = 14;
        public const int MAX = 15;

        public const int Brazil = 100; // @note: 정식으로 사용하려면 Max 안의 값으로 넣어야 한다
        public const int Arabic = 101;
        public const int Spanish_Latam = 102;
        public const int Polish = 103;
    }

    private class RetryHttpGetUnit
    {
        public string url = "";
        public string retryTip = "";
        public EN_LOAD_TYPE loadType = EN_LOAD_TYPE.INIT;
        public byte[] bys = null;
    }

    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    private class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }

        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }

        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }

    private const string CATSOUPACCOUNTID = "catsoupaccountid";

    // 热更新不支持重载程序集
    public static int assemblySize = 0;
    public static string assemblyMD5 = "";
    private Dictionary<string, string> dictManifest = new Dictionary<string, string>();

    private const string Language_Index = "languageIndex";

    private const string KEY_APP = "App";
    private const string KEY_YES = "Yes";
    private const string KEY_QUIT = "Quit";
    private const string KEY_NO = "No";
    private const string KEY_TIP = "Tip";
    private const string KEY_ORIGINTIP = "OriginTip";
    private const string KEY_ORIGINUPDATE = "OriginUpdate";
    private const string KEY_REFRESH = "Refresh";
    private const string KEY_QUITGAME = "QuitGame";
    private const string KEY_LOADCONFFAIL = "LoadConfFail";
    private const string KEY_LOADRESFAIL = "LoadResFail";
    private const string KEY_MAINTAIN_TIMEDAY = "MaintainTimeDay";
    private const string KEY_MAINTAIN_TIMEHOUR = "MaintainTimeHour";
    private const string KEY_MAINTAIN_TIMEMINUTE = "MaintainTimeMinute";
    private const string KEY_MAINTAIN_TIMESECOND = "MaintainTimeSecond";
    private const string KEY_BIGVERSIONUPDATE = "BigVersionUpdate";
    private const string KEY_BIGVERSIONUPDATECOMMON = "BigVersionUpdateCommon";
    private const string KEY_BIGVERSIONUPDATEWX = "BigVersionUpdateWX";
    private const string KEY_UPDATERESFAIL = "UpdateResFail";
    private const string KEY_UPDATERESSIZETIP = "UpdateResSizeTip";
    private const string KEY_UPDATERESCOMPRESS = "UpdateResCompress";
    private const string KEY_UPDATERESCOMPRESSFAIL = "UpdateResCompressFail";
    private const string KEY_UPDATERESENGINEGAMECHANGE = "UpdateResEngineGameChange";
    private const string KEY_UPDATERESDOWNSYNC = "UpdateResDownSync";
    private const string KEY_AGREE = "Agree";
    private const string KEY_REFUSE = "Refuse";
    private const string KEY_PLATFORM_TITLE = "PlatformTitle";
    private const string KEY_PLATFORM_DESC = "PlatformContent";

    private const string STR_CONF_URL = "https://source.cs.seayoo.com/clientCfg/{0}";
    private const string STR_WEBGLCDN_URL = "https://cs.seayooassets.com/Res/WebGLDY";
    private const string STR_DEFAULT_PACKAGE_NAME = "DefaultPackage";

    private string m_defaultHostServer => GetHostServerURL(); //CDN地址
    private string m_fallbackHostServer => GetHostServerURL(); //CDN备用地址

    public const int PLATFORM_CODE_GENERAL = 100; // 通用平台-内服
    public const int PLATFORM_CODE_WEIXIN = 500; // 微信小游戏
    public const int PLATFORM_CODE_DOUYIN = 600; // 抖音小游戏
    public const int PLATFORM_TAPTAP_SDK = 900; // TapSDK

    private const int WEB_GET_TIMEOUT = 10;

    // 渠道数据
    private int nPlatformCode = 0;

    // 需要远端获取
    private string m_strLimitAppVersion = "";
    private Dictionary<string, int> m_mapLimitResVersion = new Dictionary<string, int>();
    private string m_strReviewAppVersion = ""; // 提审版本号
    private string m_strAppStoreURL = ""; // 商店URL
    private string m_strNewsURL = ""; // 公告数据URL
    private string m_strChannelSwitchURL = ""; // 开关数据URL
    private string m_strServerInfo = "";
    private List<string> m_lstWhiteListIP = new List<string>();
    private List<string> m_lstWhiteListAccount = new List<string>();
    private string m_ip = "";

    public string strUpdateURL { get; private set; } = "";
    public string strConfURL { get; private set; } = "";

    private DateTime dateTimeServerRecv { get; set; } = DateTime.Now;
    private DateTime dateTimeServerLocal { get; set; } = DateTime.Now;

    private bool IsMaintain { get; set; } = false;
    private string MaintainContent { get; set; } = "";
    private DateTime MaintainStart { get; set; } = DateTime.Now;
    private DateTime MaintainEnd { get; set; } = DateTime.Now;
    private CBaseTimer timerMaintain { get; set; } = null;
    private bool IsConfirmMaintain { get; set; } = false;

    private bool IsLoginBox { get; set; } = false;
    private string LoginBoxContent { get; set; } = "";
    private DateTime LoginBoxStart { get; set; } = DateTime.Now;
    private DateTime LoginBoxEnd { get; set; } = DateTime.Now;
    private bool IsConfirmLoginBox { get; set; } = false;

    private string m_strResPackageDataURL = "";
    private string m_strResPackageName = "";
    private string m_strResPackagePath = "";
    private int m_nResPackageSize = 0;

    public string ResVersion { get; private set; } = "";

    public TextAsset assetVersion = null;
    public TextAsset assetUIString = null;

    private bool IsLoadingLoadConf { get; set; } = false;

    private bool bShowQuitGameUI = false;

    private int languageIndex { get; set; } = 3;

    private Dictionary<string, string> dictUIString = new Dictionary<string, string>();

    private EngineLauncherResUI m_engineLauncherResUI;

    // 资源系统运行模式
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    //是否Debug版本
    public static bool IsDebug = true;
    
    void Awake()
    {
        GRoot.inst.SetContentScaleFactor(720, 1280 /*, UIContentScaler.ScreenMatchMode.MatchWidth*/);
        
        if (m_engineLauncherResUI == null)
        {
            m_engineLauncherResUI = EngineLauncherResUI.Init();
        }
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));  
    }

    private void DestroyEngineLauncherResUI()
    {
        if (m_engineLauncherResUI != null)
        {
            m_engineLauncherResUI.Dispose();
            m_engineLauncherResUI = null;
        }
    }

    void Start()
    {
#if !UNITY_EDITOR && PF_TAPTAPSDK
        nPlatformCode = PLATFORM_TAPTAP_SDK;
        int firstApp = UnityEngine.PlayerPrefs.GetInt("firstApp", 0);
        if (firstApp == 0)
        {
            InitUIString();
            string strTitle = GetStringByKey(KEY_PLATFORM_TITLE);
            string strContent = GetStringByKey(KEY_PLATFORM_DESC);
            string strBtnOK = GetStringByKey(KEY_AGREE);
            string strBtnCancel = GetStringByKey(KEY_REFUSE);
            ShowTapSDKWindows(strTitle, strContent, strBtnOK, strBtnCancel, OnStartUp, OnCancelUpdateRes);
        }
        else
        {
            StartCoroutine(StartUp());
        }
#else
        StartCoroutine(StartUp());
#endif
    }

    void Update()
    {
        CheckMaintainWindowsTime();
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     RequestQuitGame();
        // }
    }

    private void OnStartUp()
    {
        StartCoroutine(StartUp());
        UnityEngine.PlayerPrefs.SetInt("firstApp", 1);
        UnityEngine.PlayerPrefs.SetInt("userCheck", 1);
    }
    
    private void ShowTapSDKWindows(string title, string content, string strBtnOK, string strBtnCancel, Action callbackOk, Action callbackCancel)
    {
        m_engineLauncherResUI.ShowTapSDKWindows(title, content, strBtnOK, strBtnCancel, callbackOk, callbackCancel);
    }
    
    public IEnumerator StartUp()
    {
        IsConfirmLoginBox = false;
        IsConfirmMaintain = false;

        if(IsDebug)
            InitReporter();

#if !UNITY_EDITOR && PF_WEIXIN
        yield return null;

        InitLanguage();
        InitUIString();

        var updateManager = WX.GetUpdateManager();

        if (updateManager == null)
        {
            yield break;
        }

        CI.LogWarning("OnCheckForUpdate");

        updateManager.OnCheckForUpdate(
            (res) =>
            {
                CI.LogWarning("OnCheckForUpdate Done " + res.hasUpdate);
            });

        updateManager.OnUpdateReady(
            (errMsg) =>
            {
                CI.LogWarning("OnUpdateReady");

                if (errMsg != null && !string.IsNullOrEmpty(errMsg.errMsg))
                {
                    string strBtnOK = GetStringByKey(KEY_YES);
                    string strContent = errMsg.errMsg;
                    ShowSingleTipWindows(strContent, strBtnOK, null);
                }

                ShowModalOption callback = new ShowModalOption();

                callback.title = GetStringByKey(KEY_ORIGINTIP);
                callback.content = GetStringByKey(KEY_ORIGINUPDATE);
                callback.success = (res) =>
                {
                    // 请求完新版本信息的回调
                    if (res.confirm)
                    {
                        updateManager.ApplyUpdate();
                    }
                };
                WX.ShowModal(callback);
            });

        updateManager.OnUpdateFailed(
            (errMsg) =>
            {
                CI.LogWarning("OnUpdateFailed");

                if (errMsg != null && !string.IsNullOrEmpty(errMsg.errMsg))
                {
                    string strBtnOK = GetStringByKey(KEY_YES);
                    string strContent = errMsg.errMsg;
                    ShowSingleTipWindows(strContent, strBtnOK, null);
                }
            });

        InitAndCheckResVersion();

        this.LoadConf();
        SetTitleBg();
#elif !UNITY_EDITOR && UNITY_WEBGL
        yield return null;

        InitLanguage();
        InitUIString();
        InitAndCheckResVersion();

        this.LoadConf();
        SetTitleBg();
#else
        yield return null;

        InitLanguage();
        InitUIString();
        
        InitAndCheckResVersion();

        // 开始加载资源
        this.LoadConf();
        SetTitleBg();
#endif
    }
    
    // 获取程序版本
    private string GetAppVersion()
    {
        return Application.version;
    }

    // 获取资源版本
    private string GetResVersion()
    {
        return ResVersion;
    }

    private Shader GetShader(string shaderName)
    {
        return Shader.Find(shaderName);
    }

    private void InitLanguage()
    {
        if (!UnityEngine.PlayerPrefs.HasKey(Language_Index))
        {
        }
        else
        {
            // languageIndex = PlayerPrefs.GetInt(Language_Index, 0);//默认英文
            languageIndex = PlayerPrefs.GetInt(Language_Index, 3);//默认中文
        }

        CI.LogWarningFormat("InitLanguage {0}", languageIndex);
    }

    private void InitUIString()
    {
        if (dictUIString.Count <= 0)
        {
            if (null != assetUIString)
            {
                string strValue = assetUIString.text;
                strValue = strValue.Replace("\r", "");
                string[] strLines = strValue.Split('\n');

                for (int nIndex = 0; nIndex < strLines.Length; ++nIndex)
                {
                    string strFileDataLine = strLines[nIndex];
                    string[] strFileDataLines = strFileDataLine.Split(" =");

                    if (strFileDataLines != null && strFileDataLines.Length >= 2)
                    {
                        string key = strFileDataLines[0].Trim();
                        string value = strFileDataLines[1].Trim();

                        if (!dictUIString.ContainsKey(key))
                        {
                            value = value.Replace("<br>", "\n");
                            value = value.Replace("\\n", "\n");
                            dictUIString.Add(key, value);
                        }
                    }
                }
            }
        }
    }

    private void SetTitleBg()
    {
        string name = GetLanguageTypeName();
        m_engineLauncherResUI.UpdateTitleBg(name);
    }
    
    /// <summary>
    /// 获取语言名称简写
    /// </summary>
    /// <returns></returns>
    public string GetLanguageTypeName()
    {
        string name = "zh";
        switch (languageIndex)
        {
            case defLanguage.English:
                name = "en";
                break;
            case defLanguage.Korean:
                name = "kr";
                break;
            case defLanguage.Japanese:
                name = "ja";
                break;
            case defLanguage.ChineseSimplified:
                name = "zh";
                break;
            case defLanguage.ChineseTraditional:
                name = "tw";
                break;
        }
        return name;
    }
    
    private string GetStringByKey(string key)
    {
        string newKey = string.Format("{0}_{1}", key, languageIndex);

        string value;

        if (dictUIString.TryGetValue(newKey, out value))
        {
            return value;
        }

        if (dictUIString.TryGetValue(key, out value))
        {
            return value;
        }

        return "";
    }

    private string FormatStringByKey(string key, params object[] args)
    {
        var value = GetStringByKey(key);

        if (!string.IsNullOrEmpty(value))
        {
            return string.Format(value, args);
        }

        return "";
    }

    private void InitAndCheckResVersion()
    {
        m_engineLauncherResUI.UpdateVersion(FormatStringByKey(KEY_APP, GetAppVersion()));

        if (YooAssets.Initialized)
        {
            var package = YooAssets.TryGetPackage(STR_DEFAULT_PACKAGE_NAME);
            if (package == null)
            {
                CI.LogError("InitAndCheckResVersion package == null");
                return;
            }

            var appRes = string.Format("{0}.{1}", GetAppVersion(), package.GetPackageVersion());
            m_engineLauncherResUI.UpdateVersion(FormatStringByKey(KEY_APP, appRes));

            CI.LogWarning($"InitAndCheckResVersion appRes:{appRes}");
        }
    }

    private int GetPlatform()
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
            nPlatformCode = CI.CallJavaWithReturn<int>("GetPlatform");
#elif !UNITY_EDITOR && UNITY_IOS
            nPlatformCode = CI.SDK_GetPlatform();
#else
            nPlatformCode = PLATFORM_CODE_GENERAL;
#endif
        }

        return nPlatformCode;
    }

    // 是否内部渠道
    private bool IsNormalPlatform()
    {
        int nPlatform = GetPlatform();

        if (nPlatform == PLATFORM_CODE_GENERAL)
        {
            return true;
        }

        return false;
    }

    #region HttpGet Output: byte[]

    private void HttpGet(MonoBehaviour monoHost, string strUrl, Action<byte[], DateTime> actionSuccess, Action actionFail)
    {
        if (monoHost == null)
        {
            return;
        }

        monoHost.StartCoroutine(OnHttpGetWithUnityWebRequest(
            strUrl,
            actionSuccess,
            actionFail));
    }

    private IEnumerator OnHttpGetWithUnityWebRequest(string strUrl, Action<byte[], DateTime> actionSuccess, Action actionFail)
    {
        CI.LogWarningFormat("OnHttpGetWithUnityWebRequest {0}", strUrl);

        UnityWebRequest httpRequest = UnityWebRequest.Get(strUrl);

        httpRequest.timeout = WEB_GET_TIMEOUT;

        yield return httpRequest.SendWebRequest();

        if (httpRequest.error != null)
        {
            CI.LogWarningFormat("OnHttpGetWithUnityWebRequest Fail {0} {1}", strUrl, httpRequest.error);
            try
            {
                if (actionFail != null)
                {
                    actionFail();
                }
            }
            catch (Exception e)
            {
                CI.LogException(e);
            }

            httpRequest.Dispose();
            yield break;
        }

        if (httpRequest.downloadHandler == null)
        {
            CI.LogWarningFormat("OnHttpGetWithUnityWebRequest downloadHandler null {0}", strUrl);
            try
            {
                if (actionFail != null)
                {
                    actionFail();
                }
            }
            catch (Exception e)
            {
                CI.LogException(e);
            }

            httpRequest.Dispose();
            yield break;
        }

        yield return httpRequest.downloadHandler.data;

        var byData = httpRequest.downloadHandler.data;
        var strDate = httpRequest.GetResponseHeader("DATE");
        var dateHttp = DateTime.Now;

        if (strDate != null)
        {
            if (DateTime.TryParse(strDate, out dateHttp))
            {
                dateHttp = dateHttp.ToLocalTime();
            }
            else
            {
                dateHttp = DateTime.Now;
            }
        }

        try
        {
            if (actionSuccess != null)
            {
                actionSuccess(byData, dateHttp);
            }
        }
        catch (Exception e)
        {
            CI.LogException(e);
        }

        httpRequest.Dispose();
    }

    #endregion

    private string GetRandomParam()
    {
        return string.Format("?t={0}", CI.GetTimeStamp(DateTime.Now).ToString());
    }

    private int CompareVersion(string strVersionBase, string strVersionTarget)
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
                    nBaseVersion = CI.GetInt(strLineBase[i]);
                }

                if (i < strLineTarget.Length)
                {
                    nTargetVersion = CI.GetInt(strLineTarget[i]);
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
            CI.LogError(ex.Message);
        }

        return 0;
    }

    private void ShowUILoading(int step = 1)
    {
        this.ShowProgress(step);
    }

    private void HideUILoading()
    {
        this.HideProgress();
        this.HideTipStep();
    }

    private void HideTip()
    {
        m_engineLauncherResUI?.HideTip();
    }

    private void TipStep(string strValue)
    {
        m_engineLauncherResUI?.TipStep(strValue);
    }

    private void HideTipStep()
    {
        TipStep("");
    }

    /// <summary>
    /// 下载更新包进度
    /// </summary>
    /// <param name="nDownSize"></param>
    /// <param name="nAllSize"></param>
    private void OnNewVersionPointDownSync(long nDownSize, long nAllSize)
    {
        if (nAllSize == 0)
        {
            nAllSize = 1;
        }

        float fDownSize = nDownSize / 1024.0f / 1024.0f;
        float fAllSize = nAllSize / 1024.0f / 1024.0f;

        float fPrecent = fDownSize / fAllSize;
        fPrecent = Mathf.Clamp01(fPrecent) * 100.0f;

        TipStep(FormatStringByKey(KEY_UPDATERESDOWNSYNC, fPrecent));
    }

    public bool IsPassVersionCheck()
    {
        bool bPassVersionCheck = false;

        if (IsNormalPlatform())
        {
#if USE_RESUPDATE
            bPassVersionCheck = false;
#else
            bPassVersionCheck = false;
#endif
        }
        else
        {
#if NOUSE_RESUPDATE
            bPassVersionCheck = true;
#else
            bPassVersionCheck = false;
#endif
        }

        return bPassVersionCheck;
    }

    private void LoadConf()
    {
        CI.LogWarning("LoadConf");

        if (this.IsLoadingLoadConf)
        {
            CI.LogWarning("LoadConf Is Loading");
            return;
        }

        this.IsLoadingLoadConf = true;
        this.HideTip();
        this.ShowUILoading(1);

        int nPlatform = GetPlatform();

        strConfURL = string.Format(STR_CONF_URL, nPlatform);

        if (IsNormalPlatform())
        {
            LoadConfDone();
            return;
        }

        HttpGet(this, strConfURL,
            (byte[] byData, DateTime dateServer) =>
            {
                if (byData == null)
                {
                    CI.LogWarning("LoadConf Error By Null");

                    LoadConfFail();
                    return;
                }

                dateTimeServerRecv = dateServer;
                dateTimeServerLocal = DateTime.Now;

                string response = UTF8Encoding.UTF8.GetString(byData);
                CI.LogWarningFormat("LoadConf\n{0} {1} ", response, dateServer.ToString());

                var jsonResponse = JSONObject.Create(response);

                if (jsonResponse != null && jsonResponse.HasField("code"))
                {
                    var code = jsonResponse.GetIntValue("code");
                    m_ip = jsonResponse.GetStringValue("ip");

                    if (code == 0)
                    {
                        var jsonCfg = jsonResponse.GetField("cfg");

                        if (jsonCfg != null)
                        {
                            m_strLimitAppVersion = jsonCfg.GetStringValue("LimitAppVersion");

                            m_mapLimitResVersion.Clear();

                            string strLimitResVersion = jsonCfg.GetStringValue("LimitResVersion");

                            if (!string.IsNullOrEmpty(strLimitResVersion))
                            {
                                string[] strResVersions = strLimitResVersion.Split(';');

                                foreach (var strResVersion in strResVersions)
                                {
                                    string[] strTmp = strResVersion.Split(',');

                                    if (strTmp != null && strTmp.Length >= 2)
                                    {
                                        m_mapLimitResVersion[strTmp[0]] = CI.GetInt(strTmp[1]);
                                    }
                                }
                            }

                            m_strReviewAppVersion = jsonCfg.GetStringValue("ReviewAppVersion");
                            m_strAppStoreURL = jsonCfg.GetStringValue("AppStoreURL");
                            m_strNewsURL = jsonCfg.GetStringValue("NewsUrl");
                            m_strChannelSwitchURL = jsonCfg.GetStringValue("channelSwitch");

                            CI.LogWarningFormat("SetVersionLimit ({0}) ({1})", m_strLimitAppVersion,
                                m_strReviewAppVersion);

                            var ServerID = jsonCfg.GetStringValue("ServerID");
                            var ServerURL = jsonCfg.GetStringValue("ServerURL");
                            var ServerName = jsonCfg.GetStringValue("ServerName");
                            var ServerPayURL = jsonCfg.GetStringValue("ServerPayURL");
                            var ReviewServerID = jsonCfg.GetStringValue("ReviewServerID");
                            var ReviewServerURL = jsonCfg.GetStringValue("ReviewServerURL");
                            var ReviewServerName = jsonCfg.GetStringValue("ReviewServerName");
                            var ReviewServerPayURL = jsonCfg.GetStringValue("ReviewServerPayURL");

                            m_strServerInfo = string.Format("{0};{1};{2};{3};{4};{5};{6};{7}",
                                ServerID, ServerName, ServerURL, ServerPayURL,
                                ReviewServerID, ReviewServerName, ReviewServerURL, ReviewServerPayURL);

                            CI.LogWarningFormat("SetServerInfo {0}", m_strServerInfo);

                            IsMaintain = CI.GetBool(jsonCfg.GetStringValue("IsMaintain"));
                            MaintainContent = jsonCfg.GetStringValue("MaintainContent");
                            MaintainStart = CI.GetDateTime(jsonCfg.GetStringValue("MaintainStart"));
                            MaintainEnd = CI.GetDateTime(jsonCfg.GetStringValue("MaintainEnd"));

                            if (!string.IsNullOrEmpty(MaintainContent))
                            {
                                MaintainContent = MaintainContent.Replace("\\n", "\n");
                                MaintainContent = MaintainContent.Replace("<br>", "\n");
                            }

                            CI.LogWarningFormat("SetMaintainInfo {0} {1} {2} {3}", IsMaintain, MaintainContent,
                                MaintainStart.ToString(), MaintainEnd.ToString());

                            IsLoginBox = CI.GetBool(jsonCfg.GetStringValue("IsLoginBox"));
                            LoginBoxContent = jsonCfg.GetStringValue("LoginBoxContent");
                            LoginBoxStart = CI.GetDateTime(jsonCfg.GetStringValue("LoginBoxStart"));
                            LoginBoxEnd = CI.GetDateTime(jsonCfg.GetStringValue("LoginBoxEnd"));

                            if (!string.IsNullOrEmpty(LoginBoxContent))
                            {
                                LoginBoxContent = LoginBoxContent.Replace("\\n", "\n");
                                LoginBoxContent = LoginBoxContent.Replace("<br>", "\n");
                            }

                            CI.LogWarningFormat("SetLoginBox {0} {1} {2} {3}", IsLoginBox, LoginBoxContent,
                                LoginBoxStart.ToString(), LoginBoxEnd.ToString());

                            m_lstWhiteListIP.Clear();

                            string strWhiteListIP = jsonCfg.GetStringValue("WhiteListIP");

                            if (!string.IsNullOrEmpty(strWhiteListIP))
                            {
                                string[] strLines = strWhiteListIP.Split(';');

                                foreach (var line in strLines)
                                {
                                    m_lstWhiteListIP.Add(line);
                                }
                            }

                            m_lstWhiteListAccount.Clear();

                            string WhiteListAccount = jsonCfg.GetStringValue("WhiteListAccount");

                            if (!string.IsNullOrEmpty(WhiteListAccount))
                            {
                                string[] strLines = WhiteListAccount.Split(';');

                                foreach (var line in strLines)
                                {
                                    m_lstWhiteListAccount.Add(line);
                                }
                            }

                            CI.LogWarningFormat("WhiteList {0} {1}", strWhiteListIP, WhiteListAccount);
                        }
                    }
                }

                LoadConfDone();
            },
            () =>
            {
                CI.LogErrorFormat("LoadConf Error URL {0} ", strConfURL);

                LoadConfFail();
            });
    }

    private void LoadConfDone()
    {
        CI.LogWarning("LoadConfDone");

        this.IsLoadingLoadConf = false;
        HideUILoading();

        //编辑器默认跳过     todo 配置加载完成  判断是热更还是整包更
#if !UNITY_EDITOR
        StartCoroutine(StartStepByYooAssetsInitPackage());
#else
        if(PlayMode == EPlayMode.OfflinePlayMode)
            StartCoroutine(StartStepByYooAssetsInitPackage());
        else
        {
            CheckNewVersionNone();
        }
#endif
    }

    private void LoadConfFail()
    {
        CI.LogWarning("LoadConfFail");

        this.IsLoadingLoadConf = false;
        HideUILoading();

        string strBtnOK = GetStringByKey(KEY_YES);
        string strContent = GetStringByKey(KEY_LOADCONFFAIL);
        ShowSingleTipWindows(strContent, strBtnOK, LoadConf);
    }

    private IEnumerator StartStepByYooAssetsInitPackage()
    {
        // TODO AB包 初始化资源系统
        YooAssets.Initialize();

        // 创建默认的资源包
        var package = YooAssets.CreatePackage("DefaultPackage");

        // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
        YooAssets.SetDefaultPackage(package);

        m_engineLauncherResUI.ShowUILoading();
        InitializationOperation initOperation = null;
        if (PlayMode == EPlayMode.OfflinePlayMode)
        {
            var createParameters = new OfflinePlayModeParameters();
            initOperation = package.InitializeAsync(createParameters);
        }
        else if (PlayMode == EPlayMode.HostPlayMode)
        {
            Debug.LogFormat("==================走热更了。。。。。。。。。。。。。");
            var createParameters = new HostPlayModeParameters();
            createParameters.BuildinQueryServices = new GameQueryServices();
            createParameters.RemoteServices = new RemoteServices(m_defaultHostServer, m_fallbackHostServer);
            initOperation = package.InitializeAsync(createParameters);
        }
        else if (PlayMode == EPlayMode.WebPlayMode)
        {
            var createParameters = new WebPlayModeParameters();
            createParameters.BuildinQueryServices = new GameQueryServices();
            createParameters.RemoteServices = new RemoteServices(m_defaultHostServer, m_fallbackHostServer);
            initOperation = package.InitializeAsync(createParameters);
        }

        yield return initOperation;

        if (initOperation?.Status == EOperationStatus.Succeed)
        {
            CI.Log("StartStepByYooAssetsInitPackage Initialize Success");

            ResVersion = package.GetPackageVersion();
            
            m_engineLauncherResUI.HideUILoading();
            
            StartStepByCheckWindowsTip();
        }
        else
        {
            CI.LogError($"StartStepByYooAssetsInitPackage Initialize Failed:{initOperation?.Error}");

            m_engineLauncherResUI.HideUILoading();
            StartStepByYooAssetsInitPackageFail();
        }
    }

    private void StartStepByYooAssetsInitPackageFail()
    {
        HideUILoading();

        string strBtnOK = GetStringByKey(KEY_YES);
        string strContent = GetStringByKey(KEY_LOADCONFFAIL);
        ShowSingleTipWindows(strContent, strBtnOK, OnConfirmExitApp);
    }

    // 获取SDK时间
    private DateTime GetSDKServerTime()
    {
        var passTime = DateTime.Now - dateTimeServerLocal;
        return dateTimeServerRecv + passTime;
    }

    // 是否白名单
    private bool IsWhiteList()
    {
        return false;
    }

    // 提审版本
    private bool IsReviewStoreVersion()
    {
        return !string.IsNullOrEmpty(m_strReviewAppVersion) && GetAppVersion().Equals(m_strReviewAppVersion);
    }

    // 是否需要登录弹窗
    private bool IsNeedSDKLoginBox()
    {
        // 提审版本不弹窗
        if (IsReviewStoreVersion())
        {
            return false;
        }

        if (IsLoginBox)
        {
            DateTime timeNow = GetSDKServerTime();

            if (timeNow >= LoginBoxStart && timeNow <= LoginBoxEnd)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    // 是否需要维护
    private bool IsNeedSDKMaintain()
    {
        bool bNeedSDKMaintain = false;

        if (IsReviewStoreVersion())
        {
            // 提审版本不显示维护
            bNeedSDKMaintain = false;
        }
        else if (IsMaintain)
        {
            if (IsWhiteList())
            {
                // 白名单需要弹窗维护确认
                bNeedSDKMaintain = !IsConfirmMaintain;
            }
            else
            {
                DateTime timeNow = GetSDKServerTime();

                if (timeNow >= MaintainStart && timeNow <= MaintainEnd)
                {
                    bNeedSDKMaintain = true;
                }
                else
                {
                    bNeedSDKMaintain = false;
                }
            }
        }

        if (bNeedSDKMaintain)
        {
            if (timerMaintain == null)
            {
                timerMaintain = new CBaseTimer();
            }

            timerMaintain.Startup(1.0f);
        }
        else
        {
            if (timerMaintain != null)
            {
                timerMaintain.Clear();
                timerMaintain = null;
            }
        }

        return bNeedSDKMaintain;
    }

    private void StartStepByCheckWindowsTip()
    {
        if (IsNeedSDKMaintain())
        {
            this.HideUILoading();

            this.ShowMaintainWindows();
        }
        else if (IsNeedSDKLoginBox() && !IsConfirmLoginBox)
        {
            this.HideUILoading();

            this.ShowLoginBoxWindows();
        }
        else
        {
            // 检查新版本
            StartStepByUpdateRes();
        }
    }

    public void OnBtnClickLoginBoxYES()
    {
        IsConfirmLoginBox = true;

        StartStepByCheckWindowsTip();
    }

    private void CheckMaintainWindowsTime()
    {
        if (timerMaintain != null && timerMaintain.ToNextTime())
        {
            if (!UpdateMaintainWindowsTime())
            {
                // 时间结束
                timerMaintain.Clear();
                timerMaintain = null;

                LoadConf();
            }
        }
    }

    public void ShowMaintainWindows()
    {
        /*if (goMaintain != null)
        {
            goMaintain.SetActive(true);

            lblMaintainContent.text = MaintainContent;
            scrollViewMaintain.ResetPosition();
            
            if (IsWhiteList())
            {
                btnMaintainsYES.transform.Find("txt").GetComponent<UILabel>().text = GetStringByKey(KEY_YES);
            }
            else
            {
                btnMaintainsYES.transform.Find("txt").GetComponent<UILabel>().text = GetStringByKey(KEY_REFRESH);
            }
            
            UpdateMaintainWindowsTime();
        }*/
    }

    public void ShowLoginBoxWindows()
    {
        /*if (goLoginBox != null)
        {
            goLoginBox.SetActive(true);

            lblLoginBoxContent.text = LoginBoxContent;
            scrollViewLoginBox.ResetPosition();
            btnLoginBoxYES.transform.Find("txt").GetComponent<UILabel>().text = GetStringByKey(KEY_YES);
        }*/
    }

    private bool UpdateMaintainWindowsTime()
    {
        DateTime timeNow = GetSDKServerTime();

        if (MaintainEnd >= timeNow)
        {
            /*TimeSpan timeDiff = MaintainEnd - timeNow;
            
            if (timeDiff.TotalHours >= 24 * 7)
            {
                // 维护时间超过7天，不显示倒计时
                lblMaintainTime.text = "";
            }
            else
            {
                if (timeDiff.Days > 0)
                {
                    lblMaintainTime.text = FormatStringByKey(KEY_MAINTAIN_TIMEDAY,
                        timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
                }
                else if (timeDiff.Hours > 0)
                {
                    lblMaintainTime.text = FormatStringByKey(KEY_MAINTAIN_TIMEHOUR,
                        timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
                }
                else if (timeDiff.Minutes > 0)
                {
                    lblMaintainTime.text = FormatStringByKey(KEY_MAINTAIN_TIMEMINUTE,
                        timeDiff.Minutes, timeDiff.Seconds);
                }
                else
                {
                    lblMaintainTime.text = FormatStringByKey(KEY_MAINTAIN_TIMESECOND,
                        timeDiff.Seconds);
                }
            }*/

            return true;
        }
        else
        {
            //lblMaintainTime.text = "";
            return false;
        }
    }

    private void StartStepByUpdateRes()
    {
        CI.LogFormat("EngineLauncherRes: Update Resource ({0}) ({1})", GetAppVersion(), m_strLimitAppVersion);

        this.HideTip();
        this.ShowUILoading(2);

        bool bPassVersionCheck = IsPassVersionCheck();

        if (bPassVersionCheck)
        {
            CheckNewVersionNone();
            return;
        }

        // 检测程序版本
        bool bNewAppVersion = false;

        if (!string.IsNullOrEmpty(m_strLimitAppVersion)) // todo 版本判断 是否整包更新
        {
            int nCompareValue = CompareVersion(GetAppVersion(), m_strLimitAppVersion);

            if (nCompareValue > 0)
            {
                bNewAppVersion = true;
            }
        }

        if (bNewAppVersion)
        {
            // 通知UI表现，引导玩家整包更新
            OnNewVersionBigVersionUpdate();
        }
        else
        {
            // 程序版本通过，检测资源版本更新
#if !UNITY_EDITOR && PF_WEIXIN
            OnUpdateResSuccess();
#elif !UNITY_EDITOR && PF_DOUYIN
            OnUpdateResSuccess();
#else
            OnLoadSuitableResVersion(this);
#endif
        }
    }

    /// <summary>
    /// 程序版本更新 包体强更 整包更新
    /// </summary>
    private void OnNewVersionBigVersionUpdate()
    {
        CI.Log("EngineLauncherRes: Need Update App");

        this.HideUILoading();

        string strBtnOK = GetStringByKey(KEY_YES);
        string strContent = GetStringByKey(KEY_BIGVERSIONUPDATE);
#if PF_WEIXIN || PF_DOUYIN
        strContent = GetStringByKey(KEY_BIGVERSIONUPDATEWX);
#elif !UNITY_EDITOR && PF_TAPTAPSDK
        CI.Log(" ==========Update App  ===pop ==");
#endif
        ShowSingleTipWindows(strContent, strBtnOK, OnConfirmNewVersionBigVersionUpdate);
    }

    private void OnConfirmNewVersionBigVersionUpdate()
    {
#if PF_WEIXIN || PF_DOUYIN
        CI.ApplicationQuit();
#elif !UNITY_EDITOR && PF_TAPTAPSDK
        CI.Log(" !UNITY_EDITOR && PF_TAPTAPSDK    ==========Update App");
        StartCoroutine(OnLoadGameAssembly(true));
#elif USE_COMBO_SDK
        if (ThirdPartyWrapper.Instance.IsUpdateGameAvailable())
        {
            // SDK 更新唤醒功能
            ThirdPartyWrapper.Instance.UpdateGame();
        }
        else
        {
            // SDK 强更URL功能
            ThirdPartyWrapper.Instance.GetDownloadUrl((url) =>
            {
                if (!string.IsNullOrEmpty(url))
                {
                    Application.OpenURL(url);
                }
                else
                {
                    OnConfirmNewVersionBigVersionUpdateWithInside();
                }
            });
        }
#else
        OnConfirmNewVersionBigVersionUpdateWithInside();
#endif
    }

    private void OnConfirmNewVersionBigVersionUpdateWithInside()
    {
#if UNITY_ANDROID
        this.HttpGet(this, m_strAppStoreURL,
            (byte[] byData, DateTime dateServer) =>
            {
                if (byData == null)
                {
                    CI.LogWarning("OnConfirmNewVersionBigVersionUpdate Error By Null");
                    string strBtnOK = GetStringByKey(KEY_YES);
                    string strContent = GetStringByKey(KEY_LOADRESFAIL);
                    ShowSingleTipWindows(strContent, strBtnOK, OnConfirmNewVersionBigVersionUpdate);
                    return;
                }

                string response = UTF8Encoding.UTF8.GetString(byData);

                CI.LogWarningFormat("OnConfirmNewVersionBigVersionUpdate\n{0} ", response);

                var jsonResponse = JSONObject.Create(response);

                if (jsonResponse != null && jsonResponse.HasField("PlatformInfo_update"))
                {
                    var arrUpdate = jsonResponse.GetField("PlatformInfo_update");

                    if (arrUpdate != null)
                    {
                        var channel = ThirdPartyWrapper.Instance.GetChannelName();
                        var cps = ThirdPartyWrapper.Instance.GetVariant();
                        var baseUpdateType = "";
                        var baseUpdateData = "";
                        var onlyUpdateType = "";
                        var onlyUpdateData = "";

                        for (int i = 0; i < arrUpdate.Count; i++)
                        {
                            var update = arrUpdate[i];

                            string strCps = update.GetStringValue("Cps");
                            string strUpdateType = update.GetStringValue("UpdateType");
                            string strUpdateData = update.GetStringValue("UpdateData");

                            if (string.IsNullOrEmpty(strCps))
                            {
                                baseUpdateType = strUpdateType;
                                baseUpdateData = strUpdateData;
                            }
                            else if (strCps.Equals(cps))
                            {
                                onlyUpdateType = strUpdateType;
                                onlyUpdateData = strUpdateData;
                            }
                        }

                        // 没有匹配到CPS，使用基准渠道数据
                        if (string.IsNullOrEmpty(onlyUpdateType))
                        {
                            onlyUpdateType = baseUpdateType;
                            onlyUpdateData = baseUpdateData;
                        }

                        CI.LogWarningFormat("OnConfirmNewVersionBigVersionUpdate type({0}) data({1})", onlyUpdateType, onlyUpdateData);

                        if (onlyUpdateType.Equals("1") && !string.IsNullOrEmpty(onlyUpdateData))
                        {
                            // 应用商店更新
                            if (onlyUpdateData.Contains(","))
                            {
                                var split = onlyUpdateData.Split(',');

                                if (split.Length >= 2)
                                {
                                    CI.JumpToAppStore(split[0], split[1]);
                                }
                            }
                            else
                            {
                                CI.JumpToAppStore(onlyUpdateData, "");
                            }
                        }
                        else if (onlyUpdateType.Equals("2") && !string.IsNullOrEmpty(onlyUpdateData))
                        {
                            // 外链
                            Application.OpenURL(onlyUpdateData);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(onlyUpdateData))
                            {
                                onlyUpdateData = GetStringByKey(KEY_BIGVERSIONUPDATECOMMON);
                            }

                            string strBtnOK = GetStringByKey(KEY_YES);
                            ShowSingleTipWindows(onlyUpdateData, strBtnOK, OnConfirmExitApp);
                        }
                    }
                }
            },
            () =>
            {
                CI.LogError("OnConfirmNewVersionBigVersionUpdate HttpError");

                string strBtnOK = GetStringByKey(KEY_YES);
                string strContent = GetStringByKey(KEY_LOADRESFAIL);
                ShowSingleTipWindows(strContent, strBtnOK, OnConfirmNewVersionBigVersionUpdate);
            });
#elif UNITY_IOS
        CI.JumpToAppStore(m_strAppStoreURL, "");
#endif
    }

    private void ResetSingleTipWindows()
    {
        m_engineLauncherResUI.SetTipWindowsOKText(GetStringByKey(KEY_YES));
    }

    private void ShowSingleTipWindows(string content, string strBtnOK, Action callbackOk)
    {
        m_engineLauncherResUI.ShowSingleTipWindows("", content, strBtnOK, false, callbackOk, null);
    }

    private void ShowDoubleTipWindows(string content, string strBtnOK, string strBtnCancel, Action callbackOk, Action callbackCancel)
    {
        m_engineLauncherResUI.ShowDoubleTipWindows("", content, strBtnOK, strBtnCancel, false, callbackOk, callbackCancel, null);
    }

    /// <summary>
    /// 无需更新
    /// </summary>
    private void CheckNewVersionNone()
    {
        CI.Log("EngineLauncherRes: Update Resource None");

        OnUpdateResSuccess();
    }

    /// <summary>
    /// todo 需要热更新，提示玩家下载
    /// </summary>
    /// <param name="nNewVersion"></param>
    /// <param name="nSize"></param>
    private void OnNewVersionCheckUpdate(long nSize)
    {
        if (0 == nSize)
        {
            return;
        }

        this.HideUILoading();

        // 提示UI，是否继续更新，如果是wifi直接下载更新
        if (CI.IsWIFI())
        {
            this.OnConfirmNewVersionCheckUpdate();
        }
        else
        {
            string strBtnOK = GetStringByKey(KEY_YES);
            string strBtnCancel = GetStringByKey(KEY_QUIT);
            float tipValue = Mathf.Max(nSize / 1024.0f / 1024.0f, 0.01f);
            string strContent = FormatStringByKey(KEY_UPDATERESSIZETIP, tipValue);
            ShowDoubleTipWindows(strContent, strBtnOK, strBtnCancel, OnConfirmNewVersionCheckUpdate, OnCancelNewVersionCheckUpdate);
        }
    }

    private void OnConfirmNewVersionCheckUpdate()
    {
        this.HideTip();
        this.HideTipStep();
        //开启下载
        m_downloader?.BeginDownload();
    }

    private void OnCancelNewVersionCheckUpdate()
    {
        CI.Log("EngineLauncherRes: OnCancelNewVersionCheckUpdate");

        this.HideTip();
        CI.ApplicationQuit();
    }

    private void CheckUpdateResSuccess()
    {
        OnUpdateResSuccess();
    }

    private void CheckUpdateResFail()
    {
        CI.LogError("EngineLauncherRes: Update Resource Fail");

        this.HideUILoading();

        string strBtnOK = GetStringByKey(KEY_YES);
        string strBtnCancel = GetStringByKey(KEY_QUIT);
        string strContent = GetStringByKey(KEY_UPDATERESFAIL);
        ShowDoubleTipWindows(strContent, strBtnOK, strBtnCancel, StartStepByUpdateRes, OnCancelUpdateRes);
    }

    private void OnCancelUpdateRes()
    {
        this.HideTip();

        CI.ApplicationQuit();
    }

    private void OnLoadSuitableResVersion(MonoBehaviour monoHost)
    {
        //int nValue = 0;
        // 白名单不受版本限制
        if (!IsWhiteList()) //if (!IsWhiteList() && m_mapLimitResVersion.TryGetValue(GetAppVersion(), out nValue))
        {
            StartCoroutine(LoadSuitableResWithYooAssets());
        }
        else
        {
            // 不需要更新
            CheckNewVersionNone();
        }
    }

    /// <summary>
    /// //TODO 使用YooAssets开始更新步骤
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadSuitableResWithYooAssets()
    {
        var package = YooAssets.TryGetPackage(STR_DEFAULT_PACKAGE_NAME);
        if (package == null)
        {
            CI.LogError("LoadSuitableResWithYooAssets package == null");

            CheckUpdateResFail();
            yield break;
        }

        //获取资源版本
        var operation = package.UpdatePackageVersionAsync();
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            CI.LogError("LoadSuitableResWithYooAssets UpdatePackageVersionAsync error:" + operation.Error);

            CheckUpdateResFail();
            yield break;
        }

        string PackageVersion = operation.PackageVersion;

        //更新补丁清单
        var operation2 = package.UpdatePackageManifestAsync(PackageVersion);
        yield return operation2;

        if (operation2.Status != EOperationStatus.Succeed)
        {
            CI.LogError("LoadSuitableResWithYooAssets UpdatePackageManifestAsync error:" + operation2.Error);

            CheckUpdateResFail();
            yield break;
        }

        //下载补丁包信息，反馈到弹窗
        yield return DownloadPackageFilesWithDownloader();
    }

    private ResourceDownloaderOperation m_downloader;

    /// <summary>
    /// 获取下载的信息大小，显示弹窗上
    /// </summary>
    /// <returns></returns>
    IEnumerator DownloadPackageFilesWithDownloader()
    {
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;

        var package = YooAssets.TryGetPackage(STR_DEFAULT_PACKAGE_NAME);
        if (package == null)
        {
            CI.LogError("DownloadPackageFilesWithDownloader package == null");

            CheckUpdateResFail();
            yield break;
        }

        m_downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        if (m_downloader == null)
        {
            CI.LogError("DownloadPackageFilesWithDownloader m_downloader == null");

            CheckUpdateResFail();
            yield break;
        }

        //没有需要下载的资源
        if (m_downloader.TotalDownloadCount == 0)
        {
            CI.Log("DownloadPackageFilesWithDownloader CheckNewVersionNone");

            CheckNewVersionNone();
            yield break;
        }

        //需要下载的文件总数和总大小
        int totalDownloadCount = m_downloader.TotalDownloadCount;
        long totalDownloadBytes = m_downloader.TotalDownloadBytes;

        //注册回调方法
        m_downloader.OnDownloadErrorCallback = OnDownloadErrorFunction;
        m_downloader.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
        m_downloader.OnDownloadOverCallback = OnDownloadOverFunction;
        m_downloader.OnStartDownloadFileCallback = OnStartDownloadFileFunction;

        CI.Log($"DownloadPackageFilesWithDownloader totalDownloadCount:{totalDownloadCount}, totalDownloadBytes:{totalDownloadBytes}");

        OnNewVersionCheckUpdate(totalDownloadBytes);
    }

    /// <summary>
    /// 开始下载
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="sizeBytes"></param>
    private void OnStartDownloadFileFunction(string fileName, long sizeBytes)
    {
        CI.Log($"OnStartDownloadFileFunction fileName:{fileName}, sizeBytes:{sizeBytes}");
    }

    /// <summary>
    /// 下载完成
    /// </summary>
    /// <param name="isSucceed"></param>
    private void OnDownloadOverFunction(bool isSucceed)
    {
        CI.Log($"OnDownloadOverFunction isSucceed:{isSucceed}");

        if (isSucceed)
        {
            InitAndCheckResVersion();
            CheckUpdateResSuccess();
        }
        else
        {
            CheckUpdateResFail();
        }
    }

    /// <summary>
    /// 更新中
    /// </summary>
    /// <param name="totalDownloadCount"></param>
    /// <param name="currentDownloadCount"></param>
    /// <param name="totalDownloadBytes"></param>
    /// <param name="currentDownloadBytes"></param>
    private void OnDownloadProgressUpdateFunction(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
    {
        //CI.Log($"OnDownloadProgressUpdateFunction totalCount：{totalDownloadCount}, currentCount:{currentDownloadCount}, totalSize:{totalDownloadBytes}, currentSize:{currentDownloadBytes}");

        OnNewVersionPointDownSync(currentDownloadBytes, totalDownloadBytes);
    }

    /// <summary>
    /// 下载出错
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="error"></param>
    private void OnDownloadErrorFunction(string fileName, string error)
    {
        CI.LogError($"OnDownloadErrorFunction：fileName:{fileName}, error:{error}");

        CheckUpdateResFail();
    }

    public string GetGameDataPath(string strFilePath)
    {
#if !UNITY_EDITOR && PF_DOUYIN
        return string.Format("{0}/StreamingAssets/{1}", STR_WEBGLCDN_URL, strFilePath);
#else
        return Path.Combine(Application.streamingAssetsPath, strFilePath);
#endif
    }

    // 读取DataFile(仅仅Android)
    public byte[] LoadDataFile(string strPath)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        return CI.CallJavaWithReturn<byte[]>("LoadFile", strPath);
#else
        return null;
#endif
    }

    public byte[] ReadFileAllBytes(string strResPath, bool bRootPath = false)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return null;
#else
        var package = YooAssets.TryGetPackage(STR_DEFAULT_PACKAGE_NAME);
        if (package == null)
        {
            CI.LogError("ReadFileAllBytes package == null!");
            return null;
        }

        var location = "Assets/ArchivedBundles/" + strResPath;
        CI.LogWarning($"ReadFileAllBytes location:{location}");

        var handle = package.LoadRawFileSync(location);
        if (handle.Status == EOperationStatus.Succeed)
            return handle.GetRawFileData();
        else
            return null;
#endif
    }

    private void OnConfirmExitApp()
    {
        this.HideTip();
        CI.ApplicationQuit();
    }

    private static GameObject m_goReporter;
    private static Reporter m_reporter;

    public static bool IsCreateReporter()
    {
#if LOG_SLG || LOGW_SLG
        return true;
#else
        return false;
#endif
    }

    private static void InitReporter()
    {
        if (IsCreateReporter() && m_goReporter == null)
        {
            var go = Resources.Load("Reporter") as GameObject;

            if (go != null)
            {
                m_goReporter = Instantiate(go);
                m_goReporter.name = "Reporter";
                m_reporter = m_goReporter.GetComponent<Reporter>();
            }
        }
    }

    // Step.Done 更新资源
    private void OnUpdateResSuccess()
    {
        CI.LogWarning("EngineLauncher Turn To EngineGame");

        ShowUILoading(3);

        ResetSingleTipWindows();

        StartCoroutine(OnLoadGameAssembly());
    }

    private IEnumerator RetryHttpGet(RetryHttpGetUnit unit)
    {
        if (unit == null)
        {
            CI.LogError("RetryHttpGet Unit Is Null Error");
            yield break;
        }

        unit.loadType = EN_LOAD_TYPE.INIT;

        while (true)
        {
            if (unit.loadType == EN_LOAD_TYPE.INIT)
            {
                // 开始加载
                unit.loadType = EN_LOAD_TYPE.LOADING;
                HttpGet(this, unit.url,
                    (byte[] bys, DateTime dt) =>
                    {
                        if (bys == null)
                        {
                            CI.LogErrorFormat("RetryHttpGet RootContent is Null {0}", unit.url);
                            unit.loadType = EN_LOAD_TYPE.LOADERROR;
                        }
                        else
                        {
                            unit.bys = bys;
                            unit.loadType = EN_LOAD_TYPE.LOADED;
                        }
                    },
                    () =>
                    {
                        CI.LogErrorFormat("RetryHttpGet Fail {0}", unit.url);
                        unit.loadType = EN_LOAD_TYPE.LOADERROR;
                    });
            }
            else if (unit.loadType == EN_LOAD_TYPE.LOADED)
            {
                break;
            }
            else if (unit.loadType == EN_LOAD_TYPE.TERMINATION)
            {
                break;
            }
            else if (unit.loadType == EN_LOAD_TYPE.LOADERROR)
            {
                // 提示玩家错误，等待玩家操作继续加载
                unit.loadType = EN_LOAD_TYPE.LOADWAITCONTINUE;
                string strBtnOK = GetStringByKey(KEY_YES);
                string strContent = !string.IsNullOrEmpty(unit.retryTip) ? unit.retryTip : GetStringByKey(KEY_LOADRESFAIL);
                ShowSingleTipWindows(strContent, strBtnOK,
                    () =>
                    {
                        // 重新加载
                        this.HideTip();
                        unit.loadType = EN_LOAD_TYPE.INIT;
                    });
            }

            yield return null;
        }
    }

    // 加载 热更代码 加载程序集
    private IEnumerator OnLoadGameAssembly(bool isBigVersion = false)
    {
        Assembly assembly = null;

#if LOAD_HYBRIDCLR
#if UNITY_WEBGL && !UNITY_EDITOR
        var PreLoadGameAssembly = new RetryHttpGetUnit();
        PreLoadGameAssembly.url = GetGameDataPath(string.Format("mxj/manifest.ress"));
        StartCoroutine(RetryHttpGet(PreLoadGameAssembly));
        while (PreLoadGameAssembly.loadType != EN_LOAD_TYPE.LOADED)
        {
            yield return null;
        }
        if (PreLoadGameAssembly.bys == null)
        {
            CI.LogError("Assembly Manifest bytes Is Null!");
            yield break;
        }
        var text = Encoding.UTF8.GetString(PreLoadGameAssembly.bys);
        text = text.Replace("\r", "");
        var lines = text.Split('\n');
        foreach (var line in lines) 
        {
            var spl = line.Split(',');

            if (spl.Length >= 2)
            {
                dictManifest[spl[0]] = spl[1];
            }
        }
#endif

        if (assemblySize == 0)
        {
            // Load-ThirdParty-dll
            var tpDllNameMain = "mxj9.bin";
            var tpDllNameMainNew = tpDllNameMain;
            if (dictManifest.ContainsKey(tpDllNameMain))
            {
                tpDllNameMainNew = dictManifest[tpDllNameMain];
            }
            var tpDllPath = string.Format("mxj/{0}", tpDllNameMainNew);
            byte[] tpDllBytes = null;
            
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                var PreLoadTPDll = new RetryHttpGetUnit();
                PreLoadTPDll.url = GetGameDataPath(tpDllPath);
                StartCoroutine(RetryHttpGet(PreLoadTPDll));
                while (PreLoadTPDll.loadType != EN_LOAD_TYPE.LOADED)
                {
                    yield return null;
                }
                tpDllBytes = PreLoadTPDll.bys;
            }
            else
            {
                tpDllBytes = ReadFileAllBytes(tpDllPath);
            }
            
            if (tpDllBytes == null)
            {
                CI.LogError("Assembly ThirdParty bytes Is Null!");
                yield break;
            }
            
            int tpPos = 0;
            int tpSize = tpDllBytes.Length;
        
            while (true)
            {
                if (tpPos + sizeof(uint) > tpSize)
                {
                    CI.LogErrorFormat("Load-ThirdParty-dll tpPos={0} tpSize={1}", tpPos, tpSize);
                    break;
                }

                uint headLength = BitConverter.ToUInt32(tpDllBytes, tpPos);
                tpPos += sizeof(uint);
            
                if (tpPos + headLength > tpSize)
                {
                    CI.LogErrorFormat("Load-ThirdParty-dll tpPos={0} tpSize={1} headLength={2}", tpPos, tpSize, headLength);
                    break;
                }
                
                byte[] tpBysCur = new byte[headLength];
                
                Array.Copy(tpDllBytes, tpPos, tpBysCur, 0, headLength);
                tpPos += (int)headLength;
                
                HomologousImageMode mode = HomologousImageMode.SuperSet;
                LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(tpBysCur, mode);
                CI.Log($"LoadMetadataForAOTAssembly:{headLength}. mode:{mode} ret:{err}");

                if (tpPos >= tpSize)
                {
                    break;
                }
            }
            
            // Load-EngineGame-dll
            var egDllNameMain = "mxj2.bin";
            var egDllNameMainNew = egDllNameMain;
            if (dictManifest.ContainsKey(egDllNameMain))
            {
                egDllNameMainNew = dictManifest[egDllNameMain];
            }
            var egDllPath = string.Format("mxj/{0}", egDllNameMainNew);
            byte[] egDllBytes = null;
            
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                var PreLoadEGDll = new RetryHttpGetUnit();
                PreLoadEGDll.url = GetGameDataPath(egDllPath);
                StartCoroutine(RetryHttpGet(PreLoadEGDll));
                while (PreLoadEGDll.loadType != EN_LOAD_TYPE.LOADED)
                {
                    yield return null;
                }
                egDllBytes = PreLoadEGDll.bys;
            }
            else
            {
                egDllBytes = ReadFileAllBytes(egDllPath);
            }

            if (egDllBytes == null)
            {
                CI.LogError("Assembly Game bytes Is Null!");
                yield break;
            }
            
            assembly = Assembly.Load(egDllBytes);
            
            egDllNameMain = "mxj1.bin";
            egDllNameMainNew = egDllNameMain;
            if (dictManifest.ContainsKey(egDllNameMain))
            {
                egDllNameMainNew = dictManifest[egDllNameMain];
            }
            egDllPath = string.Format("mxj/{0}", egDllNameMainNew);
            egDllBytes = null;
            
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                var PreLoadEGDll = new RetryHttpGetUnit();
                PreLoadEGDll.url = GetGameDataPath(egDllPath);
                StartCoroutine(RetryHttpGet(PreLoadEGDll));
                while (PreLoadEGDll.loadType != EN_LOAD_TYPE.LOADED)
                {
                    yield return null;
                }
                egDllBytes = PreLoadEGDll.bys;
            }
            else
            {
                egDllBytes = ReadFileAllBytes(egDllPath);
            }

            if (egDllBytes == null)
            {
                CI.LogError("Assembly Game bytes Is Null!");
                yield break;
            }
            
            assembly = Assembly.Load(egDllBytes);
            assemblySize = egDllBytes.Length;
            assemblyMD5 = CI.GetMd5Str(egDllBytes);
        }
        else
        {
            // 重新登录
            var bEngineGameChange = false;

            // Load-EngineGame-dll
            var egDllNameMain = "eg1.bin";
            var egDllNameMainNew = egDllNameMain;
            if (dictManifest.ContainsKey(egDllNameMain))
            {
                egDllNameMainNew = dictManifest[egDllNameMain];
            }
            var egDllPath = string.Format("mxj/{0}", egDllNameMainNew);
            byte[] egDllBytes = null;
            
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                var PreLoadEGDll = new RetryHttpGetUnit();
                PreLoadEGDll.url = GetGameDataPath(egDllPath);
                StartCoroutine(RetryHttpGet(PreLoadEGDll));
                while (PreLoadEGDll.loadType != EN_LOAD_TYPE.LOADED)
                {
                    yield return null;
                }
                egDllBytes = PreLoadEGDll.bys;
            }
            else
            {
                egDllBytes = ReadFileAllBytes(egDllPath);
            }

            if (egDllBytes == null)
            {
                CI.LogError("ReLogin Assembly Game bytes Is Null!");
                yield break;
            }

            if (egDllBytes.Length != assemblySize || !assemblyMD5.Equals(CI.GetMd5Str(egDllBytes)))
            {
                bEngineGameChange = true;
            }

            if (bEngineGameChange)
            {
            	string strBtnOK = GetStringByKey(KEY_YES);
                string strContent = GetStringByKey(KEY_UPDATERESENGINEGAMECHANGE);
                ShowSingleTipWindows(strContent, strBtnOK, OnConfirmExitApp);
                yield break;
            }
        }
#endif

        if (assembly == null)
        {
            var assembliesInCurrentDomain = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var item in assembliesInCurrentDomain)
            {
                if (item.GetName().Name.ToLower().Equals("engine"))
                {
                    assembly = item;
                    break;
                }
            }
        }

        if (assembly == null)
        {
            CI.LogError("Assembly Game Is Null!");
            yield break;
        }

        Type typeGameManager = assembly.GetType("GameManager");

        if (typeGameManager != null)
        {
            MethodInfo mi = typeGameManager.GetMethod("CreateNew", new Type[] { typeof(string), typeof(int), typeof(string), typeof(string), typeof(bool) });
            string param1 = GetResVersion();
            int param2 = 0;
            string param3 = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                m_strReviewAppVersion,
                m_strAppStoreURL,
                m_strNewsURL,
                m_strChannelSwitchURL,
                strConfURL,
                strUpdateURL,
                STR_WEBGLCDN_URL,
                GetStringByKey(KEY_LOADRESFAIL));
            string param4 = m_strServerInfo;
            mi.Invoke(null, new object[] { param1, param2, param3, param4, isBigVersion });
        }
    }

    private void RequestQuitGame()
    {
        CI.Log("ready.RequestQuitGame");

        if (CI.IsChannelHasExitDialog())
        {
            // 调用渠道退出框
            ThirdPartyWrapper.Instance.QuitGame();
        }
        else
        {
            if (!bShowQuitGameUI)
            {
                // 显示游戏退出框
                string strBtnOK = GetStringByKey(KEY_YES);
                string strBtnCancel = GetStringByKey(KEY_NO);
                string strContent = GetStringByKey(KEY_QUITGAME);
                bShowQuitGameUI = true;
                ShowDoubleTipWindows(strContent, strBtnOK, strBtnCancel, OnConfirmQuitGame, OnCancelQuitGame);
            }
            else
            {
                OnCancelQuitGame();
            }
        }
    }

    private void OnConfirmQuitGame()
    {
        bShowQuitGameUI = false;
        this.HideTip();
        CI.ApplicationQuit();
        YooAssets.Destroy();
    }

    private void OnCancelQuitGame()
    {
        bShowQuitGameUI = false;
        this.HideTip();
    }

    #region 假进度相关

    private bool isStartProgress = false; //是否开始模拟进度
    private float flaseProgressValue = 0; //当前模拟进度值
    private Coroutine flaseProgressCoroutine; //模拟进度协程实例
    private Coroutine delayHideLoadingCoroutine;
    private float loadingPrecentMax = 1.0f;
    private float loadingSpeed = 0.5f;
    private bool bSlowSpeedMode = false;

    private float GetLoadingSpeed()
    {
        if (bSlowSpeedMode)
        {
            return loadingSpeed * 0.7f;
        }

        return loadingSpeed;
    }

    private void ShowProgress(int step)
    {
        m_engineLauncherResUI.SetVisibleProgress(true);

        isStartProgress = true;

        if (step == 1)
        {
            flaseProgressValue = 0;
            loadingPrecentMax = 0.5f;
            //var accountCur = BaseUtil.LoadDecryptString(CATSOUPACCOUNTID, "");
            //bSlowSpeedMode = string.IsNullOrEmpty(accountCur);
        }
        else if (step == 2)
        {
            loadingSpeed = 0.5f;
            loadingPrecentMax = 0.5f;
        }
        else if (step == 3)
        {
            loadingSpeed = 0.75f;
            loadingPrecentMax = 1.0f;
        }

        StartFalseProgressCoroutine();
    }

    private void HideProgress()
    {
        isStartProgress = false;
        m_engineLauncherResUI.SetVisibleProgress(false);
        StopFalseProgressCoroutine();
    }

    /// <summary>
    /// set_login 界面SendMessage 调用该方法
    /// 设置进度并且延迟停止假进度条
    /// </summary>
    /// <param name="value">当前进度</param>
    public void DelayHideLoading(object value)
    {
        float proValue = float.Parse(value.ToString());
        SetReadyProgressValue(proValue);
        if (delayHideLoadingCoroutine != null)
        {
            StopCoroutine(delayHideLoadingCoroutine);
            delayHideLoadingCoroutine = null;
        }

        delayHideLoadingCoroutine = this.StartCoroutine(StartDelayHideLoading());
    }

    /// <summary>
    /// 开始延迟0.2秒 隐藏转圈圈
    /// </summary>
    /// <returns></returns>
    IEnumerator StartDelayHideLoading()
    {
        yield return new WaitForSeconds(0.2f);
        HideUILoading();
    }

    /// <summary>
    /// set_login 界面SendMessage 调用该方法
    /// </summary>
    public void ForceHideLoading()
    {
        HideProgress();
    }

    private IEnumerator StartLoadingPrecent()
    {
        while (isStartProgress)
        {
            var speed = GetLoadingSpeed();

            if ((flaseProgressValue / loadingPrecentMax) >= 0.9f)
            {
                speed /= 10.0f;
            }

            flaseProgressValue = Mathf.MoveTowards(flaseProgressValue, loadingPrecentMax, Time.deltaTime * speed);
            SetReadyProgressValue(flaseProgressValue);
            yield return null;
            if (flaseProgressValue >= (loadingPrecentMax - 0.01f))
            {
                break;
            }
        }
    }

    /// <summary>
    /// set_login 界面SendMessage 调用该方法
    /// </summary>
    /// <param name="value"></param>
    public void SetReadyProgressMax(object value)
    {
        loadingPrecentMax = float.Parse(value.ToString());
    }

    /// <summary>
    /// set_login 界面SendMessage 调用该方法
    /// </summary>
    /// <param name="value"></param>
    public void SetReadyProgressSpeed(object value)
    {
        this.loadingSpeed = float.Parse(value.ToString());
    }

    /// <summary>
    /// set_login 界面SendMessage 调用该方法
    /// </summary>
    /// <param name="value"></param>
    public void SetReadyProgressValue(object value)
    {
        flaseProgressValue = float.Parse(value.ToString());
        m_engineLauncherResUI.TipUpdateProgress(flaseProgressValue * 100.0f);
        m_engineLauncherResUI.TipStep(string.Format("{0}%", (flaseProgressValue * 100.0f).ToString("#.#")));
    }

    private void StartFalseProgressCoroutine()
    {
        StopFalseProgressCoroutine();
        flaseProgressCoroutine = this.StartCoroutine(StartLoadingPrecent());
    }

    /// <summary>
    /// 停止假进度条协程
    /// </summary>
    private void StopFalseProgressCoroutine()
    {
        if (flaseProgressCoroutine != null)
        {
            StopCoroutine(flaseProgressCoroutine);
            flaseProgressCoroutine = null;
        }
    }

    #endregion

    /// <summary>
    /// 获取资源服务器地址
    /// </summary>
    private string GetHostServerURL()
    {
        // string hostServerIP = "https://pandoo.obs.cn-east-3.myhuaweicloud.com"; //国内
        // string hostServerIP = "http://game.fenkuan.cc"; //国内
        string hostServerIP = "http://build.fzsk.cc";
        // string hostServerIP = "https://fkpd.obs.ap-southeast-3.myhuaweicloud.com";//海外
        string appVersion = GetAppVersion();
        
        // return $"{hostServerIP}/CDN/Android/DefaultPackage/20250314124532";

#if !UNITY_EDITOR && PF_TAPTAPSDK
        return $"{hostServerIP}/CDN/Android/Public/{appVersion}";
#elif !UNITY_EDITOR && UNITY_ANDROID
        return $"{hostServerIP}/CDN/Android/{appVersion}";
#elif !UNITY_EDITOR && UNITY_IOS
        return $"{hostServerIP}/CDN/IPhone/{appVersion}";
#elif !UNITY_EDITOR && UNITY_WEBGL
        return $"{hostServerIP}/CDN/WebGL/{appVersion}";
#else
        return $"{hostServerIP}/CDN/PC/{appVersion}";
#endif
    }
}