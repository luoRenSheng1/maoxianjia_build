using msg;

namespace Engine
{
    using EngineBase;
    
    public class Stage_Failed_CSSend : ISender
    {
        public Stage_Failed_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Stage_Failed_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as Stage_Failed_CS;
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
