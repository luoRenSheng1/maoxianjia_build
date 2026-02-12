using EngineBase;

namespace Engine
{
    public class OnlineManager : TSingleton<OnlineManager>
    {
        private bool _isStart;
        private float _timer;
        private ulong _onlineTime;
        public void OnInit()
        {
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_ONLINEAWARD, this.UpdateOnlineReward);
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateOnlineReward);
        }

        public override void Dispose()
        {
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_ONLINEAWARD, this.UpdateOnlineReward);
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateOnlineReward);
            base.Dispose();
        }

        public void Tick(float deltaTime)
        {
            if(!_isStart) return;
            _timer += deltaTime;
            if (_timer >= 1)
            {
                _onlineTime += (ulong) _timer;
                _timer = 0;
                DataManager.Instance.mRoleData.OnlineAwardCdTime = _onlineTime;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_ONLINEAWARD_lobby, (int) DataManager.Instance.mRoleData.OnlineAwardCdTime);
            }
        }

        private void UpdateOnlineReward()
        {
            var onlineMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.OnlineReward);
            _isStart = onlineMap.Item1;
            _onlineTime = DataManager.Instance.mRoleData.OnlineAwardCdTime;
        }
    }
}