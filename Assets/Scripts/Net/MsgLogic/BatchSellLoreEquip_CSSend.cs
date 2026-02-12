using msg;

namespace Engine
{
    using EngineBase;
    
    public class BatchSellLoreEquip_CSSend : ISender
    {
        public BatchSellLoreEquip_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BatchSellLoreEquip_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as BatchSellLoreEquip_CS;
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
