#if PF_DOUYIN
using System;
using System.Collections.Generic;
using StarkSDKSpace;
using ThirdParty.Wrapper.ExtensionsDefs;
using JsonData = StarkSDKSpace.UNBridgeLib.LitJson.JsonData;

namespace ThirdParty.Wrapper
{
    public class DouyinWrapper
    {
        private bool m_bInited = false;
        public StarkSystemInfo m_systemInfo { get; private set; }
        public ClientRect m_clientRect { get; private set; }

        static DouyinWrapper _instance = null;

        public static DouyinWrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DouyinWrapper();
                }

                return _instance;
            }
        }

        public void InitSystemInfo()
        {
            if (!m_bInited)
            {
                LogUtils.LogWarning("DouyinWrapper InitSystemInfo");

                m_systemInfo = StarkSDK.API.GetSystemInfo();
                m_clientRect = new ClientRect(StarkSDK.API.GetMenuButtonLayout());
                
                m_bInited = true;
                
                var appLifeCycle = StarkSDK.API.GetStarkAppLifeCycle();
                appLifeCycle.OnShowWithDict += OnShowWithDict;
            }
        }

        public StarkSystemInfo GetSystemInfoSync()
        {
            InitSystemInfo();
            return m_systemInfo;
        }

        public void GetSystemInfoByTDLogin(Action<string, string> action)
        {
            if (action != null)
            {
                var sysInfo = GetSystemInfoSync();
                action.Invoke(sysInfo.model, sysInfo.platform);
            }
        }
        
        //抖音时机：确保在游戏启动时机（game.js运行时机），估修改
        private string launchFrom { get; set; }
        private string location { get; set; }
        public string accountIDFrom { get; set; }

        public Dictionary<string, object> dictOnShowParams = new Dictionary<string, object>();
        
        private void OnShowWithDict(Dictionary<string, object> param)
        {
            if (param != null)
            {
                JsonData jsonData = new JsonData();
                foreach (var kvp in param)
                {
                    if (!dictOnShowParams.TryAdd(kvp.Key, kvp.Value))
                    {
                        dictOnShowParams[kvp.Key] = kvp.Value;
                    }
                    string toString = SimpleJson.SerializeObjectInHeap(kvp.Value);
                    jsonData[kvp.Key] = toString;
                }
                string json = jsonData.ToJson();
                LogUtils.LogWarning($"OnShowWithDict param:{json}");
            
                object lf;
                if (param.TryGetValue("launch_from", out lf))
                {
                    LogUtils.LogWarning($"OnShowWithDict launch_from{lf}");
                    launchFrom = lf.ToString();
                }
                object lt;
                if (param.TryGetValue("location", out lt))
                {
                    LogUtils.LogWarning($"OnShowWithDict location{lt}");
                    location = lt.ToString();
                }
                
                object af;
                if (param.TryGetValue("ac", out af))
                {
                    LogUtils.LogWarning($"OnShowWithDict Query ac {af}");
                    accountIDFrom = af.ToString();
                }
            }
        }
        
        /// <summary>
        /// 游戏中途从侧边栏中复访
        /// </summary>
        /// <returns></returns>
        public string GetLaunchFrom()
        {
            if (string.IsNullOrEmpty(launchFrom))
            {
                launchFrom = StarkSDK.s_ContainerEnv.GetLaunchFrom();
            }

            return launchFrom;
        }

        /// <summary>
        /// 游戏中途从侧边栏中复访
        /// </summary>
        /// <returns></returns>
        public string GetLocation()
        {
            if (string.IsNullOrEmpty(location))
            {
                location = StarkSDK.s_ContainerEnv.GetLocation();
            }

            return location;
        }
    }
}

#endif