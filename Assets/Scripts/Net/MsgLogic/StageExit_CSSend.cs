using msg;

namespace Engine
{
    using EngineBase;
    
    public class StageExit_CSSend : ISender
    {
        public StageExit_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Stage_Exit_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as StageExit_CS;
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
