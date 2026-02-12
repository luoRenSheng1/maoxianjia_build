using msg;

namespace Engine
{
    using EngineBase;
    
    public class RemoveRuneFromSlot_CSSend : ISender
    {
        public RemoveRuneFromSlot_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RemoveRuneFromSlot_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as RemoveRuneFromSlot_CS;
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
