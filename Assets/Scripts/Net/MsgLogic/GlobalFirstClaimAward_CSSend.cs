using msg;

namespace Engine
{
    using EngineBase;
    
    public class GlobalFirstClaimAward_CSSend : ISender
    {
        public GlobalFirstClaimAward_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalFirstClaimAward_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as GlobalFirstClaimAward_CS;
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
