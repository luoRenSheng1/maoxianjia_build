using System;
using System.Collections.Generic;
using EngineBase;

namespace Engine
{
    public class AdManager : TSingleton<AdManager>
    {
        private Dictionary<int, int> _adFreeTimeDict = new Dictionary<int, int>();
        public void OnInit()
        {
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_WATCHAD_START, this.OnWatchStart);
            EventDispatcher.GameWorld.Regist(EventDefine.EVNET_WATCHAD_SUCCESS, this.OnWatchSuccess);
        }

        public override void Dispose()
        {
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_WATCHAD_START, this.OnWatchStart);
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVNET_WATCHAD_SUCCESS, this.OnWatchSuccess);
            base.Dispose();
        }

        private void OnWatchStart()
        {
            UIManager.Instance.ShowUIPanel("PayLoading", 1);
        }

        private void OnWatchSuccess()
        {
            UIManager.Instance.CloseUIPanel("PayLoading");
            
        }

        public void WatchAd(Action paySuccess = null)
        {
            //sdk
            paySuccess?.Invoke();
        }

        public void SetAdFreeTime(int type, int freeTimes)
        {
            if (_adFreeTimeDict.ContainsKey(type))
            {
                _adFreeTimeDict[type] = freeTimes;
            }
            else
            {
                _adFreeTimeDict.Add(type, freeTimes);
            }
        }

        public int GetAdFreeTimes(int type)
        {
            if (_adFreeTimeDict.TryGetValue(type, out int freeTimes))
            {
                return freeTimes;
            }

            return 0;
        }
    }
}