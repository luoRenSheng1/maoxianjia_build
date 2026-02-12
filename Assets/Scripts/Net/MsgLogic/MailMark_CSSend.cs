using msg;

namespace Engine
{
    using EngineBase;
    
    public class MailMark_CSSend : ISender
    {
        public MailMark_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MailMark_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as MailMark_CS;
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
