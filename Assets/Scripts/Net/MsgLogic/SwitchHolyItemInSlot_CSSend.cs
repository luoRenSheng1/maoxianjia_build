using msg;

namespace Engine
{
    public class SwitchHolyItemInSlot_CSSend : ISender
    {
        public SwitchHolyItemInSlot_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_SwitchHolyItemInSlot_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as SwitchHolyItemInSlot_CS;
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