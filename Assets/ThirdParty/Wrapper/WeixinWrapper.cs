#if PF_WEIXIN
using System;
using System.Collections.Generic;
using WeChatWASM;

namespace ThirdParty.Wrapper
{
    public class WeixinWrapper
    {
        private bool m_bInited = false;
        public WeChatWASM.SystemInfo m_systemInfo { get; private set; }
        public WeChatWASM.ClientRect m_clientRect { get; private set; }

        static WeixinWrapper _instance = null;

        static public WeixinWrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WeixinWrapper();
                }

                return _instance;
            }
        }

        public void InitSystemInfo()
        {
            if (m_bInited)
            {
                return;
            }

            m_bInited = true;

#if !UNITY_EDITOR
            m_systemInfo = WX.GetSystemInfoSync();
            m_clientRect = WX.GetMenuButtonBoundingClientRect();
#else
            m_systemInfo = new WeChatWASM.SystemInfo();
            m_systemInfo.platform = "";
            m_clientRect = new WeChatWASM.ClientRect();
#endif

#if !UNITY_EDITOR
            WXShareAppMessageParam defaultParam = new WXShareAppMessageParam();
            defaultParam.imageUrl = @"https://mmocgame.qpic.cn/wechatgame/8DkXgX0bkC8NK3n0c5QrGsF0E7N2Cl505G6YxCa1zflA2ZjezkV3veK4sGRqGMon/0";
            defaultParam.imageUrlId = @"c27BvOxSQz++Ds85h5CPeg==";

            WX.OnShareAppMessage(defaultParam, (Action<WXShareAppMessageParam> action) =>
            {
                if (action != null)
                {
                    action(defaultParam);
                }
            });
#endif
        }

        public WeChatWASM.SystemInfo GetSystemInfoSync()
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
    }
}

#endif