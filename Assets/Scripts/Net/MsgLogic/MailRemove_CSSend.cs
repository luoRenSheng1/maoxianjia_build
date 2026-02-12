using msg;

namespace Engine
{
    using EngineBase;
    
    public class MailRemove_CSSend : ISender
    {
        public MailRemove_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MailRemove_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as MailRemove_CS;
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
