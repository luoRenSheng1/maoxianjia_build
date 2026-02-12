#if !UNITY_EDITOR && UNITY_ANDROID
#define USE_ANDROID
#endif

#if !UNITY_EDITOR && UNITY_IOS
#define USE_IOS
#endif

using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using ThirdParty.Wrapper;

#if PF_WEIXIN
using WeChatWASM;
#endif

public class CI : MonoBehaviour
{
    public enum LOG_LEVEL
    {
        INFO = 1,
        WARNING = 2,
        ERROR = 3,
        EXCEPTION = 4,
    }

    private bool bInitSDK = false;
    public SpriteRenderer spr;

    void Awake()
    {
        InitSDK();

#if UNITY_STANDALONE || UNITY_EDITOR || PF_WEIXIN || PF_DOUYIN
        SceneManager.LoadScene("ready", LoadSceneMode.Single);
#else
        StartCoroutine(ShowCI());
#endif
    }
    
    IEnumerator ShowCI()
    {
        float t = 1.5f;
        Color targetColor = Color.white;
        while (t > 0)
        {
            t -= Time.deltaTime;
            targetColor.a = t;
            spr.color = targetColor;
            yield return null;
        }
        
        SceneManager.LoadScene("ready", LoadSceneMode.Single);
    }

    public bool IsLogSDK()
    {
#if LOG_SLG || LOGW_SLG
        return true;
#else
        return false;
#endif
    }

    public void InitSDK()
    {
        if (bInitSDK)
        {
            InitSDKSuccess("");
            return;
        }

        bInitSDK = true;
        LogWarning("InitSDK");
        
        ThirdPartyWrapper.CreateNew();
        
#if !UNITY_EDITOR && PF_WEIXIN
        WX.InitSDK((code) =>
			{
                InitSDKSuccess("");
            });
#elif USE_ANDROID
        CallJava("InitSDK", "", IsLogSDK());
#elif USE_IOS
        SDK_InitSDK("", IsLogSDK());
#else
        InitSDKSuccess("");
#endif
    }

    public void InitSDKSuccess(string strMsg)
    {
        LogWarningFormat("InitSDKSuccess {0}", strMsg);

        ThirdPartyWrapper.Instance.InitSDKSuccess();
    }

    public void InitSDKFail()
    {
        LogError("InitSDKFail");
    }

    // 跳转到应用商店
    public static void JumpToAppStore(string url, string marketPkg)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        CallJava("JumpToAppStore", url, marketPkg);
#elif !UNITY_EDITOR && UNITY_IOS
        SDK_JumpToAppStore(url);
#endif
    }

    /// <summary>
    /// 渠道有退出确认框使用渠道，没有使用自身
    /// </summary>
    /// <returns></returns>
    public static bool IsChannelHasExitDialog()
    {
        return ThirdPartyWrapper.Instance.IsChannelHasExitDialog();
    }

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
            LogException("CallJava " + ex.Message);
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
            LogException("CallJava " + ex.Message);
        }
#endif
        return default(T);
    }

#if USE_IOS
    [DllImport("__Internal")]
    public static extern int SDK_InitSDK(string language, bool bSDKLog);
    [DllImport("__Internal")]
    public static extern int SDK_GetPlatform();
    [DllImport("__Internal")]
    public static extern int SDK_JumpToAppStore(string url);
#endif

#if USE_IOS
    [DllImport("__Internal")]
    public static extern int TOOLUnCompress(string strSrcFile, string strDestDire);
#elif USE_ANDROID
    [DllImport("fc")]
    public static extern int TOOLUnCompress(string strSrcFile, string strDestDire);
#else
    [DllImport("fc.dll")]
    public static extern int TOOLUnCompress(IntPtr strSrcFile, int fileSize, IntPtr strDestDire, int dirSize);
#endif

    [Conditional("LOG_SLG")]
    public static void Log(string message)
    {
        DoLog(LOG_LEVEL.INFO, message);
    }

    [Conditional("LOG_SLG")]
    public static void LogFormat(string format, params object[] args)
    {
        DoLogFormat(LOG_LEVEL.INFO, format, args);
    }

    [Conditional("LOG_SLG"), Conditional("LOGW_SLG")]
    public static void LogWarning(string message)
    {
        DoLog(LOG_LEVEL.WARNING, message);
    }

    [Conditional("LOG_SLG"), Conditional("LOGW_SLG")]
    public static void LogWarningFormat(string format, params object[] args)
    {
        DoLogFormat(LOG_LEVEL.WARNING, format, args);
    }

    [Conditional("LOG_SLG"), Conditional("LOGW_SLG"), Conditional("LOGE_SLG")]
    public static void LogError(string message)
    {
        DoLog(LOG_LEVEL.ERROR, message);
    }

    [Conditional("LOG_SLG"), Conditional("LOGW_SLG"), Conditional("LOGE_SLG")]
    public static void LogErrorFormat(string format, params object[] args)
    {
        DoLogFormat(LOG_LEVEL.ERROR, format, args);
    }

    [Conditional("LOG_SLG"), Conditional("LOGW_SLG"), Conditional("LOGE_SLG"), Conditional("LOGP_SLG")]
    public static void LogException(string message)
    {
        DoLog(LOG_LEVEL.EXCEPTION, message);
    }

    [Conditional("LOG_SLG"), Conditional("LOGW_SLG"), Conditional("LOGE_SLG"), Conditional("LOGP_SLG")]
    public static void LogException(Exception e)
    {
        LogException(e.ToString());
    }

    private static void DoLog(LOG_LEVEL nLogLevel, string message)
    {
        switch (nLogLevel)
        {
            case LOG_LEVEL.INFO:
                UnityEngine.Debug.Log(message);
                break;
            case LOG_LEVEL.WARNING:
                UnityEngine.Debug.LogWarning(message);
                break;
            case LOG_LEVEL.ERROR:
                UnityEngine.Debug.LogError(message);
                break;
            case LOG_LEVEL.EXCEPTION:
                UnityEngine.Debug.LogError(message);
                break;
            default:
                break;
        }
    }

    private static void DoLogFormat(LOG_LEVEL nLogLevel, string format, params object[] args)
    {
        switch (nLogLevel)
        {
            case LOG_LEVEL.INFO:
                UnityEngine.Debug.LogFormat(format, args);
                break;
            case LOG_LEVEL.WARNING:
                UnityEngine.Debug.LogWarningFormat(format, args);
                break;
            case LOG_LEVEL.ERROR:
                UnityEngine.Debug.LogErrorFormat(format, args);
                break;
            case LOG_LEVEL.EXCEPTION:
                UnityEngine.Debug.LogErrorFormat(format, args);
                break;

            default:
                break;
        }
    }

    public static bool GetBool(string vaule)
    {
        return !string.IsNullOrEmpty(vaule) && !vaule.Equals("0");
    }
    
    public static int GetInt(string vaule)
    {
        int i;
        return int.TryParse(vaule, out i) ? i : 0;
    }

    public static DateTime GetDateTime(string vaule)
    {
        // 固定格式为 yyyy/MM/dd hh:mm:ss
        if (!string.IsNullOrEmpty(vaule))
        {
            int year = 0;
            int month = 0;
            int day = 0;
            int hour = 0;
            int minute = 0;
            int second = 0;

            var ls1 = vaule.Split(' ');

            if (ls1 != null && ls1.Length > 0)
            {
                var ls11 = ls1[0].Split('/');

                if (ls11 != null && ls11.Length >= 3)
                {
                    year = CI.GetInt(ls11[0]);
                    month = CI.GetInt(ls11[1]);
                    day = CI.GetInt(ls11[2]);
                }
            }

            if (ls1 != null && ls1.Length > 1)
            {
                var ls11 = ls1[1].Split(':');

                if (ls11 != null && ls11.Length >= 3)
                {
                    hour = CI.GetInt(ls11[0]);
                    minute = CI.GetInt(ls11[1]);
                    second = CI.GetInt(ls11[2]);
                }
            }

            try
            {
                var result = new DateTime(year, month, day, hour, minute, second);
                return result;
            }
            catch (System.Exception ex)
            {
                LogException("CallJava " + ex.Message);
            }
            return DateTime.Now;
        }

        return DateTime.Now;
    }
    
    public static string GetString(string[] arrVaule, int index)
    {
        if (arrVaule != null && index >= 0 && index < arrVaule.Length)
        {
            return arrVaule[index];
        }

        return "";
    }

    public static bool GetBool(string[] arrVaule, int index)
    {
        var s = GetString(arrVaule, index);
        return !string.IsNullOrEmpty(s) && !s.Equals("0");
    }

    public static int GetTimeStamp(DateTime date)
    {
        return (int)((date.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
    }

    /// <summary>
    /// 尝试将 bytes 解析为 xml
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="root"></param>
    /// <returns></returns>
    public static bool TryParse(byte[] bytes, out XML root)
    {
        if (bytes == null)
        {
            root = null;
            return false;
        }

        string str = GetUTF8StringWithoutBom(bytes);
        root = new XML(str);
        return true;
    }

    public static string GetUTF8StringWithoutBom(byte[] buffer)
    {
        if (buffer == null)
        {
            return null;
        }

        if (buffer.Length >= 3 && (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf))
        {
            if (buffer.Length == 3)
            {
                return "";
            }
            else
            {
                return new UTF8Encoding(false).GetString(buffer, 3, buffer.Length - 3);
            }
        }

        return Encoding.UTF8.GetString(buffer);
    }

    /// <summary>
    /// 获取属性，异常情况下为空字符串
    /// </summary>
    /// <param name="node"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetString(XML node, string name)
    {
        if (node == null)
        {
            return string.Empty;
        }

        string vaule = node.GetAttribute(name);
        return !string.IsNullOrEmpty(vaule) ? vaule.Replace("\\n", "\n") : string.Empty;
    }


    /// <summary>
    /// 获取属性，异常情况下为 0
    /// </summary>
    /// <param name="node"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static int GetInt(XML node, string name)
    {
        if (node == null)
        {
            return 0;
        }

        return node.GetAttributeInt(name, 0);
    }

    public static bool IsWIFI()
    {
#if !UNITY_EDITOR
        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            return true;
        }
#endif

        return false;
    }

    public static string GetMd5Str(byte[] bytes)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        string t2 = BitConverter.ToString(md5.ComputeHash(bytes), 4, 8);
        return t2.ToLower();
    }

    public static void ApplicationQuit()
    {
        ThirdPartyWrapper.Instance.ApplicationQuit();
    }
}
