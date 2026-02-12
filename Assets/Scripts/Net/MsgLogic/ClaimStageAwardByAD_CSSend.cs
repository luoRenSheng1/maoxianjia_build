using msg;

namespace Engine
{
    using EngineBase;
    
    public class ClaimStageAwardByAD_CSSend : ISender
    {
        public ClaimStageAwardByAD_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimStageAwardByAD_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ClaimStageAwardByAD_CS;
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
