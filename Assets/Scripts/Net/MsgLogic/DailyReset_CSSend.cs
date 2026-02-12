using msg;

namespace Engine
{
    using EngineBase;
    
    public class DailyReset_CSSend : ISender
    {
        public DailyReset_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DailyReset_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as DailyReset_CS;
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
