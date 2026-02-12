using msg;

namespace Engine
{
    using EngineBase;
    
    public class DailyClaimHeroMonthAward_CSSend : ISender
    {
        public DailyClaimHeroMonthAward_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DailyClaimHeroMonthAward_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as DailyClaimHeroMonthAward_CS;
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
