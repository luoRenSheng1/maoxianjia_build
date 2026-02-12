using msg;

namespace Engine
{
    public class SetAvatarFrame_SCRecv : IReceiver
    {
        public SetAvatarFrame_SC msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_SetAvatarFrame_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                // throw new NotImplementedException();
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = SetAvatarFrame_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}