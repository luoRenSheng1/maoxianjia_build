using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class BatchRecyclePets_CSSend : ISender
    {
        public BatchRecyclePets_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BatchRecyclePets_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as BatchRecyclePets_CS;
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