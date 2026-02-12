using msg;

namespace Engine
{
    public class ExitRuin_CSSend : ISender
    {
        public ExitRuin_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ExitRuin_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ExitRuin_CS;
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