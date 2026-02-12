using msg;

namespace Engine
{
    using EngineBase;
    
    public class PetSkillBookInfo_CSSend : ISender
    {
        public PetSkillBookInfo_CS msg;
        
        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetSkillBookInfo_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as PetSkillBookInfo_CS;
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