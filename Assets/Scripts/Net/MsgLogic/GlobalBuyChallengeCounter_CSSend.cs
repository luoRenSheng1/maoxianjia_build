using msg;

namespace Engine
{
    using EngineBase;
    
    public class GlobalBuyChallengeCounter_CSSend : ISender
    {
        public GlobalBuyChallengeCounter_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalBuyChallengeCounter_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as GlobalBuyChallengeCounter_CS;
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
