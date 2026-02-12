using System;
using EngineBase;

namespace Engine
{
    public class PayManager : TSingleton<PayManager>
    {
        public void OnInit()
        {
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PAY_START, this.OnPayStart);
            EventDispatcher.GameWorld.Regist(EventDefine.EVNET_PAY_SUCCESS, this.OnPaySuccess);
        }

        public override void Dispose()
        {
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PAY_START, this.OnPayStart);
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVNET_PAY_SUCCESS, this.OnPaySuccess);
            base.Dispose();
        }

        private void OnPayStart()
        {
            UIManager.Instance.ShowUIPanel("PayLoading", 0);
        }

        private void OnPaySuccess()
        {
            UIManager.Instance.CloseUIPanel("PayLoading");
            
        }

        public void Pay(string goodsID, string goodsDesc, string extrasParams, double amount, string currency,string cpOrderID, Action<string> paySuccess = null)
        {
            //sdk
            paySuccess?.Invoke(extrasParams);
        }
    }
}