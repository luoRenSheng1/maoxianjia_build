using msg;

namespace Engine
{
    using EngineBase;
    
    public class GlobalFirstRank_CSSend : ISender
    {
        public GlobalFirstRank_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalFirstRank_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as GlobalFirstRank_CS;
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
