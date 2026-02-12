using msg;

namespace Engine
{
    using EngineBase;
    
    public class ReadRedPoints_CSSend : ISender
    {
        public ReadRedPoints_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ReadRedPoints_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ReadRedPoints_CS;
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
