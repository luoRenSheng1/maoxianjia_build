using msg;

namespace Engine
{
    public class HeroSkillLevelUp_CSSend : ISender
    {
        public HeroSkillLevelUp_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroSkillLevelUp_CS;
        }

        public bool Build(object data)
        {
            msg = data as HeroSkillLevelUp_CS;
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