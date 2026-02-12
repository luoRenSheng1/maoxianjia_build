using msg;

namespace Engine
{
    public class AvatarFrameList_SCRecv : IReceiver
    {
        public AvatarFrameList_SC msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_AvatarFrameList_SC;
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
            msg = AvatarFrameList_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}