using msg;

namespace Engine
{
    using EngineBase;
    
    public class AttackWildBossEnd_CSSend : ISender
    {
        public AttackWildBossEnd_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AttackWildBossEnd_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as AttackWildBossEnd_CS;
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
