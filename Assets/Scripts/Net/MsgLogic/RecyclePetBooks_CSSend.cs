using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class RecyclePetBooks_CSSend : ISender
    {
        public RecyclePetBooks_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RecyclePetBooks_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as RecyclePetBooks_CS;
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