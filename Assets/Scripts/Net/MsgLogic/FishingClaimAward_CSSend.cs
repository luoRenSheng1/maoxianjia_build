using msg;

namespace Engine
{
    public class FishingClaimAward_CSSend : ISender
    {
        public FishingClaimAward_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_FishingClaimAward_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as FishingClaimAward_CS;
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