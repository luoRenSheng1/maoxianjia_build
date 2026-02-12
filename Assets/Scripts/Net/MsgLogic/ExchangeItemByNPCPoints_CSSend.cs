using msg;

namespace Engine
{
    public class ExchangeItemByNPCPoints_CSSend : ISender
    {
        public ExchangeItemByNPCPoints_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ExchangeItemByNPCPoints_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ExchangeItemByNPCPoints_CS;
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