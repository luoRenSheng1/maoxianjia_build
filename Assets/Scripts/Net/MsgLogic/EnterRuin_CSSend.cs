using msg;

namespace Engine
{
    public class EnterRuin_CSSend : ISender
    {
        public EnterRuin_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_EnterRuin_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as EnterRuin_CS;
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