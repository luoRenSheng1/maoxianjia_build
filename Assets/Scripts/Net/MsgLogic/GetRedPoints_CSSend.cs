using msg;

namespace Engine
{
    using EngineBase;
    
    public class GetRedPoints_CSSend : ISender
    {
        public GetRedPoints_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GetRedPoints_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as GetRedPoints_CS;
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
