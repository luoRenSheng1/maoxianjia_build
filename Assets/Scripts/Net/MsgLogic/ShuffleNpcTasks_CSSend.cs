using msg;

namespace Engine
{
    public class ShuffleNpcTasks_CSSend : ISender
    {
        public ShuffleNpcTasks_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ShuffleNpcTasks_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ShuffleNpcTasks_CS;
            return true;
        }

        public bool Send(BaseStructSend send)
        {
            send.obj = msg.ToByteArray();;
            return true;
        }

        public PacketReliability GetPackageMode()
        {
            return PacketReliability.RELIABLE_ORDERED;
        }
    }
}