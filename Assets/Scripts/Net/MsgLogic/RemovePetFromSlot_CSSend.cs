using msg;

namespace Engine
{
    using EngineBase;
    
    public class RemovePetFromSlot_CSSend : ISender
    {
        public RemovePetFromSlot_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RemovePetFromSlot_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as RemovePetFromSlot_CS;
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
