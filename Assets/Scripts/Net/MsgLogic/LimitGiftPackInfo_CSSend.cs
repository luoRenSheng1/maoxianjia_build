using msg;

namespace Engine
{
    using EngineBase;
    
    public class LimitGiftPackInfo_CSSend : ISender
    {
        public LimitGiftPackInfo_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_LimitGiftPackInfo_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as LimitGiftPackInfo_CS;
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
