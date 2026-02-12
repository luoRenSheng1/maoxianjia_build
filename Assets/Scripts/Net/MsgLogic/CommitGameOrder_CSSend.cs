using msg;

namespace Engine
{
    using EngineBase;
    
    public class CommitGameOrder_CSSend : ISender
    {
        public CommitGameOrder_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_CommitGameOrder_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as CommitGameOrder_CS;
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
