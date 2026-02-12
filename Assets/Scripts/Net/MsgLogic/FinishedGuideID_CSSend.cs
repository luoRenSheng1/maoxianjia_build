using msg;

namespace Engine
{
    using EngineBase;
    
    public class FinishedGuideID_CSSend : ISender
    {
        public FinishedGuideID_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_FinishedGuideID_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as FinishedGuideID_CS;
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
