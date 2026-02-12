using msg;

namespace Engine
{
    using EngineBase;
    
    public class RemoveSkillFromSlot_CSSend : ISender
    {
        public RemoveSkillFromSlot_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RemoveSkillFromSlot_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as RemoveSkillFromSlot_CS;
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
