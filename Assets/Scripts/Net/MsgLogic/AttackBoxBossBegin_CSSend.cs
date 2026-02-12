using msg;

namespace Engine
{
    using EngineBase;
    /// <summary>
    /// 深埋宝藏 宝箱结果是传承boss,则发起战斗
    /// </summary>
    public class AttackBoxBossBegin_CSSend : ISender
    {
        public AttackBoxBossBegin_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AttackBoxBossBegin_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as AttackBoxBossBegin_CS;
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
