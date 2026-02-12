using msg;

namespace Engine
{
    using EngineBase;
    
    public class ClaimPassTaskAward_CSSend : ISender
    {
        public ClaimPassTaskAward_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimPassTaskAward_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ClaimPassTaskAward_CS;
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
