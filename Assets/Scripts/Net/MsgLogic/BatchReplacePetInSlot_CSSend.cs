using msg;

namespace Engine
{
    using EngineBase;
    
    public class BatchReplacePetInSlot_CSSend : ISender
    {
        public BatchReplacePetInSlot_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BatchReplacePetInSlot_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as BatchReplacePetInSlot_CS;
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
