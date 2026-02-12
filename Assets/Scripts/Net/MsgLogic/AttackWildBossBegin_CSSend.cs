using msg;

namespace Engine
{
    using EngineBase;
    
    public class AttackWildBossBegin_CSSend : ISender
    {
        public AttackWildBossBegin_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AttackWildBossBegin_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as AttackWildBossBegin_CS;
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
