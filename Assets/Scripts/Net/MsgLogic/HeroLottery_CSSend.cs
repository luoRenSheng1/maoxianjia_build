using msg;

namespace Engine
{
    using EngineBase;
    
    public class HeroLottery_CSSend : ISender
    {
        public HeroLottery_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroLottery_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as HeroLottery_CS;
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
