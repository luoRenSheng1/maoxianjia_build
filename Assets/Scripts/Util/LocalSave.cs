using System.Collections.Generic;
using PlayerPrefsEx = EngineBase.PlayerPrefs;

namespace Engine
{
    public static class LocalSave
    {
        public static void DeleteAll()
        {
            PlayerPrefsEx.DeleteAll();
        }

        public static bool HasKey(string key)
        {
            return PlayerPrefsEx.HasKey(key);
        }

        public static void DeleteKey(string key)
        {
            PlayerPrefsEx.DeleteKey(key);
        }

        public static float GetFloat(string key, float defaultValue)
        {
            return PlayerPrefsEx.GetFloat(key, defaultValue);
        }

        public static void SetFloat(string key, float value)
        {
            PlayerPrefsEx.SetFloat(key, value);
        }

        public static int GetInt(string key, int defaultValue)
        {
            return PlayerPrefsEx.GetInt(key, defaultValue);
        }

        public static void SetInt(string key, int value)
        {
            PlayerPrefsEx.SetInt(key, value);
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            return PlayerPrefsEx.GetInt(key, defaultValue ? 1 : 0) == 1;
        }

        public static void SetBool(string key, bool value)
        {
            PlayerPrefsEx.SetInt(key, value ? 1 : 0);
        }

        public static string GetString(string key, string defaultValue)
        {
            return PlayerPrefsEx.GetString(key, defaultValue);
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefsEx.SetString(key, value);
        }

        private static string GetAccountKey(string key)
        {
            var userID = "";
            if (null !=  DataManager.Instance && null != DataManager.Instance.mRoleData)
            {
                userID = DataManager.Instance.mRoleData.userID;
            }
            string appendKey = string.Format("{0}_{1}", key, userID);
            return appendKey;
        }

        public static float GetFloatWithAccount(string key, float defaultValue)
        {
            var newKey = GetAccountKey(key);
            return GetFloat(newKey, defaultValue);
        }

        public static void SetFloatWithAccount(string key, float value)
        {
            var newKey = GetAccountKey(key);
            SetFloat(newKey, value);
        }

        public static int GetIntWithAccount(string key, int defaultValue)
        {
            var newKey = GetAccountKey(key);
            return GetInt(newKey, defaultValue);
        }

        public static void SetIntWithAccount(string key, int value)
        {
            var newKey = GetAccountKey(key);
            SetInt(newKey, value);
        }

        public static bool GetBoolWithAccount(string key, bool defaultValue)
        {
            var newKey = GetAccountKey(key);
            return GetBool(newKey, defaultValue);
        }

        public static void SetBoolWithAccount(string key, bool value)
        {
            var newKey = GetAccountKey(key);
            SetBool(newKey, value);
        }

        public static string GetStringWithAccount(string key, string defaultValue)
        {
            var newKey = GetAccountKey(key);
            return GetString(newKey, defaultValue);
        }

        public static void SetStringWithAccount(string key, string value)
        {
            var newKey = GetAccountKey(key);
            SetString(newKey, value);
        }

        public static void Save()
        {
            PlayerPrefsEx.Save();
        }
    }

    public static class SaveKey
    {
        public const string CATSOUPACCOUNT = "catsoupaccount";
        public const string CATSOUPSERVER = "catsoupserver";
        public const string CATSOUPSERVERURL = "catsoupserverurl";
        public const string CATSOUPACCOUNTID = "catsoupaccountid";

        public const string Volume_BGM = "vol_bgm";
        public const string BGM_Save = "BGM_Save";
        public const string Volume_SFX = "vol_sfx";
        public const string Volume_UI = "vol_ui";
        public const string Volume_Cat = "vol_cat";
        public const string AnnounceKey = "AnnounceKey";
        public const string Vibration = "vibration";

        public const string Language_Index = "languageIndex";
        public const string monsterEntry = "monsterEntry";
        public const string exploredMapData = "exploredMapData";
        public const string MapUnlockData = "MapUnlockData";
        public const string MapBossStageMove = "MapBossStageMove";
    }
}