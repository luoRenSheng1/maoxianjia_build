using msg;

namespace Engine
{
    using EngineBase;
    
    public class DailyTaskInfo_CSSend : ISender
    {
        public DailyTaskInfo_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DailyTaskInfo_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as DailyTaskInfo_CS;
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
