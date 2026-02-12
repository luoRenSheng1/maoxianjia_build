using msg;

namespace Engine
{
    using EngineBase;
    
    public class PlayerChangeHero_CSSend : ISender
    {
        public PlayerChangeHero_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerChangeHero_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as PlayerChangeHero_CS;
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
