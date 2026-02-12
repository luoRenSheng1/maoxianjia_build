using msg;

namespace Engine
{
    public class AttackFishingBossEnd_CSSend : ISender
    {
        public AttackFishingBossEnd_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AttackFishingBossEnd_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as AttackFishingBossEnd_CS;
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