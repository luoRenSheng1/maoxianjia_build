using msg;

namespace Engine
{
    using EngineBase;
    
    public class DiamondExchange4Gold_CSSend : ISender
    {
        public DiamondExchange4Gold_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DiamondExchange4Gold_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as DiamondExchange4Gold_CS;
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
