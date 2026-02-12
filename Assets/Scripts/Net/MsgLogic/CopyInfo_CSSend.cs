using msg;

namespace Engine
{
    using EngineBase;
    
    public class CopyInfo_CSSend : ISender
    {
        public CopyInfo_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_CopyInfo_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as CopyInfo_CS;
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
