using msg;

namespace Engine
{
    using EngineBase;
    
    public class EnterChapterMap_CSSend : ISender
    {
        public EnterChapterMap_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_EnterChapterMap_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as EnterChapterMap_CS;
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
