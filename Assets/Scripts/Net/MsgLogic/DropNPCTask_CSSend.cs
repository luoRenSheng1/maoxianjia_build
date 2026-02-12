using msg;

namespace Engine
{
    using EngineBase;
    
    public class DropNPCTask_CSSend : ISender
    {
        public DropNPCTask_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DropNPCTask_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as DropNPCTask_CS;
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
