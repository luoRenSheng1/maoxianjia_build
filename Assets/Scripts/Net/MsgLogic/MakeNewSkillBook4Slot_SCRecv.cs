using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class MakeNewSkillBook4Slot_SCRecv : IReceiver
    {
        public MakeNewSkillBook4Slot_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MakeNewSkillBook4Slot_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = MakeNewSkillBook4Slot_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}