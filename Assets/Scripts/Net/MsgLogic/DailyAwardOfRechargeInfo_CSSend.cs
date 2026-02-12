using msg;

namespace Engine
{
    public class DailyAwardOfRechargeInfo_CSSend : ISender
    {
        public DailyAwardOfRechargeInfo_CS msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_DailyAwardOfRechargeInfo_CS;
        }

        public bool Build(object data)
        {
            msg = data as DailyAwardOfRechargeInfo_CS;
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