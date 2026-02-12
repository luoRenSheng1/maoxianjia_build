using msg;

namespace Engine
{
    using EngineBase;
    
    public class SkillInSlot_CSSend : ISender
    {
        public SkillInSlot_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_SkillInSlot_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as SkillInSlot_CS;
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
