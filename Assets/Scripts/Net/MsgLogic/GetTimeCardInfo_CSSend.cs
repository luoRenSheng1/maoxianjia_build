using msg;

namespace Engine
{
    using EngineBase;
    
    public class GetTimeCardInfo_CSSend : ISender
    {
        public GetTimeCardInfo_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GetTimeCardInfo_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as GetTimeCardInfo_CS;
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
