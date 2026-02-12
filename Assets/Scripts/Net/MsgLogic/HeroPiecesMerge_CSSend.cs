using msg;

namespace Engine
{
    using EngineBase;
    
    public class HeroPiecesMerge_CSSend : ISender
    {
        public HeroPiecesMerge_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroPiecesMerge_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as HeroPiecesMerge_CS;
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
