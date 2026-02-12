using msg;

namespace Engine
{
    using EngineBase;
    
    public class ReachNodeInStageChallenge_CSSend : ISender
    {
        public ReachNodeInStageChallenge_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ReachNodeInStageChallenge_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ReachNodeInStageChallenge_CS;
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
