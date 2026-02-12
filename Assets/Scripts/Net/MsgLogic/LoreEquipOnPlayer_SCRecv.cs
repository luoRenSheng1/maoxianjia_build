using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class LoreEquipOnPlayer_SCRecv : IReceiver
    {
        public LoreEquipOnPlayer_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_LoreEquipOnPlayer_SC;
        }

        public void Process()
        {//穿上传承装备回复
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                //1、uint64 equip_guid = 2; //装备上的equip guid
                ulong equipGuid = msg.EquipGuid;   
                
                //2、uint64  replaced_equip_guid = 3; //替换下的equip guid，为0表示没有替换，原来栏位是空
                ulong replaceEquipGuid = msg.ReplacedEquipGuid;
                if (equipGuid > 0)
                {
                    EquipManager.Instance.GetEquipById(equipGuid).isWear = true;
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_LORE_EQUIP_EQUIP, equipGuid);
                }
                else
                {
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_LOREEQUIP_UPDATE);
                }
                
                //3、传承装备栏
                EquipSlotInfo slotInfo = new EquipSlotInfo();
                slotInfo.SlotType = (int)msg.EquipSlots.SlotType;
                slotInfo.equipGuid = msg.EquipSlots.EquipId;
                slotInfo.Status = msg.EquipSlots.SlotStatus;
                slotInfo.EquipCfgId = (int) msg.EquipSlots.BaseItemId;
                RoleManager.Instance.AddEquipSlotInfo(slotInfo);
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = LoreEquipOnPlayer_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
