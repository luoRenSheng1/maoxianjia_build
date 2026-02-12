using msg;

namespace Engine
{
    using EngineBase;
    
    public class MailExtract_CSSend : ISender
    {
        public MailExtract_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MailExtract_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as MailExtract_CS;
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
