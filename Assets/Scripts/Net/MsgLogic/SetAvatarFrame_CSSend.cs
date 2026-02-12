using msg;

namespace Engine
{
    public class SetAvatarFrame_CSSend : ISender
    {
        public SetAvatarFrame_CS msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_SetAvatarFrame_CS;
        }

        public bool Build(object data)
        {
            msg = data as SetAvatarFrame_CS;
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