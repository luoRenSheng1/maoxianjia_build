using msg;

namespace Engine
{
    using EngineBase;
    
    public class SkillLottery_CSSend : ISender
    {
        public SkillLottery_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_SkillLottery_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as SkillLottery_CS;
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
