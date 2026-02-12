using msg;

namespace Engine
{
    public class FishingFailed_CSSend : ISender
    {
        public FishingFailed_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_FishingFailed_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as FishingFailed_CS;
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