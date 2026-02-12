using msg;

namespace Engine
{
    using EngineBase;
    
    public class PetDispatched_CSSend : ISender
    {
        public PetDispatched_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetDispatched_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as PetDispatched_CS;
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
