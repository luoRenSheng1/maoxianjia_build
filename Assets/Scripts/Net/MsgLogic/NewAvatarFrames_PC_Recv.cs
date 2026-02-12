using msg;

namespace Engine
{
    public class NewAvatarFrames_PC_Recv : IReceiver
    {
        public NewAvatarFrames_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_NewAvatarFrames_PC;
        }

        public void Process()
        {
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewAvatarFrames_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}