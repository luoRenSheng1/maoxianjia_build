using msg;

namespace Engine
{
    public class DelAvatarFrames_PC_Recv : IReceiver
    {
        public DelAvatarFrames_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DelAvatarFrames_PC;
        }

        public void Process()
        {
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DelAvatarFrames_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}