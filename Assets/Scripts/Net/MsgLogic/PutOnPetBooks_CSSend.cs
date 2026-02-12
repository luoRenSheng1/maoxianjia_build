using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class PutOnPetBooks_CSSend : ISender
    {
        public PutOnPetBooks_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PutOnPetBooks_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as PutOnPetBooks_CS;
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