using msg;

namespace Engine
{
    using EngineBase;
    
    public class ClaimGiftAward_CSSend : ISender
    {
        public ClaimGiftAward_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimGiftAward_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ClaimGiftAward_CS;
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
