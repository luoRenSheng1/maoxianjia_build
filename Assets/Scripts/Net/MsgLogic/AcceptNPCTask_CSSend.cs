using msg;

namespace Engine
{
    using EngineBase;
    
    public class AcceptNPCTask_CSSend : ISender
    {
        public AcceptNPCTask_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AcceptNPCTask_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as AcceptNPCTask_CS;
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
