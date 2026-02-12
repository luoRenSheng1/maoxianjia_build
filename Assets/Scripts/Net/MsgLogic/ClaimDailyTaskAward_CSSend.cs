using msg;

namespace Engine
{
    using EngineBase;
    
    public class ClaimDailyTaskAward_CSSend : ISender
    {
        public ClaimDailyTaskAward_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimDailyTaskAward_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ClaimDailyTaskAward_CS;
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
