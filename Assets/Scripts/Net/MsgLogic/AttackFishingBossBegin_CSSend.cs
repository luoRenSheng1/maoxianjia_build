using msg;

namespace Engine
{
    public class AttackFishingBossBegin_CSSend : ISender
    {
        public AttackFishingBossBegin_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AttackFishingBossBegin_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as AttackFishingBossBegin_CS;
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