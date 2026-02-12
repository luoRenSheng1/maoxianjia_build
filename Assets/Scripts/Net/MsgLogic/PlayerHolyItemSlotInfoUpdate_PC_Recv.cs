using msg;

namespace Engine
{
    using EngineBase;
    
    public class PlayerHolyItemSlotInfoUpdate_PC_Recv : IReceiver
    {
        public PlayerHolyItemSlotInfoUpdate_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerHolyItemSlotInfoUpdate_PC;
        }

        public void Process()
        {
            foreach (var item in msg.HolyItemSlotsList)
            {
                HolySlotInfo holySlotInfo = new HolySlotInfo();
                holySlotInfo.SlotId = (int)item.SlotId;
                holySlotInfo.ItemId = (int)item.ItemId;
                holySlotInfo.Status = item.SlotStatus;
                RoleManager.Instance.AddHolySlotInfo(holySlotInfo);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_HolyInfo);
            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_HEROInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerHolyItemSlotInfoUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}