using msg;

namespace Engine
{
    using EngineBase;
    
    public class HeroAddExp_CSSend : ISender
    {
        public HeroAddExp_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroAddExp_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as HeroAddExp_CS;
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
