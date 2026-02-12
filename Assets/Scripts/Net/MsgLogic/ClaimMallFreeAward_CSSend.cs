using msg;

namespace Engine
{
    using EngineBase;
    
    public class ClaimMallFreeAward_CSSend : ISender
    {
        public ClaimMallFreeAward_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimMallFreeAward_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ClaimMallFreeAward_CS;
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
