using msg;

namespace Engine
{
    using EngineBase;
    
    public class MailList_CSSend : ISender
    {
        public MailList_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MailList_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as MailList_CS;
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
