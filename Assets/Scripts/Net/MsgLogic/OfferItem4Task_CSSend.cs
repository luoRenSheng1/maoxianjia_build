using msg;

namespace Engine
{
    public class OfferItem4Task_CSSend : ISender
    {
        public OfferItem4Task_CS msg;
        
        public int MsgID()
        {
            return (int)eMsgID.eMsg_OfferItem4Task_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as OfferItem4Task_CS;
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