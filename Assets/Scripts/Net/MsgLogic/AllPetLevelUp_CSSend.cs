using msg;

namespace Engine
{
    using EngineBase;
    
    public class AllPetLevelUp_CSSend : ISender
    {
        //public AllPetLevelUp_CS msg;

        public int MsgID()
        {
            return 0; //(int)eMsgID.eMsg_AllPetLevelUp_CS;
        }
        
        public bool Build(object data)
        {
            //msg = data as AllPetLevelUp_CS;
            return true;
        }

        public bool Send(BaseStructSend send)
        {
            //send.obj = msg.ToByteArray();;
            return true;
        }

        public PacketReliability GetPackageMode()
        {
            return PacketReliability.RELIABLE_ORDERED;
        }
    }
}
