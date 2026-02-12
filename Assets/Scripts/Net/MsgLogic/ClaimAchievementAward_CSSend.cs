using msg;

namespace Engine
{
    public class ClaimAchievementAward_CSSend : ISender
    {
        public ClaimAchievementAward_CS msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_ClaimAchievementAward_CS;
        }

        public bool Build(object data)
        {
            msg = data as ClaimAchievementAward_CS;
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