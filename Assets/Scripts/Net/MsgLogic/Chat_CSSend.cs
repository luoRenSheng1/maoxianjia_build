using msg;

namespace Engine
{
    using EngineBase;
    
    public class Chat_CSSend : ISender
    {
        public Chat_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Chat_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as Chat_CS;
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
