using msg;

namespace Engine
{
    public class ClaimDailyAwardOfRechargeAward_CSSend : ISender
    {
        public ClaimDailyAwardOfRechargeAward_CS msg;


        public int MsgID()
        {
            return (int) eMsgID.eMsg_ClaimDailyAwardOfRechargeAward_CS;
        }

        public bool Build(object data)
        {
            msg = data as ClaimDailyAwardOfRechargeAward_CS;
            return true;
        }

        public bool Send(BaseStructSend send)
        {
            send.obj = msg.ToByteArray();
            return true;
        }

        public PacketReliability GetPackageMode()
        {
            return PacketReliability.RELIABLE_ORDERED;
        }
    }
}