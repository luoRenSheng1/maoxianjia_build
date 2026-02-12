//内部sdk聚合

#if !UNITY_EDITOR && UNITY_ANDROID
#define USE_ANDROID
#endif

#if !UNITY_EDITOR && UNITY_IOS
#define USE_IOS
#endif

#if !UNITY_EDITOR && PF_WEIXIN
#define USE_WEIXIN
#endif

#if !UNITY_EDITOR && PF_DOUYIN
#define USE_DOUYIN
#endif

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ThirdParty.Util;
using ThirdParty.Wrapper.ExtensionsDefs;

#if USE_DOUYIN
using StarkSDKSpace;
#endif

namespace ThirdParty.Wrapper
{
    public class ThirdPartyWrapper : MonoBehaviour
    {
        #region Call Android

        public static void CallJava(string methodName, params object[] parms)
        {
#if USE_ANDROID
            try
            {
                using (AndroidJavaClass jc = new AndroidJavaClass("com.zhuque.catsoup.utils.SDKInterface"))
                {
                    using (AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("getInstance"))
                    {
                        jo.Call(methodName, parms);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogUtils.LogException("CallJava " + ex.Message);
            }
#endif
        }

        public static T CallJavaWithReturn<T>(string methodName, params object[] parms)
        {
#if USE_ANDROID
            try
            {
                using (AndroidJavaClass jc = new AndroidJavaClass("com.zhuque.catsoup.utils.SDKInterface"))
                {
                    using (AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>("getInstance"))
                    {
                        return jo.Call<T>(methodName, parms);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogUtils.LogException("CallJava " + ex.Message);
            }
#endif
            return default(T);
        }

        #endregion

        #region Call iOS

#if USE_IOS
        [DllImport("__Internal")]
        public static extern string SDK_GetPlatformParam1();
        [DllImport("__Internal")]
        public static extern string SDK_GetPlatformParam2();
        [DllImport("__Internal")]
        public static extern string SDK_GetPlatformParam3();
        [DllImport("__Internal")]
        public static extern int SDK_GetPlatform();
        [DllImport("__Internal")]
        public static extern string SDK_GetDeviceName();
        [DllImport("__Internal")]
        public static extern string SDK_GetDeviceID();
        [DllImport("__Internal")]
        public static extern string SDK_GetSystemLanguage();
        [DllImport("__Internal")]
        public static extern string SDK_GetSystemVersion();
        [DllImport("__Internal")]
        public static extern string SDK_GetChannelName();
        [DllImport("__Internal")]
        public static extern int SDK_CopyToClipboard(string input);
        [DllImport("__Internal")]
        public static extern int SDK_JumpToAppStore(string url);
        [DllImport("__Internal")]
        public static extern int SDK_InitVersion(bool bReviewPatentVersion, bool bReviewStoreVersion);
        [DllImport("__Internal")]
        public static extern int SDK_UnInitSDK();
        [DllImport("__Internal")]
        public static extern int SDK_LoadConfig();
        [DllImport("__Internal")]
        public static extern int SDK_Login();
        [DllImport("__Internal")]
        public static extern int SDK_Logout();
        [DllImport("__Internal")]
        public static extern int SDK_RequestAgreementSignStatus();
        [DllImport("__Internal")]
        public static extern int SDK_TranslateText(string id, string text, string language);
        [DllImport("__Internal")]
        public static extern int SDK_ExceptionLog(string condition, string info);
        [DllImport("__Internal")]
        public static extern int SDK_EventLog(string strEvent, string iggid, string playerLv, string eMoney, string money, string createTime, string isWifi);
        [DllImport("__Internal")]
        public static extern string SDK_GetAgreementURL(int nType);
        [DllImport("__Internal")]
        public static extern int SDK_RequestServiceURL();
        [DllImport("__Internal")]
        public static extern int SDK_GetComplianceState();
        [DllImport("__Internal")]
        public static extern int SDK_RealNameVerification();
        [DllImport("__Internal")]
        public static extern int SDK_SignAgreementSign();
        [DllImport("__Internal")]
        public static extern int SDK_ConfirmLoginWithType(int nType, bool bConfirm);
        [DllImport("__Internal")]
        public static extern int SDK_BindWithType(int nType);
        [DllImport("__Internal")]
        public static extern int SDK_ConfirmBindWithType(int nType, bool bConfirm);
        [DllImport("__Internal")]
        public static extern int SDK_Pay(string strProductID, string strJsonData);
        [DllImport("__Internal")]
        public static extern string SDK_GetProductItems();
        [DllImport("__Internal")]
        public static extern int SDK_GetScreenBrightness();
        [DllImport("__Internal")]
        public static extern int SDK_SetScreenBrightness(float value);
        [DllImport("__Internal")]
        public static extern int SDK_InitUtils();
        [DllImport("__Internal")]
        public static extern int SDK_OpenUrlScheme(string strScheme);
#endif

        #endregion

        #region Call Weixin

#if USE_WEIXIN
        [DllImport("__Internal")]
        private static extern void WX_ShareImageToGameCenter(string conf, string callbackId);
        [DllImport("__Internal")]
        private static extern void WX_ShareCanvasToGameCenter(string conf, string callbackId);
#endif

        /// <summary>
        /// 分享CDN图片到游戏圈
        /// </summary>
        /// <param name="gameCenterOption"></param>
        public void ShareImageToGameCenter(ShareImageToGameCenterOption gameCenterOption)
        {
#if USE_WEIXIN
            var conf = JsonUtility.ToJson(gameCenterOption);
            var callbackId = MGCallbackHandler.Add(gameCenterOption);
            WX_ShareImageToGameCenter(conf, callbackId);
#endif
        }

        /// <summary>
        /// 分享canvas导出到游戏圈
        /// conf: { x: number, y: number, width: number, height: number, destWidth: number, destHeight: number }
        /// </summary>
        /// <param name="gameCenterOption"></param>
        public void ShareCanvasToGameCenter(ShareImageToGameCenterOption gameCenterOption)
        {
#if USE_WEIXIN
            var conf = JsonUtility.ToJson(gameCenterOption);
            var callbackId = MGCallbackHandler.Add(gameCenterOption);
            WX_ShareCanvasToGameCenter(conf, callbackId);
#endif
        }

        /// <summary>
        /// 分享到游戏圈回调
        /// </summary>
        /// <param name="result"></param>
        public void OnShareImageToGameCenterCallback(string result)
        {
#if USE_WEIXIN
            MGCallbackHandler.InvokeResponseCallback<MGBaseCallbackResult>(result);
#endif
        }

        #endregion

        #region Call Douyin

#if USE_DOUYIN
        //[DllImport("__Internal")]
        //private static extern void DY_PluginShowToast();
#endif

        public void DoPluginShowToast()
        {
#if USE_DOUYIN
            //DY_PluginShowToast();
#endif
        }

        #endregion

        private bool bNotchFit = false; // 是否是刘海屏幕
        private int nNotchFitWidth = 0;
        private int nNotchFitHeight = 0;
        public bool NotchFit => bNotchFit;
        public int NotchFitWidth => nNotchFitWidth;
        public int NotchFitHeight => nNotchFitHeight;

        private static ThirdPartyWrapper _instance = null;

        public static ThirdPartyWrapper Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = CreateNew();
                }

                return _instance;
            }
        }

        public static ThirdPartyWrapper CreateNew()
        {
            GameObject go = new GameObject();
            go.name = "ThirdPartyWrapper";
            GameObject.DontDestroyOnLoad(go);

            _instance = go.AddComponent<ThirdPartyWrapper>();
            _instance.InitSDK();

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

        // void Awake()
        // {
        //
        // }

        // void OnDestroy()
        // {
        //
        // }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitSDK()
        {
            try
            {
                LogUtils.LogWarning("[Wrapper] InitSDK");
#if USE_ANDROID
                ThirdPartyWrapper.CallJava("InitUtils");//包含InitNotchFitMode
#elif USE_IOS
                SDK_InitUtils();//包含InitNotchFitMode
#endif
            }
            catch (Exception e)
            {
                LogUtils.LogException(e.Message);
            }
        }

        /// <summary>
        /// 初始化SDK结果
        /// </summary>
        /// <param name="strParam"></param>
        public void InitSDKResult(string strParam)
        {
            LogUtils.LogWarning($"[Wrapper] InitSDKResult: Result:{strParam}");

            List<string> lstResult = Utils.UnPackageParam(strParam);

            int type = Utils.GetInt(lstResult, 0);
            switch (type)
            {
                case ConstDefs.OPT_RESULT_SUCCESS:
                    InitSDKSuccess();
                    break;
                case ConstDefs.OPT_RESULT_FAIL:
                    InitSDKFail();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 初始化SDK完成
        /// </summary>
        public void InitSDKSuccess()
        {
            LogUtils.LogWarning("[Wrapper] InitSDKSuccess");

#if USE_WEIXIN
            WeixinWrapper.Instance.InitSystemInfo();
#elif USE_DOUYIN
            DouyinWrapper.Instance.InitSystemInfo();
#endif
        }

        /// <summary>
        /// 初始化SDK失败
        /// </summary>
        public void InitSDKFail()
        {
            LogUtils.LogWarning("[Wrapper] InitSDKFail");
        }

        /// <summary>
        /// 刘海屏适配相关结果
        /// </summary>
        /// <param name="strParam"></param>
        public void InitNotchFitModeResult(string strParam)
        {
            List<string> lstResult = Utils.UnPackageParam(strParam);

            bNotchFit = Utils.GetBool(lstResult, 0);
            nNotchFitWidth = Utils.GetInt(lstResult, 1);
            nNotchFitHeight = Utils.GetInt(lstResult, 2);

            LogUtils.LogWarning($"InitNotchFitModeResult on({bNotchFit}) width({nNotchFitWidth}) height({nNotchFitHeight})");
        }

        /// <summary>
        /// 功能更新唤醒是否可用
        /// </summary>
        /// <returns></returns>
        public bool IsUpdateGameAvailable()
        {
#if USE_COMBO_SDK
            return false;
#else
            return false;
#endif
        }

        /// <summary>
        /// 功能退出游戏是否可用
        /// </summary>
        /// <returns></returns>
        public bool IsChannelHasExitDialog()
        {
#if USE_COMBO_SDK
            return false;
#else
            return false;
#endif
        }

        /// <summary>
        /// 更新唤醒, 配合IsUpdateGameAvailable使用，必须校验Feature.UPDATE_GAME是否可用
        /// </summary>
        public void UpdateGame(Action success = null, Action fail = null)
        {
#if USE_COMBO_SDK
            try
            {
                success?.Invoke();
            }
            catch (Exception e)
            {
                LogUtils.LogException(e);
                fail?.Invoke();
            }
#else
            success?.Invoke();
#endif
        }

        /// <summary>
        /// 渠道退出，配合IsUpdateGameAvailable使用，必须校验Feature.QUIT是否可用
        /// </summary>
        public void QuitGame()
        {
#if USE_COMBO_SDK

#else
            ApplicationQuit();
#endif
        }

        /// <summary>
        /// 退出应用
        /// </summary>
        public void ApplicationQuit()
        {
#if USE_WEIXIN
            WeChatWASM.ExitMiniProgramOption callback = new WeChatWASM.ExitMiniProgramOption();
            WeChatWASM.WX.ExitMiniProgram(callback);
#elif USE_DOUYIN
            StarkSDK.API.ExitApp();
#else
            UnityEngine.Application.Quit();
#endif
        }

        /// <summary>
        /// 获取渠道ID
        /// </summary>
        /// <returns></returns>
        public string GetChannelID()
        {
            return GetChannelName();
        }

        /// <summary>
        /// 获取渠道名
        /// </summary>
        /// <returns></returns>
        public string GetChannelName()
        {
#if USE_WEIXIN
            return "weixin";
#elif USE_DOUYIN
            return "douyin";
#elif USE_ANDROID
            return CallJavaWithReturn<string>("GetChannelName");
#elif USE_IOS
            return SDK_GetChannelName();
#else
            return "GuanFang";
#endif
        }

        /// <summary>
        /// 针对不支持更新唤起的发行版本（即 UpdateGame 不可用时），可调用 GetDownloadUrl 方法
        /// 获得该发行版本或发行版本分包对应的下载地址，游戏方自行实现更新唤起弹窗，引导用户进行游戏更新。
        /// </summary>
        /// <param name="action"></param>
        public void GetDownloadUrl(Action<string> action = null)
        {
#if USE_COMBO_SDK
            try
            {
                action?.Invoke("");
            }
            catch (Exception e)
            {
                LogUtils.LogException(e);
                action?.Invoke("");
            }
#else
            action?.Invoke("");
#endif
        }

        /// <summary>
        /// 获取分包标识
        /// </summary>
        /// <returns></returns>
        public string GetVariant()
        {
#if USE_COMBO_SDK
            return "";
#else
            return "";
#endif
        }
    }
}