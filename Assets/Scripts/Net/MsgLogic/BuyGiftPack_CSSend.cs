using msg;

namespace Engine
{
    using EngineBase;
    
    public class BuyGiftPack_CSSend : ISender
    {
        public BuyGiftPack_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BuyGiftPack_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as BuyGiftPack_CS;
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
