using EngineBase;
using Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using FairyGUI;
using msg;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using EventDispatcher = EngineBase.EventDispatcher;
using Random = UnityEngine.Random;
#if !UNITY_WEBGL
using System.IO;
#endif

public class GameManager : MonoBehaviour
{
    #region ConstDefine

    public enum LoginState
    {
        NONE = 0,
        LoginCheckServer = 1,
        LoginCheckServerDone = 2,
        LoginPlayerServer = 3,
        LoginPlayerServerDone = 4,
        LoginQueryData = 5,
        LoginQueryDataSuccess = 6,
    }
    
    public WaitForSeconds wait1 = new WaitForSeconds(0.1f);
    public WaitForSeconds wait2 = new WaitForSeconds(0.2f);
    public WaitForSeconds wait3 = new WaitForSeconds(0.3f);
    public WaitForSeconds wait5 = new WaitForSeconds(0.5f);
    public WaitForSeconds waitSec1 = new WaitForSeconds(1);
    public WaitForSeconds wait15 = new WaitForSeconds(1.5f);
    public WaitForSeconds waitSec2 = new WaitForSeconds(2);
    public WaitForSeconds wait25 = new WaitForSeconds(2.5f);
    public WaitForSeconds waitSec3 = new WaitForSeconds(3);
    public WaitForSeconds waitSec4 = new WaitForSeconds(4);
    public WaitForSeconds waitSec5 = new WaitForSeconds(5);
    public WaitForSeconds waitSec6 = new WaitForSeconds(6);
    public WaitForSeconds waitSec24 = new WaitForSeconds(24);
    public WaitForSeconds waitMin1 = new WaitForSeconds(60);
    public WaitForEndOfFrame waitEndFrame = new WaitForEndOfFrame();

    public static byte LOCALIZATION_ENCRYPT_KEY = 0xfa;  // no encrypt set value 0x0

    #endregion

    // public bool IsStopBattle = false;
    public bool Pause = false;
    public bool OnAppquit = false;
    public ConfigServerUnit CurServerUnit;
    public ulong CurUserId;
    public string CurUserName;
    public bool isAppBackPause = false;  //是否进入后台
    #region ManagerObj
    private static GameManager _instance = null;
    public static GameManager Instance { get { return _instance; } }

    private ModelManager modelManager;

    public ModelManager ModelManager
    {
        get
        {
            if (this.modelManager == null)
                this.modelManager = new ModelManager();
            return this.modelManager;
        }
    }

    private TimerManagerEx timerManager;

    public TimerManagerEx TimerManager
    {
        get
        {
            if (this.timerManager == null)
            {
                this.timerManager = new TimerManagerEx();
                this.timerManager.Log = false;
            }
            return this.timerManager;
        }
    }

    private SoundManager _soundManager;
    public SoundManager SoundManager
    {
        get
        {
            if (_soundManager == null)
                this._soundManager = new SoundManager();
            return _soundManager;
        }
    }


    protected MapContext context;

    public MapContext Context
    {
        get { return context; }
        set { context = value; }
    }

    public Connection Connection { get; private set; }

    [HideInInspector]
    public int _languageIndex = 0;
    public int languageIndex { get { return _languageIndex; } private set { _languageIndex = value; } }
    private const string Language_Index = "languageIndex";
    public byte[] PreLoadLocalizationBytes { get; private set; }
    
    [HideInInspector]  //TODO 语言列表 后期添加新语言
    public List<int> languageList = new List<int>{
        defLanguage.English, 
        defLanguage.Korean, 
        defLanguage.Japanese, 
        defLanguage.ChineseSimplified,
        defLanguage.ChineseTraditional
    };

    //首次登录进入大地图
    // public int _isFirstLogin = 0;//0代表是，1代表否
    // public int isFirstLogin { get { return _isFirstLogin; } private set { _isFirstLogin = value; } }
    // private string IsFirstLogin = "isFirstLogin";//第一次登录游戏时直接进入大地图
    
    //记录玩家下线时的场景，下次登录显示该场景：大地图场景、非大地图场景时显示战斗界面
    // public int _downLineScene = 0;//0代表大地图场景，1代表非大地图场景
    // public int downLineScene { get { return _downLineScene; } set { _downLineScene = value; UnityEngine.PlayerPrefs.SetInt(DownLineScene, value); } }
    // private string DownLineScene = "downLineScene";
    
    //玩家下线前最后的位置
    // public Vector2 _downLineHeroPosition = Vector2.zero;
    public string DownLineHeroPosX = "downLineHeroPositionX";
    public string DownLineHeroPosY = "downLineHeroPositionY";
    // public Vector2 downLineHeroPosition { 
    //     get { return _downLineHeroPosition; }
    //     set
    //     {
    //         _downLineHeroPosition = value;
    //         //保存到本地内存
    //         UnityEngine.PlayerPrefs.SetFloat(DownLineHeroPosX, value.x);
    //         UnityEngine.PlayerPrefs.SetFloat(DownLineHeroPosY, value.y);
    //     }
    // }

    //保持英雄的最终位置
    public void SaveHeroPosition(Vector2 position)
    {
        // downLineHeroPosition = position;
        // UnityEngine.PlayerPrefs.Save();
        
        // 获取当前用户的UID
        ulong currentUid = GameManager.Instance.ServerAccount;
        string uidKey = currentUid.ToString();
        
        string posXKey = GameManager.Instance.DownLineHeroPosX + uidKey;
        string posYKey = GameManager.Instance.DownLineHeroPosY + uidKey;
        
        UnityEngine.PlayerPrefs.SetFloat(posXKey, position.x);
        UnityEngine.PlayerPrefs.SetFloat(posYKey, position.y);
        UnityEngine.PlayerPrefs.Save();
    }
    
    // 当玩家进入新章节时,清理数据
    public void OnNewChapterStart()
    {
        // GameManager.Instance.downLineHeroPosition = Vector2.zero;
        // UnityEngine.PlayerPrefs.DeleteKey(DownLineHeroPosX);
        // UnityEngine.PlayerPrefs.DeleteKey(DownLineHeroPosX);
        
        ulong currentUid = ServerAccount;
        string uidKey = currentUid.ToString();
        
        string posXKey = DownLineHeroPosX + uidKey;
        string posYKey = DownLineHeroPosY + uidKey;
        
        UnityEngine.PlayerPrefs.DeleteKey(posXKey);
        UnityEngine.PlayerPrefs.DeleteKey(posYKey);
        
    }

    private EN_LOAD_TYPE PreLoadModelManifestState { get; set; } = EN_LOAD_TYPE.INIT;
    
    private string ServerUrl { get; set; } = "";
    private string ServerName { get; set; } = "";
    private string ServerPayUrl { get; set; } = "";
    private string ServerUrlReview { get; set; } = "";
    private string ServerNameReview { get; set; } = "";
    private string ServerPayUrlReview { get; set; } = "";

    // public string CurServerUrl
    // {
    //     get
    //     {
    //         return VersionManager.Instance.IsReviewStoreVersion() ? ServerUrlReview : ServerUrl;
    //     }
    // }

    // public string CurServerName
    // {
    //     get
    //     {
    //         return VersionManager.Instance.IsReviewStoreVersion() ? ServerNameReview : ServerName;
    //     }
    // }
    //
    // public string CurServerPayUrl
    // {
    //     get
    //     {
    //         return VersionManager.Instance.IsReviewStoreVersion() ? ServerPayUrlReview : ServerPayUrl;
    //     }
    // }

    public string AppStoreURL { get; private set; } = "";
    public string ConfURL { get; private set; } = "";
    public string UpdateURL { get; private set; } = "";
    public string NewsURL { get; private set; } = "";
    public string ChannelSwitchURL { get; private set; } = "";
    public string WebGLCDNURL { get; private set; } = "";
    public int ServerId { get; private set; }
    
    public string DefaultPackageName = "DefaultPackage";

    #endregion
    
    #region Path
    public bool GetABFilePath(string strFileName, ref string strFilePath)
    {
        //LogUtils.LogWarning($"GetABFilePath strFileName：{strFileName}, strFilePath:{strFilePath}");
        
#if UNITY_WEBGL && !UNITY_EDITOR
        strFilePath = strFileName;
        return false;
#else
        strFilePath = strFileName;

        var package = YooAssets.TryGetPackage(DefaultPackageName);
        if (package == null)
        {
            LogUtils.LogError("GetABFilePath package == null!");
            return false;
        }
        
        //strFileName = strFileName.Replace("//", "/");
        var location = "Assets/ArchivedBundles" + strFileName;
        var handle = package.LoadRawFileSync(location);
        if (handle.Status == EOperationStatus.Succeed)
        {
            //LogUtils.LogWarning($"GetABFilePath location：{location}, strFilePath：{strFilePath}");
            
            strFilePath = handle.GetRawFilePath();
            return true;
        }
        else
        {
            return false;
        }
#endif
    }

    public byte[] ReadFileAllBytes(string strResPath, bool bRootPath = false)
    {
        //LogUtils.LogWarning($"ReadFileAllBytes strResPath：{strResPath}");
        
#if UNITY_EDITOR
        var strPath = Application.dataPath + "/ArchivedBundles/" + strResPath;

        if (File.Exists(strPath))
        {
            return File.ReadAllBytes(strPath);
        }

        return null;
#else
        var package = YooAssets.TryGetPackage(DefaultPackageName);
        if (package == null)
        {
            Debug.LogError("ReadFileAllBytes package == null!");
            return null;
        }
                
        var location = "Assets/ArchivedBundles/" + strResPath;
        //LogUtils.LogWarning($"ReadFileAllBytes location：{location}");
        
        var handle = package.LoadRawFileSync(location);
        if (handle.Status == EOperationStatus.Succeed)
            return handle.GetRawFileData();
        else
            return null;
#endif
    }
    #endregion
        
    #region UIObj
    public int screenWidth { get; private set; } = SCREEN.WIDTH;
    public int screenHeight { get; private set; } = SCREEN.HEIGHT;

    float screenRatioFactor = 1.0f;
    float screenRatioFactor_real = 1.0f;
    float outLineSize = 0.01f;

    private int adjustWidth;
    private int adjustHeight;

    #endregion

    #region Login
    // public string UserName { get; private set; }
    public ulong ServerAccount { get; private set; }
    public string ServerToken { get; private set; }
    private Coroutine playLoginCoroutine { get; set; }
    public int ServerBaseTime { get; private set; } = 0;
    public int ServerStartTime { get; private set; } = 0;
    public int HttpServerNowTime()
    {
        return UnbiasedTime.GetLocalTimeStamp() - this.ServerStartTime + this.ServerBaseTime;
    }
    public LoginState UserLoginState { get; private set; } = LoginState.NONE;

    public string socialID { get; private set; } = "";
    public string GetSocialID() { return socialID; }

    #endregion

    void Awake()
    {
    }
    
    public static void CreateNew(string resVersion, int nFullPlatform, string param, string serverParam, bool isBigVersion)
    {
        if (_instance == null)
        {
            var go = new GameObject("GameManager");
            _instance = go.AddComponent<GameManager>();
            GameObject.DontDestroyOnLoad(go);
        }
        _instance.StartCoroutine(_instance.StartApp(resVersion, nFullPlatform, param, serverParam, isBigVersion));
    }

    /// <summary>
    /// 初始化实例   인스턴스 초기화를 위해서 호출하는 것으로 추측. 
    /// </summary>
    public IEnumerator StartApp(string resVersion, int nFullPlatform, string param, string serverParam, bool isBigVersion)
    {
        LogUtils.LogWarningFormat("StartApp {0} {1} {2} {3}", resVersion, nFullPlatform, param, serverParam);
        
        string AppReviewVersion = "";

        if (!string.IsNullOrEmpty(param))
        {
            string[] strAppConf = param.Split(',');

            AppReviewVersion = Utils.GetString(strAppConf, 0);
            AppStoreURL = Utils.GetString(strAppConf, 1);
            NewsURL = Utils.GetString(strAppConf, 2);
            ChannelSwitchURL = Utils.GetString(strAppConf, 3);
            ConfURL = Utils.GetString(strAppConf, 4);
            UpdateURL = Utils.GetString(strAppConf, 5);
            WebGLCDNURL = Utils.GetString(strAppConf, 6);
        }

        if (!string.IsNullOrEmpty(serverParam))
        {
            string[] strAppConf = serverParam.Split(';');

            var ServerID = Utils.GetString(strAppConf, 0);
            ServerName = Utils.GetString(strAppConf, 1);
            ServerUrl = Utils.GetString(strAppConf, 2);
            ServerPayUrl = Utils.GetString(strAppConf, 3);
            var ServerIDReview = Utils.GetString(strAppConf, 4);
            ServerNameReview = Utils.GetString(strAppConf, 5);
            ServerUrlReview = Utils.GetString(strAppConf, 6);
            ServerPayUrlReview = Utils.GetString(strAppConf, 7);
        }
 
        GRoot.inst.SetContentScaleFactor(720, 1280/*, UIContentScaler.ScreenMatchMode.MatchWidth*/);
        
        UIGLoaderManager.Instance.Init();
        
        UIObjectFactory.SetLoaderExtension(typeof(UIGLoader));

        UIConfig.defaultFont = "MISANS-BOLD";
        UIConfig.enhancedTextOutlineEffect = true;
        
        EngineBase.BaseUtil.readFileAllBytes = ReadFileAllBytes;
        EngineBase.BaseUtil.getABFilePath = GetABFilePath;
        
        VersionManager.Instance.UpdateResVersion(AppReviewVersion, resVersion, WebGLCDNURL);

        SDKInterface.CreateNew(nFullPlatform);
        
#if !UNITY_EDITOR && PF_TAPTAPSDK
        if (isBigVersion)
        {
            SDKInterface.Instance.SDKUpdateGame();
            yield break;
        }
#endif
        
        InitLanguage();
        
        ConstDefine.Init();

        UnityEngine.Input.multiTouchEnabled = true;
        GameManager.Instance.SoundManager.musicVolume = !EngineBase.PlayerPrefs.HasKey("music") ?  1 : EngineBase.PlayerPrefs.GetFloat("music");
        GameManager.Instance.SoundManager.soundVolume = !EngineBase.PlayerPrefs.HasKey("sound") ? 1 : EngineBase.PlayerPrefs.GetFloat("sound");
        
#if UNITY_WEBGL && !UNITY_EDITOR
        DataAnalyticsWrapper.Instance.LoadingLocalizationStart(); //打点，加载多语言资源开始

        var PreLoadLocalization = new CRetryHttpGetUnit();
        PreLoadLocalization.url = VersionManager.Instance.GetGameDataPath(string.Format("localization/manifest.ress"));
        PreLoadLocalization.retryTip = string.Format("{0}({1})", readyErrorTip, "Localization");
        StartCoroutine(RetryHttpGet(PreLoadLocalization));
        while (PreLoadLocalization.loadType != EN_LOAD_TYPE.LOADED)
        {
            yield return null;
        }
        
        if (PreLoadLocalization.bys == null)
        {
            Debug.LogError("CriticalError \tLocalization Manifest bytes Is Null!");
            yield break;
        }
        
        var strLoadLocalization = System.Text.Encoding.UTF8.GetString(PreLoadLocalization.bys);
        strLoadLocalization = strLoadLocalization.Replace("\r", "");
        var lines = strLoadLocalization.Split('\n');
        var localizationFile = "";
        foreach (var line in lines) 
        {
            var spl = line.Split(',');

            if (spl.Length >= 2 && spl[0].Equals(GameExtensions.LanguageKey(languageIndex)))
            {
                localizationFile = spl[1];
                break;
            }
        }
        
        if (string.IsNullOrEmpty(localizationFile))
        {
            Debug.LogError("CriticalError \tLocalizationFile Path Is Null!");
            yield break;
        }
        
        var PreLoadLocalizationFile = new CRetryHttpGetUnit();
        PreLoadLocalizationFile.url = VersionManager.Instance.GetGameDataPath(string.Format("localization/{0}.bin", localizationFile));
        PreLoadLocalizationFile.retryTip = string.Format("{0}({1})", readyErrorTip, "LocalizationFile");
        StartCoroutine(RetryHttpGet(PreLoadLocalizationFile));
        while (PreLoadLocalizationFile.loadType != EN_LOAD_TYPE.LOADED)
        {
            yield return null;
        }
        
        if (PreLoadLocalizationFile.bys == null)
        {
            Debug.LogError("CriticalError \tLocalizationFile bytes Is Null!");
            yield break;
        }
        
        var bysNew = new byte[PreLoadLocalizationFile.bys.Length];
        Array.Copy(PreLoadLocalizationFile.bys, bysNew, PreLoadLocalizationFile.bys.Length);
        Utils.XORBytes(LOCALIZATION_ENCRYPT_KEY, ref bysNew);
        PreLoadLocalizationBytes = bysNew;
        
        DataAnalyticsWrapper.Instance.LoadingLocalizationDone(); //打点，加载多语言资源完成
#endif
        
        NetManager.Instance.OnInit();
        
        UIManager.Instance.OnInit();

        SystemAdapter.InitSystemInfo();
        
        SceneHelper.Instance.OnInit();//在NetManager.OnInit后

        UIRedsManager.Instance.OnInit();//后续可以移动到EnterGame后

        InitUI();

        ModelManager.Init();
        TimerManager.Init();
        SoundManager.Init();

        // 初始化资源清单
        PreLoadModelManifestState = EN_LOAD_TYPE.INIT;

        while (true)
        {
            if (PreLoadModelManifestState == EN_LOAD_TYPE.INIT)
            {
                // 开始加载
                LoadModelManifest();
            }
            else if (PreLoadModelManifestState == EN_LOAD_TYPE.LOADED)
            {
                break;
            }
            else if (PreLoadModelManifestState == EN_LOAD_TYPE.LOADERROR)
            {
                // 提示玩家错误，等待玩家操作继续加载
                PreLoadModelManifestState = EN_LOAD_TYPE.LOADWAITCONTINUE;
                /*ShowReadyTipWindows(readyErrorTip, () =>
                {
                    // 开始加载
                    HideReadyTipWindows();
                    LoadModelManifest();
                });*/
            }
            
            yield return null;
        }
        
        // 加载公有资源，将来需要移动到合适的时机（例如登录过程中）
        LogUtils.Log("LoadCommonRes");

        yield return null;

        SystemAdapter.EnableHighLoadingMode(true);

        ModelManager.Instance.LoadCommonRes();

        while (ModelManager.Instance.CommonResLoaded == false)
        {
            yield return null;
        }
        
        LogUtils.Log("LoadImportRes");
        
        AdapterManager.CleanUp();
        AdapterManager.CreateNew();

        while (EN_LOAD_TYPE.LOADED != AdapterManager.LoadType)
        {
            if (AdapterManager.LoadType == EN_LOAD_TYPE.LOADERROR)
            {
                // 提示玩家错误，等待玩家操作继续加载
                AdapterManager.OnLoadWaitContinue();
                /*ShowReadyTipWindows(readyErrorTip, () =>
                {
                    HideReadyTipWindows();
                    
                    AdapterManager.CleanUp();
                    AdapterManager.CreateNew();
                });*/
            }
            
            yield return null;
        }

        yield return null;

        LogUtils.Log("Load CommonUI");

        var bLoadCommonUIRes = false;
        
        UIManager.Instance.AddPackageIfNot("Common", () =>
        {
            Common.CommonBinder.BindAll();
        });
        UIManager.Instance.AddPackageIfNot("CommonEx", () =>
        {
            SystemAdapter.EnableHighLoadingMode(false);
            CommonEx.CommonExBinder.BindAll();
            bLoadCommonUIRes = true;
        });

        while (!bLoadCommonUIRes)
        {
            yield return null;
        }
        
        LogUtils.Log("Load LoginUI");
        UIConfig.buttonSound = (NAudioClip)UIPackage.GetItemAsset("CommonEx", "TY_1");
        UIManager.Instance.ShowUIPanel("Login");
        
        EventDispatcher.GameWorld.Regist(EventDefine.STR_RECONNECT_SUCCESS, this.ReconncetGame);
    }

    private void ReconncetGame()
    {
        GameManager.Instance.Connection.ActiveClose();
        GameManager.Instance.Init(GameManager.Instance.CurServerUnit.IP, GameManager.Instance.CurServerUnit.Port);
        GameManager.Instance.PreLoginServer(GameManager.Instance.CurUserName, "");
        GameManager.Instance.LoginCheckServer();
        ItemInfoManager.Instance.Clear();
        EquipManager.Instance.Clear();
        ShopInfoManager.Instance.Clear();
        VillageInfoManager.Instance.IsGetServer = false;
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            isAppBackPause = true;
        }
        else
        {
            isAppBackPause = false;
            if (Connection != null)
            {
                Connection.SendHeartBeatTimely();
            }
        }
    }

    private void Update()
    {
        // this.CheckRefreshResolution();
        float deltaSeconds = Time.deltaTime;

        if (Connection != null)
        {
            Connection.Tick(deltaSeconds);
        }

        if (TimerManager != null)
        {
            TimerManager.Tick(deltaSeconds);
        }

        if (ModelManager != null)
        {
            ModelManager.Tick(deltaSeconds);
        }
        
        if (SoundManager != null)
        {
            SoundManager.Tick(deltaSeconds);
        }

        NetManager.Instance.Tick(deltaSeconds);

        UIManager.Instance.Tick(deltaSeconds);

        if (!Pause)
        {
            MapObjectManager.Instance.Tick(deltaSeconds);
            DungeonMapManager.Instance.Tick(deltaSeconds);
            MapVillagePetManager.Instance.Tick(deltaSeconds);
            PVPMapManager.Instance.Tick(deltaSeconds);
        }
        
        UIGLoaderManager.Instance.Tick(deltaSeconds);
        
        ServerTimeManager.Instance.Update(deltaSeconds);
    }

    private void LateUpdate()
    {
        float deltaSeconds = Time.deltaTime;

        if (ModelManager != null)
        {
            ModelManager.LateTick(deltaSeconds);
        }

        if (!Pause)
        {
            MapObjectManager.Instance.LateTick(deltaSeconds);
            MapVillagePetManager.Instance.LateTick(deltaSeconds);
        }
    }

    private void InitLanguage()
    {
        //TODO 暂时默认英文
        if (!UnityEngine.PlayerPrefs.HasKey(Language_Index))
        {
            languageIndex = defLanguage.ChineseSimplified;
        }
        else
        {
            // languageIndex = PlayerPrefs.GetInt(Language_Index, 0);// 默认英文
            languageIndex = UnityEngine.PlayerPrefs.GetInt(Language_Index, 3);// 默认中文
        }
        LaodLanguage();
    }

    //第一次登录游戏直接进入大地图
    private void InitFirstLogin()
    {
        // if (!UnityEngine.PlayerPrefs.HasKey(IsFirstLogin))
        // {
        //     // 首次登录：设置状态并保存标记
        //     isFirstLogin = 0;
        //     UnityEngine.PlayerPrefs.SetInt(IsFirstLogin, 1); // 保存"非首次"标记
        //     UnityEngine.PlayerPrefs.Save();
        // }
        // else
        // {
        //     // 非首次登录
        //     isFirstLogin = UnityEngine.PlayerPrefs.GetInt(IsFirstLogin, 1);
        // }
    }

    //记录下线场景
    private void InitDownLineScene()
    {
        // 从持久化存储加载记录，默认值0（大地图）
        // _downLineScene = UnityEngine.PlayerPrefs.GetInt(DownLineScene, 0);
    }

    //下线时英雄的位置
    // private void InitDownLineHeroPosition()
    // {
    //     // 加载保存的位置
    //     if (UnityEngine.PlayerPrefs.HasKey(DownLineHeroPosX))
    //     {
    //         _downLineHeroPosition = new Vector2(
    //             UnityEngine.PlayerPrefs.GetFloat(DownLineHeroPosX),
    //             UnityEngine.PlayerPrefs.GetFloat(DownLineHeroPosY)
    //         );
    //     }
    // }

    private void InitUI()
    {
        layer_gui = LayerMask.GetMask("UI");
        layer_nothing = 0;
        layer_focus = LayerMask.GetMask("focus");
        layer_unit = LayerMask.GetMask("unit");
        layer_ground = LayerMask.GetMask("ground");
        layer_object = LayerMask.GetMask("object");
        layer_ball = LayerMask.GetMask("ball");
        layer_outbound = LayerMask.GetMask("outbound");
    }

    private IEnumerator RetryHttpGet(CRetryHttpGetUnit unit)
     {
         if (unit == null)
         {
             LogUtils.LogError("RetryHttpGet Unit Is Null Error");
             yield break;
         }
         
         unit.loadType = EN_LOAD_TYPE.INIT;
 
         while (true)
         {
             if (unit.loadType == EN_LOAD_TYPE.INIT)
             {
                 // 开始加载
                 unit.loadType = EN_LOAD_TYPE.LOADING;
                 VersionManager.Instance.HttpGetOutputByte(this, unit.url,
                     (byte[] bys) =>
                     {
                         if (bys == null)
                         {
                             LogUtils.LogErrorFormat("CriticalError \tRetryHttpGet RootContent is Null {0}", unit.url);
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
                         LogUtils.LogErrorFormat("CriticalError \tRetryHttpGet Fail {0}", unit.url);
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
                 /*ShowReadyTipWindows(readyErrorTip, () =>
                 {
                     // 重新加载
                     HideReadyTipWindows();
                     unit.loadType = EN_LOAD_TYPE.INIT;
                 });*/
             }
             
             yield return null;
         }
     }
    
    private byte[] LoadLocalization(string path)
    {
#if UNITY_EDITOR
        var textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(
            string.Format("Assets/Editor Default Resources/Localization/{0}.txt", path));
        return textAsset != null ? textAsset.bytes : null;
#elif UNITY_WEBGL && !UNITY_EDITOR
        return PreLoadLocalizationBytes;
#else
        var bys = ReadFileAllBytes(string.Format("localization/{0}.bin", path));
        Utils.XORBytes(LOCALIZATION_ENCRYPT_KEY, ref bys);
        return bys;
#endif
    }

    public void LoadModelManifest()
    {
        LogUtils.Log("LoadModelManifest");

        PreLoadModelManifestState = EN_LOAD_TYPE.LOADING;
        
        this.modelManager.InitManifest(
            () =>
            {
                PreLoadModelManifestState = EN_LOAD_TYPE.LOADED;
            },
            () =>
            {
                PreLoadModelManifestState = EN_LOAD_TYPE.LOADERROR;
            });
    }
    
    public void InitLoginState()
    {
        UserLoginState = LoginState.NONE;
    }

    public void ChangeServerURL(string servername, string url)
    {
        ServerName = servername;
        ServerUrl = url;
        ServerNameReview = servername;
        ServerUrlReview = url;
    }

    public void LoginServer(string url, string accountLogin)
    {
    }

    public void LoginServerWithSDK(string url, string uin, string session)
    {
    }

    public void ReLoginServer()
    {
        LogUtils.LogWarning("ReLoginServer");
    }

    public bool PreLoginServer(string account, string token)
    {
        LogUtils.LogWarningFormat("PreLoginLobbyServer {0} {1}", account, account.GetHashCode());
        CurUserName = account;
        ServerToken = token;
        return true;
    }

    public void LoginCheckServer()
    {
        if (Connection == null)
        {
            Connection = new Connection(context);
            Connection.Init();
        }

        if (Connection.IsConnected())
        {
            Connection.ActiveClose();
        }

        UserLoginState = LoginState.LoginCheckServer;

        Connection.SetReconnectFlag(false);
        Connection.ChangeAddr(this.Context);
        Connection.Connect();
    }

    public void LoginServerFail(int code)
    {
        UserLoginState = LoginState.NONE;
        
        LogUtils.LogWarningFormat("LoginServerError {0}", code);
        
        DataAnalyticsWrapper.Instance.OnLoginHttpServerFail(code);
    }

    public void LobbyServerConnectComplete()
    {
        LogUtils.LogWarningFormat("LobbyServerConnectComplete UserLoginState:{0}", UserLoginState);
        if (UserLoginState <= LoginState.LoginCheckServer)
        {
            var msg = AccountCheck_CS.CreateBuilder();
            msg.SetUserName(CurUserName);
            msg.PublisherId = (int)ePublisher.ePublisher_None;
            msg.Platform = (int) ePlatorm.ePlatorm_Android;
            var data = msg.Build();
            this.Connection.SendMessage((int)eMsgID.eMsg_AccountCheck_CS, data);
        }
        else if (UserLoginState >= LoginState.LoginCheckServerDone)
        {
            LogUtils.LogFormat("PlayerLogin_CS.CreateBuilder()");
            UserLoginState = LoginState.LoginPlayerServer;
            var msg = PlayerLogin_CS.CreateBuilder();
            msg.SetUserId((ulong)ServerAccount);
            msg.SetCheckOutCode(ServerToken);
            var data = msg.Build();
            this.Connection.SendMessage((int)eMsgID.eMsg_PlayerLogin_CS, data);
        }

        /*var msg = TestMsg_Proto2_CS.CreateBuilder();
        msg.SetTestDouble(1.15645f);
        msg.SetTestFloat(1.2f);
        msg.SetTestInt32(11);
        msg.SetTestInt64(12);
        msg.SetTestUint32(101);
        msg.SetTestUint64(201);
        msg.SetTestBool(false);
        msg.SetTestString("gamelogin");
        msg.SetTestBytes(Google.ProtocolBuffers.ByteString.CopyFrom(new byte[]{ 0x30, 0x31}));
        var member = MemberTestAttr.CreateBuilder();
        member.SetAttrId(1);
        member.SetAttrName("gameattr");
        member.SetAttrValue(101);
        for (int i = 0; i < 5; i++)
        {
            member.AddAttrRepeatedId((uint)i*10);
            member.AddAttrRepeatedValue(i*5);
        }
        msg.SetTestMember(member);

        for (int j = 0; j < 5; j++)
        {
            var member2 = MemberTestAttr.CreateBuilder();
            member2.SetAttrId((uint)(1 + j));
            member2.SetAttrName("gameattr22222");
            member2.SetAttrValue(101 + j);
            for (int i = 0; i < 5; i++)
            {
                member2.AddAttrRepeatedId((uint)i);
                member2.AddAttrRepeatedValue(i);
            }

            msg.AddTestRepeatedMember(member2);
        }
        
        var data = msg.Build();
        this.Connection.SendMessage((int)eMsgID.eTestMsg_Proto2_CS, data);*/
    }

    public void OnAccountCheck(eErrCode code, string ip, int port, string token, ulong userId)
    {
        UserLoginState = LoginState.LoginCheckServerDone;
        ServerAccount = userId;
        string uid = ServerAccount.ToString();
        // IsFirstLogin = IsFirstLogin + uid;
        // InitFirstLogin();//首次登录进入大地图
        // DownLineScene = DownLineScene + uid;
        // InitDownLineScene();//记录下线场景
        DownLineHeroPosX = DownLineHeroPosX + uid;
        DownLineHeroPosY = DownLineHeroPosY + uid;
        // InitDownLineHeroPosition();//下线时英雄的位置
        if (code != eErrCode.eErrCode_Success)
        {
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_LOGIN_CHECK_FAIL);
            return;
        }
        
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_LOGIN_CHECK_SUCCESS);

        if (playLoginCoroutine != null)
        {
            StopCoroutine(playLoginCoroutine);
            playLoginCoroutine = null;
        }
        playLoginCoroutine = StartCoroutine(DoPlayerLogin(ip, port, token));
    }

    private IEnumerator DoPlayerLogin(string ip, int port, string token)
    {
        LogUtils.LogFormat("=============DoPlayerLogin:{0}   {1}", ip, port);

        if (this.context != null)
        {
            this.context.host = ip;
            this.context.port = port;
        }
        
        ServerToken = token;
        
        yield return null;
        
        if (this.Connection != null)
        {
            this.Connection.ActiveClose();
        }
        
        yield return null;
        
        if (this.Connection != null)
        {
            this.Connection.SetReconnectFlag(false);
            this.Connection.ChangeAddr(this.Context);
            this.Connection.Connect();
        }
    }
    
    public void OnPlayerLogin(eErrCode code)
    {
        if (code != eErrCode.eErrCode_Success)
        {
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_LOGIN_PLAYER_FAIL);
            return;
        }

        // 先直接成功
        UserLoginState = LoginState.LoginPlayerServerDone;
        var msg = LoginFinish_CS.CreateBuilder();
        var data = msg.Build();
        this.Connection?.SendMessage((int)eMsgID.eMsg_LoginFinish_CS, data);
        
        // 启动心跳
        if (this.Connection != null)
        {
            this.Connection.InitHeartBeat(15.0f);
        }
    }
    
    public void ActiveCloseLobbyServer()
    {
        if (this.Connection != null)
        {
            this.Connection.ActiveClose();
        }

        UserLoginState = LoginState.NONE;
    }

    public void DisconnectLobbyServer(string strTip = "")
    {
        //todo断开连接
        var reconnect = UIManager.Instance.FindByName("ReconnectWindow");
        if(reconnect != null && reconnect.IsShow())
            return;
        UIManager.Instance.ShowUIPanel("ReconnectWindow");
    }

    public void ReLoginGame()
    {
        ActiveCloseLobbyServer();

        if (SceneManager.GetActiveScene().name == "ready")
        {
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_LOGIN_RES_RESHOW);
        }
        else
        {
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_LOGIN_RES_STOPALL);

            MoveScene("ready", null, () =>
                {
                    GameManager.Instance.Cleanup();
                });
        }
    }

    /// <summary>
    /// 切换场景中
    /// </summary>
    public bool IsMoveScene { get; private set; }
    private Action moveSceneAction;

    /// <summary>
    /// Scene을 이동한다.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_finishAction">이동 후 실행할 Action</param>
    public void MoveScene(string _name, Action _finishAction, Action _loadDoneAction = null)
    {
        if (string.Equals(SceneManager.GetActiveScene().name, _name))
        {
            if (_loadDoneAction != null)
            {
                _loadDoneAction.Invoke();
                _loadDoneAction = null;
            }
            return;
        }

        IsMoveScene = true;

        // 加载场景
        StartCoroutine(OnMoveScene(_name, _finishAction, _loadDoneAction));
    }

    private IEnumerator OnMoveScene(string strScene, Action _finishAction, Action _loadDoneAction)
    {
        LogUtils.Log("OnMoveScene Start");

        Application.backgroundLoadingPriority = ThreadPriority.High;

        enableFakeAds = false;
        moveSceneAction = _finishAction;
        UIManager.Instance.SaveCommonUI();

        yield return null;

        if (strScene.Equals("ci") || strScene.Equals("ready"))
        {
            if (_loadDoneAction != null)
            {
                _loadDoneAction.Invoke();
                _loadDoneAction = null;
            }
            SceneManager.LoadScene(strScene, LoadSceneMode.Single);
            LightProbes.Tetrahedralize();
            yield break;
        }

#if UNITY_EDITOR
        if (!Utils.IsLoadModelFromAssetBundle())
        {
            if (_loadDoneAction != null)
            {
                _loadDoneAction.Invoke();
                _loadDoneAction = null;
            }
            SceneManager.LoadScene(strScene, LoadSceneMode.Single);
            LightProbes.Tetrahedralize();
            yield break;
        }
#endif

        LogUtils.Log("OnMoveScene Load");

        ModelManager.Instance.AsyncLoadBundle(MODEL_LOAD_TYPE.MODEL_LOAD_TYPE_SCENE, strScene,
            onComplete: (ModelLoad loader, AssetBundleInfo ab, UnityEngine.Object go) =>
            {
                LogUtils.Log("OnMoveScene LoadDone");

                Application.backgroundLoadingPriority = SystemAdapter.InitThreadPriority;

                if (ab == null)
                {
                    LogUtils.LogErrorFormat("CriticalError \tOnMoveScene Error Is Null {0}", strScene);
                    return;
                }

                try
                {
                    if (_loadDoneAction != null)
                    {
                        _loadDoneAction.Invoke();
                        _loadDoneAction = null;
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.LogException(ex);
                }
                
                try
                {
                    AsyncOperation op = SceneManager.LoadSceneAsync(strScene, LoadSceneMode.Single);

                    if (op != null)
                    {
                        op.completed += (AsyncOperation ope) =>
                        {
                            LightProbes.Tetrahedralize();

                            ModelManager.Instance.ClearModel(loader.strAssetBundle);
                        };
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.LogException(ex);
                }
            }, priority: true);
    }

    public bool IsGamePlayScene()
    {
        return IsStageScene();
    }

    public bool IsStageScene()
    {
        return SceneManager.GetActiveScene().name == "main";
    }

    public bool IsReadyScene()
    {
        return SceneManager.GetActiveScene().name == "ready";
    }

    /// <summary>
    /// 모든 코루틴을 멈춘다.
    /// </summary>
    public void CancelAllInvoke()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        OnAppquit = true;
        Cleanup();
    }

    public void Cleanup()
    {
        DataManager.Instance.ResetAllData();
        DataManager.Delete();
        
        if (SDKInterface.Instance != null)
        {
            SDKInterface.Instance.DisableExceptionHandler();
        }
        
        UIManager.Instance.Cleanup();
        
        NetManager.Instance.Cleanup();

        ShaderMgr.Instance.Cleanup();

        // AdapterManager.CleanUp();
        
        if (ModelManager != null)
        {
            ModelManager.Dispose();
        }

        if (TimerManager != null)
        {
            TimerManager.Dispose();
        }

        if (Connection != null)
        {
            Connection.Dispose();
            Connection = null;
        }

        // SDKInterface.CleanUp();

        SceneHelper.Instance.OnDispose();
    }

    public void Init(string ip, int port)
    {
        if (this.context == null)
        {
            this.context = new MapContext();
            this.context.host = ip;
            this.context.port = port;
            this.context.autoLogin = false;
            this.context.tipCode = 0;
        }
        else
        {
            this.context.host = ip;
            this.context.port = port;
        }

        // this.SetLanguage(languageIndex);
    }

    /// <summary>
    /// Scene초기화.
    /// </summary>
    public void InitScene()
    {
        LogUtils.Log("InitScene Start -- " + SceneManager.GetActiveScene().name);

        if (null != moveSceneAction)
        {
            moveSceneAction.Invoke();
            moveSceneAction = null;
        }

        IsMoveScene = false;

        if (null != coEnableFakeAds)
        {
            StopCoroutine(coEnableFakeAds);
        }

        var enu = FakeTimeCount();
        coEnableFakeAds = StartCoroutine(enu);
    }

    bool enableFakeAds = false;
    /// <summary>
    /// 광고 준비를 위해서 Fake 타임을 준다 - 6초.
    /// </summary>
    /// <returns></returns>
    IEnumerator FakeTimeCount()
    {
        // 불필요한 정보는 주석 처리
        // using var scope = new HideaCrashlyticsScope("FakeTimeCount");
        enableFakeAds = false;
        yield return waitSec6;
        enableFakeAds = true;
    }

    Coroutine coEnableFakeAds;

    public bool IsEnableFakeAds()
    {
        return enableFakeAds;
    }

    /// <summary>
    /// _max - 1까지의 수를 리턴한다.
    /// </summary>
    /// <param name="_max"></param>
    /// <returns></returns>
    public int GetRandom(int _max)
    {
        return Random.Range(0, _max);
    }

    public int GetRandomSeed()
    {
        var dateTimeNow = DateTime.Now;
        int sec = dateTimeNow.Second;
        int min = dateTimeNow.Millisecond;
        return sec * min;
    }

    public GameObject GetRootUI()
    {
        return UIManager.Instance.GetRootUI();
    }

    public float GetOutLineSize()
    {
        return outLineSize;
    }

    /// <summary>
    /// 시작 카메라 사이즈는 스크린 비율이 비례한다.
    /// </summary>
    /// <returns></returns>
    public float GetStartCamSize()
    {
        return screenRatioFactor * 2.5f;
    }
    
    // 获取获取操作系统语言
    void SetDefaultLanguage()
    {
        int index = SDKInterface.Instance.GetSystemLanguageIndex();

        languageIndex = index;
    }

    /// <summary>
    /// //检查语言索引是否有效。
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool IsPossibleLanguage(int value)
    {
        foreach (var index in languageList)
        {
            if (value == index)
            {
                return true;
                break;
            }
        }
        return false;
		// return true;
    }

    /// <summary>
    /// 设置语言包索引
    /// </summary>
    /// <param name="value"></param>
    public void SetLanguage(int value)
    {
        if (value == languageIndex) return;
        
        if (IsPossibleLanguage(value))
        {
            languageIndex = value;
            UnityEngine.PlayerPrefs.SetInt(Language_Index, value);

            var list = new List<string>();
            List<UIPackage> packageList = UIPackage.GetPackages();
            foreach (var item in packageList)
            {
                Debug.Log("========packageName=="+item.name);
                list.Add(item.name);
            }
            UIPackage.RemoveAllPackages();    // 卸载所有UI包（包括关联的语言数据）

            LaodLanguage();
            
            GTween.Clean();
            foreach (GObject child in GRoot.inst.GetChildren()) {
                Debug.Log(child.gameObjectName);
                UIViewBase view = UIManager.Instance.FindByName(child.gameObjectName);
                if (view != null)
                {
                    UIManager.Instance.DestroyController(view);
                }
                else
                {
                    child.Dispose();
                }
            }
            
            foreach (var assetPathName in list)
            {
                if (string.IsNullOrEmpty(assetPathName))
                {
                    Debug.Log("==assetPathName is null==");
                    continue;
                }
                //UIPackage.AddPackage(assetPathName); //加载AB包里面的UI资源包，参数必须是 AssetBundle
                Debug.Log("========Add packageName==" + assetPathName);
                UIManager.Instance.AddPackageIfNot(assetPathName, () =>{});
            }
            
            // UIManager.Instance.DestroyUIPanel("Lobby");

            MapObjectManager.Instance.Dispose();
            MapVillagePetManager.Instance.CleanUp();

            StopAllCoroutines();
            UIManager.Instance.ShowLoadingUI(() =>
            {
                MapObjectManager.Instance.OnInit();
                // 显示主界面
                UIManager.Instance.ShowUIPanel("Lobby");
            });
        }
        else
        {
            Debug.LogError("SetLanguage is nill, value="+value);
            // value = defLanguage.English;
        }
    }

    //TODO 加载UI语言包 后期添加其它语种
    public void LaodLanguage()
    {
        string name = GetLanguageTypeName();
        TextAsset textAsset = Resources.Load<TextAsset>("Language/"+name);
        if (textAsset)
        {
            string fileContent = textAsset.text;
            FairyGUI.Utils.XML xml = new FairyGUI.Utils.XML(fileContent);
            UIPackage.SetStringsSource(xml);
        }
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

    /// <summary>
    /// 获取语言名称
    /// </summary>
    /// /// <param name="languageId">语言Id</param>
    /// <param name="value">语言索引</param>
    /// <returns></returns>
    public string GetTextNameByIdWithIndex(string textId, int LanguageValue = -1)
    {
        if(LanguageValue == -1)
        {
            LanguageValue = GameManager.Instance.languageIndex;
        }
        string name = "";
        ConfigMultiLangUnit data = ConfigDataGroup.GetInstance<ConfigMultiLang>().Get(textId);
        if (data == null)
        {
            Debug.LogWarning("GetTextNameByIdWithIndex textId="+textId+", LanguageValue="+LanguageValue);
            return name;
        }
        switch (LanguageValue)  //TODO 后期添加其它语种
        {
            case defLanguage.English:
                name = data.EN;
                break;
            case defLanguage.Korean:
                name = data.KR;
                break;
            case defLanguage.Japanese:
                name = data.JP;
                break;
            case defLanguage.ChineseSimplified:
                name = data.CN;
                break;
            case defLanguage.ChineseTraditional:
                name = data.ZH;
                break;
        }
        return name;
    }

    public string GetStringCsLanguageById(int id)
    {
        string name = "";
        ConfigStringUnit data = ConfigDataGroup.GetInstance<ConfigString>().Get(id);
        if (data == null)
        {
            Debug.LogWarning("GetStringCsLanguageById  string.xlsx id=" + id);
            return name;
        }
        switch (languageIndex)  //TODO 后期添加其它语种
        {
            case defLanguage.English:
                name = data.EN;
                break;
            case defLanguage.Korean:
                name = data.KR;
                break;
            case defLanguage.Japanese:
                name = data.JP;
                break;
            case defLanguage.ChineseSimplified:
                name = data.CN;
                break;
            case defLanguage.ChineseTraditional:
                name = data.ZH;
                break;
        }
        return name;
    }

    public string GetPetQuotesCsLanguageById(int id)
    {
        string name = "";
        ConfigPetQuotesUnit data = ConfigDataGroup.GetInstance<ConfigPetQuotes>().Get(id);
        if (data == null)
        {
            Debug.LogWarning("GetStringCsLanguageById  PetQuotes.xlsx id=" + id);
            return name;
        }
        switch (languageIndex)  //TODO 后期添加其它语种
        {
            case defLanguage.English:
                name = data.EN;
                break;
            // case defLanguage.Korean:
            //     name = data.KR;
            //     break;
            // case defLanguage.Japanese:
            //     name = data.JP;
            //     break;
            case defLanguage.ChineseSimplified:
                name = data.CN;
                break;
            // case defLanguage.ChineseTraditional:
            //     name = data.ZH;
            //     break;
        }
        return name;
    }

    /// <summary>
    /// 기본 폰트는 0번을 사용한다.
    /// </summary>
    /// <param name="_font"></param>
    /// <returns></returns>
    public bool IsDefaultFont(Font _font)
    {
        return _font == GetDefaultFont();
    }

    /// <summary>
    /// 기본 폰트는 0번을 사용한다.
    /// </summary>
    /// <returns></returns>
    public Font GetDefaultFont()
    {
        return AdapterManager.Instance.GetDefaultFont();
    }

    /// <summary>
    /// 현재 언어 값에 맞는 폰트를 반환한다.ㄴ
    /// </summary>
    /// <returns></returns>
    public Font GetTrueTypeFont()
    {
        return GetTrueTypeFont(languageIndex);
    }

    /// <summary>
    /// 언어에 맞는 폰트를 반환한다.
    /// </summary>
    /// <param name="_index"></param>
    /// <returns></returns>
    public Font GetTrueTypeFont(int _index)
    {
        switch (_index)
        {
            case defLanguage.ChineseSimplified:
            case defLanguage.ChineseTraditional:
            case defLanguage.Russian:
            case defLanguage.Vietnamese:
            case defLanguage.Spanish:
            case defLanguage.Portuguese:
            case defLanguage.Indonesian:
            case defLanguage.German:
            case defLanguage.French:
            case defLanguage.Turkish:
            case defLanguage.Italian:
            case defLanguage.Arabic:
            case defLanguage.Japanese:
            case defLanguage.Thai:
                return AdapterManager.Instance.GetFont(0);
        }
        return AdapterManager.Instance.GetFont(1);
    }

    public Font GetTrueTypeFont(Font initFont)
    {
        if (!IsDefaultFont(initFont))
        {
            return GetTrueTypeFont();
        }

        return null;
    }

    public LayerMask layer_gui { get; private set; }
    public LayerMask layer_nothing { get; private set; }
    public LayerMask layer_focus { get; private set; }
    public LayerMask layer_unit { get; private set; }
    public LayerMask layer_ground { get; private set; }
    public LayerMask layer_object { get; private set; }
    public LayerMask layer_ball { get; private set; }
    public LayerMask layer_outbound { get; private set; }

    /// <summary>
    /// 텍스쳐 A와 B를 합친다.
    /// </summary>
    /// <param name="_a">원본 텍스쳐</param>
    /// <param name="_b">덮어씌우는 텍스쳐</param>
    /// <returns></returns>
    public Texture2D MergeTexture(Texture2D _a, Texture2D _b)
    {
        Texture2D finalTex = new Texture2D(_a.width, _a.height);
        Color[] colorArray = new Color[finalTex.width * finalTex.height];
        Color[][] srcArray = new Color[2][];

        srcArray[0] = _a.GetPixels();
        srcArray[1] = _b.GetPixels();

#if UNITY_EDITOR
        if (_a.width != _b.width)
        {
            RenderTexture rtDes = new RenderTexture((int)_a.width, (int)_a.height, 32);
            rtDes.enableRandomWrite = true;
            rtDes.Create();
        
            var ScaleImageComputeShader = UnityEditor.AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Editor/FuncComputeShader.compute");
            //1 找到compute shader中所要使用的KernelID
            int k = ScaleImageComputeShader.FindKernel("CSMain");
 
            ScaleImageComputeShader.SetTexture(k, "Source", _b);
            ScaleImageComputeShader.SetTexture(k, "Dst", rtDes);
            ScaleImageComputeShader.SetFloat( "widthScale", _a.width * 1.0f / _b.width);
            ScaleImageComputeShader.SetFloat( "heightScale", _a.height * 1.0f / _b.height);
       
            //3 运行shader  参数1=kid  参数2=线程组在x维度的数量 参数3=线程组在y维度的数量 参数4=线程组在z维度的数量
            ScaleImageComputeShader.Dispatch(k, (int)(_a.width),
                (int)(_a.height), 1);
       
            int width = rtDes.width;
            int height = rtDes.height;
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
            RenderTexture.active = rtDes;
            texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture2D.Apply();
            
            srcArray[1] = texture2D.GetPixels();
            
            rtDes.Release();
        }
#endif

        for (int x = 0; x < finalTex.width; ++x)
        {
            for (int y = 0; y < finalTex.height; ++y)
            {
                int pixelIndex = x + y * finalTex.width;
                for (int i = 0; i < 2; ++i)
                {
                    Color srcPixel = srcArray[i][pixelIndex];
                    if (srcPixel.a > 0.5f)
                        colorArray[pixelIndex] = srcPixel;
                }
            }
        }

        finalTex.SetPixels(colorArray);
        finalTex.Apply();

        finalTex.wrapMode = TextureWrapMode.Clamp;
        finalTex.filterMode = FilterMode.Bilinear;
        //finalTex.minimumMipmapLevel = 0;
        return finalTex;
    }

    public float GetAngle(Vector3 vStart, Vector3 vEnd)
    {
        return Quaternion.FromToRotation(Vector3.up, vEnd - vStart).eulerAngles.y;
    }

    Action linkFinishAction;
    public void OpenLinkURL(string _url, Action _finishAction = null)
    {
        linkFinishAction = _finishAction;
        
        var linkUrlTrim = _url.Trim();
        Application.OpenURL(linkUrlTrim);
        LogUtils.LogWarning($"[OpenUrl] linkUrl has a null character:{linkUrlTrim.Length != _url.Length}");
    }

    public void LinkFinishActionInvoke()
    {
        if (null != linkFinishAction)
        {
            linkFinishAction.Invoke();
            linkFinishAction = null;
        }
    }

    #region Spawn 에서 이동
    public enum Unit_BodyParts
    {
        Spine0,
        Spine1,
        Spine2,
        Tail,
        Tail_Root,
        Left_Hand,
        Right_Hand,
        Head,
    }

    public Transform GetUnit_BodyPartsTrans(Transform _body, Unit_BodyParts parts)
    {
        return parts switch
        {
            Unit_BodyParts.Spine0 => GetSpine0(_body),
            Unit_BodyParts.Spine1 => GetSpine1(_body),
            Unit_BodyParts.Spine2 => GetSpine2(_body),
            Unit_BodyParts.Tail => GetTail(_body),
            Unit_BodyParts.Tail_Root => GetTailRoot(_body),
            Unit_BodyParts.Left_Hand => GetLeftHand(_body),
            Unit_BodyParts.Right_Hand => GetRightHand(_body),
            Unit_BodyParts.Head => GetHead(_body),
            _ => _body,
        };
    }

    /// <summary>
    /// 오브젝트 이름이 Spine2이 맞지만 조작 해보니 위치는 대략 뒤쪽 어깨&등 정도 되는듯 함.
    /// </summary>
    /// <param name="_body"></param>
    /// <returns></returns>
    public Transform GetSpine2(Transform _body)
    {
        return _body.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Spine2");
    }

    /// <summary>
    /// 오브젝트 이름이 Spine1이 맞지만 조작 해보니 위치는 대략 허리춤 정도 되는듯 함.
    /// </summary>
    /// <param name="_body"></param>
    /// <returns></returns>
    public Transform GetSpine1(Transform _body)
    {
        return _body.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1");
    }

    /// <summary>
    /// 오브젝트 이름이 Spine이다. 조작 해보니 위치는 대략 하체 전체 정도 되는듯 함.
    /// </summary>
    /// <param name="_body"></param>
    /// <returns></returns>
    public Transform GetSpine0(Transform _body)
    {
        return _body.Find("Bip001/Bip001 Pelvis/Bip001 Spine");
    }

    public Transform GetTail(Transform _body)
    {
        //return _body.Find("d_r");
        return _body.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Tail/Bip001 Tail1/Bip001 Tail2/Bip001 Tail3");
    }

    public Transform GetTailRoot(Transform _body)
    {
        //if (null == _body)
        //    return null;
        return _body.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Tail");
    }

    public Transform GetRightHand(Transform _body)
    {
        //return _body.Find("d_r");
        return _body.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/d_r");
    }

    public Transform GetLeftHand(Transform _body)
    {
        return _body.Find("d_l");
    }

    public Transform GetHead(Transform _body)
    {
        return _body.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck/Bip001 Head");
    }

    public Transform GetObjectParent(Transform _body)
    {
        return _body.Find("d_obj");
    }

    public Vector3 GetHeadBoneSize(float _fatFactor)
    {
        /*var f = (1 - _fatFactor * 6);
        float y = f;
        if (_fatFactor < 0)
            y *= 0.9f;
        return new Vector3(y, f, f);*/
        //if (_fatFactor < 0)
        //    return new Vector3((1 - _fatFactor * 7), (1 - _fatFactor * 7), (1 - _fatFactor * 16));
        return Vector3.one * (1 - _fatFactor * 4);
    }

    Vector3 GetHeadTransSize(float _fatFactor)
    {
        /*float f = 1 / (1 - _fatFactor * 8);
        float y = 1 / (1 - _fatFactor * 6);
        return new Vector3(f, y, y);*/
        return Vector3.one * 1 / (1 - _fatFactor * 6);
    }

    Quaternion GetRandRot()
    {
        return Quaternion.Euler(0, GameManager.Instance.GetRandom(360), 0);
    }
    #endregion

    #region UnitFactory에서 옮김.
    public void TransformTarget_Direct(Transform _trans, Vector3 _targetPos, Quaternion _targetRot)
    {
        _trans.localPosition = _targetPos;
        _trans.localRotation = _targetRot;
    }

    public IEnumerator TransformTarget(Transform _trans, Vector3 _targetPos, Quaternion _targetRot, int _speed = 4)
    {
        while (_trans.localPosition != _targetPos)
        {
            _trans.localPosition = Vector3.MoveTowards(_trans.localPosition, _targetPos, Time.deltaTime * _speed);
            _trans.localRotation = Quaternion.Lerp(_trans.localRotation, _targetRot, Time.deltaTime * 3 * _speed / 4);
            yield return null;
        }
        TransformTarget_Direct(_trans, _targetPos, _targetRot);
    }
    
    public IEnumerator TransformRotate(Transform _trans, Quaternion _targetRot, int _speed = 4)
    {
        while (_trans.localRotation != _targetRot)
        {
            _trans.localRotation = Quaternion.Lerp(_trans.localRotation, _targetRot, Time.deltaTime * 3 * _speed / 4);
            yield return null;
        }
        _trans.localRotation = _targetRot;
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////// 全新逻辑
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// 错误提示
    /// </summary>
    /// <param name="code"></param>
    public void ToastByServerCode(int code)
    {
        if (code == 0)
            return;
    }

    /// <summary>
    /// 错误提示
    /// </summary>
    /// <param name="code"></param>
    public void ToastByMsgCode(int code)
    {
        if (code == 0)
            return;
    }
    
    public void Toast(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
        }
        else
        {
            LogUtils.LogWarning("value does not exist");
        }
    }

    IEnumerator QueryDataWaitingCoroutine()
    {
        while ((int)UserLoginState < (int)LoginState.LoginQueryDataSuccess)
        {
            yield return null;
        }

        try
        {
        }
        catch (Exception e)
        {
            LogUtils.LogWarning($"Message:{e.Message},\nStackTrace:{e.StackTrace}");
        }
    }

    /// <summary>
    /// 检查分辨率是否改变
    /// </summary>
    public void CheckRefreshResolution()
    {
        if (adjustWidth != Screen.width || adjustHeight != Screen.height)
        {
            //重新设置主摄像头正交值
            Camera.main.orthographicSize = this.GetStartCamSize();
        }
    }
}