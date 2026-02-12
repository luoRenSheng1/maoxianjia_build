using msg;

namespace Engine
{
    public class ChooseStage_CSSend : ISender
    {
        public ChooseStage_CS msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_ChooseStage_CS;
        }

        public bool Build(object data)
        {
            msg = data as ChooseStage_CS;
            return true;
        }

        public bool Send(BaseStructSend send)
        {
            send.obj = msg.ToByteArray();
            return true;
        }

        public PacketReliability GetPackageMode()
        {
            return PacketReliability.RELIABLE_ORDERED;
        }
    }
}