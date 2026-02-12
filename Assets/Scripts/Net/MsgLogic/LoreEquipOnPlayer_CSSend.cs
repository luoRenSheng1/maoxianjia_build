using msg;

namespace Engine
{
    using EngineBase;
    
    public class LoreEquipOnPlayer_CSSend : ISender
    {
        public LoreEquipOnPlayer_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_LoreEquipOnPlayer_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as LoreEquipOnPlayer_CS;
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
