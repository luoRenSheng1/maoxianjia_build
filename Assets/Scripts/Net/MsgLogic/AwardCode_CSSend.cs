using msg;

namespace Engine
{
    using EngineBase;
    
    public class AwardCode_CSSend : ISender
    {
        public AwardCode_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AwardCode_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as AwardCode_CS;
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
