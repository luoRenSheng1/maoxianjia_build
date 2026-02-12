using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class RecyclePetBooks_SCRecv : IReceiver
    {
        public RecyclePetBooks_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RecyclePetBooks_SC;
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
            msg = RecyclePetBooks_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}