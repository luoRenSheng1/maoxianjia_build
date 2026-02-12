using msg;

namespace Engine
{
    public class ResetTalents_CSSend : ISender
    {
        public ResetTalents_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ResetTalents_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ResetTalents_CS;
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