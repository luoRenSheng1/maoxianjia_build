using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class PetShuffleTalent_CSSend : ISender
    {
        public PetShuffleTalent_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetShuffleTalent_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as PetShuffleTalent_CS;
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