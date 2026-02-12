using msg;

namespace Engine
{
    using EngineBase;
    
    public class ClientPing_CSSend : ISender
    {
        public ClientPing_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClientPing_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ClientPing_CS;
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
