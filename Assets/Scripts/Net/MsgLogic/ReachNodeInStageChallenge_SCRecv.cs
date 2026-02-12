using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ReachNodeInStageChallenge_SCRecv : IReceiver
    {
        public ReachNodeInStageChallenge_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ReachNodeInStageChallenge_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                DataManager.Instance.GetRoleData().subStageId = (int) msg.NodeId;
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ReachNodeInStageChallenge_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
