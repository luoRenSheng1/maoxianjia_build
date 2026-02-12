using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ReplaceEquip_SCRecv : IReceiver
    {
        public ReplaceEquip_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ReplaceEquip_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                EquipSlotInfo slotInfo = new EquipSlotInfo();
                slotInfo.SlotType = (int)msg.EquipSlots.SlotType;
                slotInfo.equipGuid = msg.EquipSlots.EquipId;
                slotInfo.Status = msg.EquipSlots.SlotStatus;
                slotInfo.EquipCfgId = (int) msg.EquipSlots.BaseItemId;
                RoleManager.Instance.AddEquipSlotInfo(slotInfo);
                ulong equipGuid = msg.EquipGuid;
                if (equipGuid > 0)
                {
                    EquipManager.Instance.GetEquipById(equipGuid).isWear = false;
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_EQUIP_CHANGE, equipGuid);
                }
                else
                {
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_SIGLE_EQUIP_WEAR);
                }
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ReplaceEquip_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
