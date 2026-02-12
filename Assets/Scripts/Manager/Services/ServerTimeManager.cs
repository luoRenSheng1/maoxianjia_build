using EngineBase;
using UnityEngine;

namespace Engine
{
    public class ServerTimeManager : Singleton<ServerTimeManager>
    {
        public ulong LoginTimeStamp = 0;
        private float time = 0;
        private ulong _serverTimeStamp = 0;
        private ulong _nextToZeroServerTime = 0;
        private ulong _nextWeekServerTime = 0;
        public bool IsStart { set; private get; }
        public void Update(float deltaSeconds)
        {
            if(!IsStart) return;
            time += deltaSeconds;
            if (time > 1)
            {
                _serverTimeStamp += 1;
                time -= 1;

                // if (_serverTimeStamp >= _nextToZeroServerTime)
                // {
                    // 跨天了
                    // ReddotSysManager.Instance.SendDailyResetCS();
                    // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_GO_TO_Lobby);
                // }
            }
        }

        public void SyncServerTime(ulong timeStamp)
        {
            this._serverTimeStamp = timeStamp;
        }

        public ulong CurServerTime => _serverTimeStamp;//按照 秒 来 

        public void SetNextToZeroServerTime(ulong timeStamp)
        {
            this._nextToZeroServerTime = timeStamp;
        }

        public ulong GetNextToZeroServerTime()
        {
            return _nextToZeroServerTime;
        }

        public int GetToZeroLeftTime()
        {
            return (int) (this._nextToZeroServerTime - _serverTimeStamp);
        }

        public void SetNextWeekServerTime(ulong timeStamp)
        {
            _nextWeekServerTime = timeStamp;
        }

        public ulong GetNextWeekServerTime()
        {
            return _nextWeekServerTime;
        }
    }
}