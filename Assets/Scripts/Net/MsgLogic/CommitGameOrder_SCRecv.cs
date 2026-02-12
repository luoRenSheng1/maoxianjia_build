using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class CommitGameOrder_SCRecv : IReceiver
    {
        public CommitGameOrder_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_CommitGameOrder_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.BuySuccessSE);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = CommitGameOrder_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
