using msg;

namespace Engine
{

    public class PM_CSSend : ISender
    {
        public Pm_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Pm_CS;
        }
    
        public bool Build(object data)
        {
            msg = data as Pm_CS;
            return true;
        }

        public bool Send(BaseStructSend send)
        {
            send.obj = msg.ToByteArray();
            return true;
        }

        public PacketReliability GetPackageMode()
        {
            return PacketReliability.RELIABLE_ORDERED;
        }
    }
}