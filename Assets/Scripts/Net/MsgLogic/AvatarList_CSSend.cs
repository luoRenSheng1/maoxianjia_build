using msg;

namespace Engine
{
    public class AvatarList_CSSend : ISender
    {
        public AvatarList_CS msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_AvatarList_CS;
        }

        public bool Build(object data)
        {
            msg = data as AvatarList_CS;
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