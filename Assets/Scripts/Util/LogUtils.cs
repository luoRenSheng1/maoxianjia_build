using System;
using System.Diagnostics;
using BestHTTP;
using Engine;
using UnityEngine;


public class LogUtils
{
    public static string strLocalPath = "";
    private const string strWebLogURL = @"http://192.168.250.150:8090/log";

    public enum LOG_LEVEL
    {
        INFO = 1,
        WARNING = 2,
        ERROR = 3,
        EXCEPTION = 4,
    }

    public static void LogLocal(string message)
    {
        if (string.IsNullOrEmpty(strLocalPath))
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            strLocalPath = Application.dataPath + "/StreamingAssets/local.log";
#else
                strLocalPath = Application.persistentDataPath + "/local.log";
#endif
        }

#if !UNITY_WEBGL
        System.IO.File.AppendAllText(strLocalPath,
            GetMilliSecond() + " " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString("00") + " : " +
            message + "\n", System.Text.Encoding.UTF8);
#endif
    }

    public static void LogLocalFormat(string format, params object[] args)
    {
        LogLocal(string.Format(format, args));
    }

    public static void LogWeb(string message)
    {
        HTTPRequest request = new HTTPRequest(new Uri(strWebLogURL), HTTPMethods.Post, null);
        request.AddField("t", message);
        request.Send();
    }

    public static void LogWebFormat(string format, params object[] args)
    {
        LogWeb(string.Format(format, args));
    }

    private static void DoLog(LOG_LEVEL nLogLevel, string message)
    {
        if(!Utils.isDebug) return;
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
            {
                var messageNew = string.Format("CriticalLogException\t{0}", message);
                UnityEngine.Debug.LogError(messageNew);
            }
                break;

            default:
                break;
        }

#if LOG_LOCAL
        LogLocal(message);
#endif

#if LOG_WEB
        LogWeb(message);
#endif
    }

    private static void DoLogFormat(LOG_LEVEL nLogLevel, string format, params object[] args)
    {
        string message = string.Format(format, args);

        DoLog(nLogLevel, message);
    }

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

    [Conditional("LOG_NETMSG")]
    public static void LogNetMsg(string message)
    {
#if UNITY_EDITOR
        DoLog(LOG_LEVEL.INFO, message);
#else
            var messageNew = string.Format("{0} {1}", GetMilliSecond(), message);
            DoLog(LOG_LEVEL.INFO, messageNew);
#endif
    }

    [Conditional("LOG_NETMSG")]
    public static void LogNetMsgFormat(string format, params object[] args)
    {
        string strMessage = string.Format(format, args);
        LogNetMsg(strMessage);
    }

    [Conditional("LOG_SLG"), Conditional("LOGW_SLG"), Conditional("LOGE_SLG")]
    public static void Assert(bool condition, string message)
    {
        if (condition)
            return;

        DoLog(LOG_LEVEL.ERROR, message);
    }

    private static string GetMilliSecond()
    {
        return DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss.ffff");
    }

    /// <summary>
    /// 关键代码，勿删 XXY Edit
    /// </summary>
#if LOG_SLG
    public static void MarkInfoLog()
    {
        DoLog(LOG_LEVEL.INFO, "MarkInfoLog");
    }
#elif LOGW_SLG
        public static void MarkWarningLog()
        {
            DoLog(LOG_LEVEL.WARNING, "MarkWarningLog");
        }
#elif LOGE_SLG
        public static void MarkErrorLog()
        {
            DoLog(LOG_LEVEL.ERROR, "MarkErrorLog");
        }
#elif LOGP_SLG
        public static void MarkExceptionLog()
        {
            DoLog(LOG_LEVEL.EXCEPTION, "MarkExceptionLog");
        }
#endif
}