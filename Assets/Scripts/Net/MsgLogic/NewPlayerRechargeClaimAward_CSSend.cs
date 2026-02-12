using msg;

namespace Engine
{
    public class NewPlayerRechargeClaimAward_CSSend : ISender
    {
        public NewPlayerRechargeClaimAward_CS msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_NewPlayerRechargeClaimAward_CS;
        }

        public bool Build(object data)
        {
            msg = data as NewPlayerRechargeClaimAward_CS;
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