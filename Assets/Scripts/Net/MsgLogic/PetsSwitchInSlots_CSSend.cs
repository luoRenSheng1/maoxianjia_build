using msg;

namespace Engine
{
    using EngineBase;
    
    public class PetsSwitchInSlots_CSSend : ISender
    {
        public PetsSwitchInSlots_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetsSwitchInSlots_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as PetsSwitchInSlots_CS;
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
