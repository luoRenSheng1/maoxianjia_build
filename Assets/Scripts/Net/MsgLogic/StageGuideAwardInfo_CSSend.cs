using msg;

namespace Engine
{
    public class StageGuideAwardInfo_CSSend : ISender
    {
        public StageGuideAwardInfo_CS msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_StageGuideAwardInfo_CS;
        }

        public bool Build(object data)
        {
            msg = data as StageGuideAwardInfo_CS;
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