using msg;

namespace Engine
{
    using EngineBase;
    
    public class SellEquip_CSSend : ISender
    {
        public SellEquip_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_SellEquip_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as SellEquip_CS;
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
