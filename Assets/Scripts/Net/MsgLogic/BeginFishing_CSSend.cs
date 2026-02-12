using msg;

namespace Engine
{
    public class BeginFishing_CSSend : ISender
    {
        public BeginFishing_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BeginFishing_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as BeginFishing_CS;
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