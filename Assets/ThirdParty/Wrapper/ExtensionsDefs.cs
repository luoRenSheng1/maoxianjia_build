using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ThirdParty.Wrapper.ExtensionsDefs
{
    public static class LogUtils
    {
        [Conditional("LOG_SLG")]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("LOG_SLG"), Conditional("LOGW_SLG")]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("LOG_SLG"), Conditional("LOGW_SLG"), Conditional("LOGE_SLG")]
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("LOG_SLG"), Conditional("LOGW_SLG"), Conditional("LOGE_SLG"), Conditional("LOGP_SLG")]
        public static void LogException(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
        
        [Conditional("LOG_SLG"), Conditional("LOGW_SLG"), Conditional("LOGE_SLG"), Conditional("LOGP_SLG")]
        public static void LogException(Exception e)
        {
            UnityEngine.Debug.LogError(e.ToString());
        }
    }

    public static class Utils
    {
        public static bool GetBool(string vaule)
        {
            if (vaule != null && !vaule.Equals("") && !vaule.Equals("0"))
            {
                return true;
            }

            return false;
        }

        public static uint GetUInt(string vaule)
        {
            uint i;
            return uint.TryParse(vaule, out i) ? i : 0;
        }

        public static int GetInt(string vaule)
        {
            int i;
            return int.TryParse(vaule, out i) ? i : 0;
        }

        public static long GetLong(string vaule)
        {
            long i;
            return long.TryParse(vaule, out i) ? i : 0;
        }

        public static float GetFloat(string vaule)
        {
            float i;
            return float.TryParse(vaule, out i) ? i : 0.0f;
        }

        public static string GetString(string[] arrVaule, int index)
        {
            if (arrVaule != null && index >= 0 && index < arrVaule.Length)
            {
                return arrVaule[index];
            }

            return "";
        }

        public static int GetInt(string[] arrVaule, int index)
        {
            if (arrVaule != null && index >= 0 && index < arrVaule.Length)
            {
                return GetInt(arrVaule[index]);
            }

            return 0;
        }

        public static float GetFloat(string[] arrVaule, int index)
        {
            if (arrVaule != null && index >= 0 && index < arrVaule.Length)
            {
                return GetFloat(arrVaule[index]);
            }

            return 0.0f;
        }

        public static bool GetBool(List<string> vaule, int index)
        {
            if (vaule != null && index >= 0 && index < vaule.Count)
            {
                return GetBool(vaule[index]);
            }

            return false;
        }

        public static string GetString(List<string> vaule, int index)
        {
            if (vaule != null && index >= 0 && index < vaule.Count)
            {
                return vaule[index];
            }

            return "";
        }

        public static int GetInt(List<string> vaule, int index)
        {
            if (vaule != null && index >= 0 && index < vaule.Count)
            {
                return GetInt(vaule[index]);
            }

            return 0;
        }

        public static long GetLong(List<string> vaule, int index)
        {
            if (vaule != null && index >= 0 && index < vaule.Count)
            {
                return GetLong(vaule[index]);
            }

            return 0;
        }

        public static float GetFloat(List<string> vaule, int index)
        {
            if (vaule != null && index >= 0 && index < vaule.Count)
            {
                return GetFloat(vaule[index]);
            }

            return 0.0f;
        }

        public static List<string> UnPackageParam(string strParam)
        {
            List<string> lstParam = new List<string>();

            if (strParam.Length <= 0)
            {
                return lstParam;
            }

            int nPos = 0;
            int nLength = strParam.Length;

            int nParamLength = (int)GetUInt(strParam.Substring(nPos, 2));

            nPos += 2;

            for (int nIndex = 0; nIndex < nParamLength; ++nIndex)
            {
                if (nPos + 4 > nLength)
                {
                    break;
                }

                int nParamLengthCur = (int)GetUInt(strParam.Substring(nPos, 4));

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
    }

    public static class ConstDefs
    {
        public const int OPT_RESULT_SUCCESS = 1;
        public const int OPT_RESULT_FAIL = 2;
    }
}