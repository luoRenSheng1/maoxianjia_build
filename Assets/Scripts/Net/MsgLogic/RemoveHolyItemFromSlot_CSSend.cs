using msg;

namespace Engine
{
    public class RemoveHolyItemFromSlot_CSSend : ISender
    {
        public RemoveHolyItemFromSlot_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RemoveHolyItemFromSlot_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as RemoveHolyItemFromSlot_CS;
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