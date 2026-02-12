using msg;

namespace Engine
{
    using EngineBase;
    /// <summary>
    /// 深埋宝藏 战斗结束
    /// </summary>
    public class AttackBoxBossEnd_CSSend : ISender
    {
        public AttackBoxBossEnd_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AttackBoxBossEnd_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as AttackBoxBossEnd_CS;
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
