using msg;

namespace Engine
{
    using EngineBase;
    
    public class ObtainProduct_CSSend : ISender
    {
        public ObtainProduct_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ObtainProduct_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ObtainProduct_CS;
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
