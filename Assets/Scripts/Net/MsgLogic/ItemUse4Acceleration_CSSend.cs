using msg;

namespace Engine
{
    using EngineBase;
    
    public class ItemUse4Acceleration_CSSend : ISender
    {
        public ItemUse4Acceleration_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ItemUse4Acceleration_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as ItemUse4Acceleration_CS;
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
