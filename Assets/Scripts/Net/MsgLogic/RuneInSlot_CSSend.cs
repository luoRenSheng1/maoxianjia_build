using msg;

namespace Engine
{
    using EngineBase;
    
    public class RuneInSlot_CSSend : ISender
    {
        public RuneInSlot_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RuneInSlot_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as RuneInSlot_CS;
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
