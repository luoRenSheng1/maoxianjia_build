using msg;

namespace Engine
{
    using EngineBase;
    
    public class ClaimPassSubTaskExp_CSSend : ISender
    {
        public ClaimPassSubTaskExp_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimPassSubTaskExp_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ClaimPassSubTaskExp_CS;
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
