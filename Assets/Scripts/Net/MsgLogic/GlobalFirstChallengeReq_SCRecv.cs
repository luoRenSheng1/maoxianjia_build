using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GlobalFirstChallengeReq_SCRecv : IReceiver
    {
        public GlobalFirstChallengeReq_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalFirstChallengeReq_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                //没有数据下发，只用来看是否能正常进战斗
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GlobalFirstChallengeReq_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
