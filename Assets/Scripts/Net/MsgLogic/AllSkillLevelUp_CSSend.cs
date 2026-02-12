using msg;

namespace Engine
{
    using EngineBase;
    
    public class AllSkillLevelUp_CSSend : ISender
    {
        public AllSkillLevelUp_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AllSkillLevelUp_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as AllSkillLevelUp_CS;
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
