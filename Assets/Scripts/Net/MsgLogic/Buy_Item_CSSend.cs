using msg;

namespace Engine
{
    using EngineBase;
    
    public class Buy_Item_CSSend : ISender
    {
        public Buy_Item_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Buy_Item_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as Buy_Item_CS;
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
