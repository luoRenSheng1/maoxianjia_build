using msg;

namespace Engine
{
    using EngineBase;
    
    public class HeroDailyFreeLottery_CSSend : ISender
    {
        public HeroDailyFreeLottery_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroDailyFreeLottery_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as HeroDailyFreeLottery_CS;
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
