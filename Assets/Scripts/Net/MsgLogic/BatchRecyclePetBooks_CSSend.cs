using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class BatchRecyclePetBooks_CSSend : ISender
    {
        public BatchRecyclePetBooks_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BatchRecyclePetBooks_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as BatchRecyclePetBooks_CS;
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