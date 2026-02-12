using msg;

namespace Engine
{
    public class ClaimStageGuideAward_CSSend : ISender
    {
        public ClaimStageGuideAward_CS msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_ClaimStageGuideAward_CS;
        }

        public bool Build(object data)
        {
            msg = data as ClaimStageGuideAward_CS;
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