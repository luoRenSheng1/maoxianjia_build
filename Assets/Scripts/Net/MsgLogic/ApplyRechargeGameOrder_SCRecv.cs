using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ApplyRechargeGameOrder_SCRecv : IReceiver
    {
        public ApplyRechargeGameOrder_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ApplyRechargeGameOrder_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(msg.PaylistId);
                UIManager.Instance.Pay(payListUnit,payListUnit.Id.ToString(), msg.GameOrderNo);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ApplyRechargeGameOrder_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
