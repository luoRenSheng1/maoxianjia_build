using msg;

namespace Engine
{
    using EngineBase;
    
    public class GetHeroMonthActivityInfo_CSSend : ISender
    {
        public GetHeroMonthActivityInfo_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GetHeroMonthActivityInfo_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as GetHeroMonthActivityInfo_CS;
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
