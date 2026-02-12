using msg;

namespace Engine
{
    using EngineBase;
    
    public class PetSkillLevelUp_CSSend : ISender
    {
        public PetSkillLevelUp_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetSkillLevelUp_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as PetSkillLevelUp_CS;
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