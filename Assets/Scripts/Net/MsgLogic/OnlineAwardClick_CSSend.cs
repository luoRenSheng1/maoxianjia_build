using msg;

namespace Engine
{
    using EngineBase;
    
    public class OnlineAwardClick_CSSend : ISender
    {
        public OnlineAwardClick_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_OnlineAwardClick_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as OnlineAwardClick_CS;
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
