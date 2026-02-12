using msg;

namespace Engine
{
    using EngineBase;
    
    public class CampEnter_CSSend : ISender
    {
        public CampEnter_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Camp_Enter_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as CampEnter_CS;
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
