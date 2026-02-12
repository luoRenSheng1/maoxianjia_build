using msg;

namespace Engine
{
    using EngineBase;
    
    public class ChangePlayerName_CSSend : ISender
    {
        public ChangePlayerName_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ChangePlayerName_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ChangePlayerName_CS;
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
