using msg;

namespace Engine
{
    using EngineBase;
    
    public class MailSend_CSSend : ISender
    {
        public MailSend_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MailSend_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as MailSend_CS;
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
