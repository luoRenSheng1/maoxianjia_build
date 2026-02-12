using msg;

namespace Engine
{
    using EngineBase;
    
    public class GlobalFirstTakeOverRank_CSSend : ISender
    {
        public GlobalFirstTakeOverRank_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalFirstTakeOverRank_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as GlobalFirstTakeOverRank_CS;
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
