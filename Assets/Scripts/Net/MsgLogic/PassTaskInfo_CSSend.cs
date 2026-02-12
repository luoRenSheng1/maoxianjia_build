using msg;

namespace Engine
{
    using EngineBase;
    
    public class PassTaskInfo_CSSend : ISender
    {
        public PassTaskInfo_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PassTaskInfo_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as PassTaskInfo_CS;
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
