using msg;

namespace Engine
{
    using EngineBase;
    
    public class GetCopyFreeTimesByAD_CSSend : ISender
    {
        public GetCopyFreeTimesByAD_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GetCopyFreeTimesByAD_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as GetCopyFreeTimesByAD_CS;
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
