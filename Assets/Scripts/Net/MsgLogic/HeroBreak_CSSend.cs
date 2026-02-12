using msg;

namespace Engine
{
    using EngineBase;
    
    public class HeroBreak_CSSend : ISender
    {
        public HeroBreak_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroBreak_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as HeroBreak_CS;
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
