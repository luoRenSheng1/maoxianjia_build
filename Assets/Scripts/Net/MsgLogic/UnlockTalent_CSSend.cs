using msg;

namespace Engine
{
    public class UnlockTalent_CSSend : ISender
    {
        public UnlockTalent_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_UnlockTalent_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as UnlockTalent_CS;
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