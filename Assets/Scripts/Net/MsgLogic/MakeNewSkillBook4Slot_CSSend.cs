using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class MakeNewSkillBook4Slot_CSSend : ISender
    {
        public MakeNewSkillBook4Slot_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MakeNewSkillBook4Slot_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as MakeNewSkillBook4Slot_CS;
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