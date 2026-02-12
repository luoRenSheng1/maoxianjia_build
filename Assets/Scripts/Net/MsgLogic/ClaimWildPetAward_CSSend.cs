using msg;

namespace Engine
{
    using EngineBase;
    
    public class ClaimWildPetAward_CSSend : ISender
    {
        public ClaimWildPetAward_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimWildPetAward_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ClaimWildPetAward_CS;
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
