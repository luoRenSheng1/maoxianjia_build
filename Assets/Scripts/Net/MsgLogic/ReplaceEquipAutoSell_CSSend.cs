using msg;

namespace Engine
{
    using EngineBase;
    
    public class ReplaceEquipAutoSell_CSSend : ISender
    {
        public ReplaceEquipAutoSell_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ReplaceEquipAutoSell_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ReplaceEquipAutoSell_CS;
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
