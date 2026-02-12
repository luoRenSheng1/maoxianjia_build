using msg;

namespace Engine
{
    using EngineBase;
    
    public class HeroExpItemByFreeAD_CSSend : ISender
    {
        public HeroExpItemByFreeAD_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroExpItemByFreeAD_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as HeroExpItemByFreeAD_CS;
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
