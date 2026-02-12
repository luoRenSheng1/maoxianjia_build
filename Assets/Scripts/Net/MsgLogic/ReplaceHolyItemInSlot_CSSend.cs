using msg;

namespace Engine
{
    public class ReplaceHolyItemInSlot_CSSend : ISender
    {
        public ReplaceHolyItemInSlot_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ReplaceHolyItemInSlot_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ReplaceHolyItemInSlot_CS;
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