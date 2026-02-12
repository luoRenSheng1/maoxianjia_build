using msg;

namespace Engine
{
    using EngineBase;
    
    public class TakeOffLoreEquip_CSSend : ISender
    {
        public TakeOffLoreEquip_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_TakeOffLoreEquip_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as TakeOffLoreEquip_CS;
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
