using msg;

namespace Engine
{
    using EngineBase;
    
    public class ClaimRandomFinance_CSSend : ISender
    {
        public ClaimRandomFinance_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimRandomFinance_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ClaimRandomFinance_CS;
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
