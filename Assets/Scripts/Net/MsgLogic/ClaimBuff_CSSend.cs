using msg;

namespace Engine
{
    using EngineBase;
    
    public class ClaimBuff_CSSend : ISender
    {
        public ClaimBuff_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimBuff_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ClaimBuff_CS;
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
