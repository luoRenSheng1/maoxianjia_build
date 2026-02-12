using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class UnlockPetBookSlot_CSSend : ISender
    {
        public UnlockPetBookSlot_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_UnlockPetBookSlot_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as UnlockPetBookSlot_CS;
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