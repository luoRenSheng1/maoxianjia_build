using msg;

namespace Engine
{
    using EngineBase;
    
    public class Copy_End_CSSend : ISender
    {
        public Copy_End_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Copy_End_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as Copy_End_CS;
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
