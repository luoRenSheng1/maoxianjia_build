using msg;

namespace Engine
{
    public class SetAvatar_CSSend : ISender
    {
        public SetAvatar_CS msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_SetAvatar_CS;
        }

        public bool Build(object data)
        {
            msg = data as SetAvatar_CS;
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