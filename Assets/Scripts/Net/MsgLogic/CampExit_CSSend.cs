using msg;

namespace Engine
{
    using EngineBase;
    
    public class CampExit_CSSend : ISender
    {
        public CampExit_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Camp_Exit_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as CampExit_CS;
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
