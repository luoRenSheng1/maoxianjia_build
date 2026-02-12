using msg;

namespace Engine
{
    using EngineBase;
    
    public class Stage_AwardCommit_CSSend : ISender
    {
        public Stage_AwardCommit_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Stage_AwardCommit_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as Stage_AwardCommit_CS;
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
