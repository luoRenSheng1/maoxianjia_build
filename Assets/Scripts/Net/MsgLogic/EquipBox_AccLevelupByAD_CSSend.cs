using msg;

namespace Engine
{
    using EngineBase;
    
    public class EquipBox_AccLevelupByAD_CSSend : ISender
    {
        public EquipBox_AccLevelupByAD_CS msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_EquipBox_AccLevelupByAD_CS;
        }
        
        public bool Build(object data)
        {
            msg = data as EquipBox_AccLevelupByAD_CS;
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
