using msg;

namespace Engine
{
    using EngineBase;
    
    public class PetLevelUp_CSSend : ISender
    {
        public PetLevelUp_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetLevelUp_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as PetLevelUp_CS;
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
