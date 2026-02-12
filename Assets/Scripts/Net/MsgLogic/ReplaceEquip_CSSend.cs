using msg;

namespace Engine
{
    using EngineBase;
    
    public class ReplaceEquip_CSSend : ISender
    {
        public ReplaceEquip_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ReplaceEquip_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ReplaceEquip_CS;
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
