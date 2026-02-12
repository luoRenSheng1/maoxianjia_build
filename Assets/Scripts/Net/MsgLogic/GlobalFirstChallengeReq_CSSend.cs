using msg;

namespace Engine
{
    using EngineBase;
    
    public class GlobalFirstChallengeReq_CSSend : ISender
    {
        public GlobalFirstChallengeReq_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalFirstChallengeReq_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as GlobalFirstChallengeReq_CS;
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
