using msg;

namespace Engine
{
    using EngineBase;
    
    public class PetInSlot_CSSend : ISender
    {
        public PetInSlot_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetInSlot_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as PetInSlot_CS;
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
