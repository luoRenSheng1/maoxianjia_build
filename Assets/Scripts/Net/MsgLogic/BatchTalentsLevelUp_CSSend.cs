using msg;

namespace Engine
{
    public class BatchTalentsLevelUp_CSSend : ISender
    {
        public BatchTalentsLevelUp_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BatchTalentsLevelUp_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as BatchTalentsLevelUp_CS;
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