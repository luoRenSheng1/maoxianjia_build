using System.Collections.Generic;
using msg;

namespace Engine
{
    using EngineBase;
    
    public class RechargeInfo_SCRecv : IReceiver
    {
        public RechargeInfo_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RechargeInfo_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                foreach (var payListId in msg.RechargedIdList)
                {
                    RoleManager.Instance.SetRechargeFlag(payListId, true);
                }

            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = RechargeInfo_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
