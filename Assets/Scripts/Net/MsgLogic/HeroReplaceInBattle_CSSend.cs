using msg;

namespace Engine
{
    using EngineBase;
    
    public class HeroReplaceInBattle_CSSend : ISender
    {
        public HeroReplaceInBattle_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroReplaceInBattle_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as HeroReplaceInBattle_CS;
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
