using msg;

namespace Engine
{
    public class UseBookItem_CSSend : ISender
    {
        public UseBookItem_CS msg;
        
        public int MsgID()
        {
            return (int)eMsgID.eMsg_UseBookItem_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as UseBookItem_CS;
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