using msg;

namespace Engine
{
    using EngineBase;
    
    public class ClaimOnlineAward_CSSend : ISender
    {
        public ClaimOnlineAward_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimOnlineAward_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ClaimOnlineAward_CS;
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
