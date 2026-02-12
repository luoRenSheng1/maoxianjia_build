using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class RecyclePets_CSSend : ISender
    {
        public RecyclePets_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RecyclePets_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as RecyclePets_CS;
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