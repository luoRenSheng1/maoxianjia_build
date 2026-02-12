using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ReplaceLoreEquip_SCRecv : IReceiver
    {
        public ReplaceLoreEquip_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ReplaceLoreEquip_SC;
        }

        public void Process()
        {//换下传承装备回复
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                //1、uint64 equip_guid = 2; //脱下的equip guid
                ulong equipGuid = msg.EquipGuid;   
                
                //2、optional PlayerEquipSlotsInfo equip_slots = 3;  //装备栏状信息
                EquipSlotInfo slotInfo = new EquipSlotInfo();
                slotInfo.SlotType = (int)msg.EquipSlots.SlotType;
                slotInfo.equipGuid = msg.EquipSlots.EquipId;
                slotInfo.Status = msg.EquipSlots.SlotStatus;
                slotInfo.EquipCfgId = (int) msg.EquipSlots.BaseItemId;
                RoleManager.Instance.AddEquipSlotInfo(slotInfo);

                if (equipGuid > 0)
                {
                    EquipManager.Instance.GetEquipById(equipGuid).isWear = false;
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_LORE_EQUIP_EQUIP, equipGuid);
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_LOREEQUIP_UPDATE);
                }
                else
                {
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_LOREEQUIP_UPDATE);
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
            msg = ReplaceLoreEquip_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
