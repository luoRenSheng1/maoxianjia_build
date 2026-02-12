using msg;

namespace Engine
{
    using EngineBase;
    
    public class AccountCheck_CSSend : ISender
    {
        public AccountCheck_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AccountCheck_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as AccountCheck_CS;
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
