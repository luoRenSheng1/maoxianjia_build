using msg;

namespace Engine
{
    using EngineBase;
    
    public class ClaimTaskAward_CSSend : ISender
    {
        public ClaimTaskAward_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimTaskAward_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ClaimTaskAward_CS;
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
