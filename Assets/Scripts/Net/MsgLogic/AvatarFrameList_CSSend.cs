using msg;

namespace Engine
{
    public class AvatarFrameList_CSSend : ISender
    {
        public AvatarFrameList_CS msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_AvatarFrameList_CS;
        }

        public bool Build(object data)
        {
            msg = data as AvatarFrameList_CS;
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