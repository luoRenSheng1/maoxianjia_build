using msg;

namespace Engine
{
    using EngineBase;
    
    public class ReplaceLoreEquip_CSSend : ISender
    {
        public ReplaceLoreEquip_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ReplaceLoreEquip_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ReplaceLoreEquip_CS;
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
