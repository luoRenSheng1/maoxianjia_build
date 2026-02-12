using msg;

namespace Engine
{
    using EngineBase;
    
    public class Chat_SCRecv : IReceiver
    {
        public Chat_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Chat_SC;
        }

        public void Process()
        {
            if (msg.ErrCode == eErrCode.eErrCode_Success)
            {

            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.ErrCode);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = Chat_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
