using msg;

namespace Engine
{
    using EngineBase;
    /// <summary>
    /// 引导免费十次抽技能
    /// </summary>
    public class FreeSkillLottery4Guide_CSSend : ISender
    {
        public FreeSkillLottery4Guide_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_FreeSkillLottery4Guide_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as FreeSkillLottery4Guide_CS;
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
