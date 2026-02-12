using msg;

namespace Engine
{
    using EngineBase;
    
    public class GetHometown_CSSend : ISender
    {
        public GetHometown_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Hometown_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as GetHometown_CS;
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
