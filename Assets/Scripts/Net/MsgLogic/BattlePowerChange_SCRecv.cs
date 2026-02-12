using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class BattlePowerChange_SCRecv : IReceiver
    {
        public BattlePowerChange_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BattlePowerChange_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                //无需处理
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = BattlePowerChange_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
