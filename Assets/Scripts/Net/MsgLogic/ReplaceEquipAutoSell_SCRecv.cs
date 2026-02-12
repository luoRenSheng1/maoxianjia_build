using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ReplaceEquipAutoSell_SCRecv : IReceiver
    {
        public ReplaceEquipAutoSell_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ReplaceEquipAutoSell_SC;
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

                EquipManager.Instance.DelEquip(msg.SoldEquipGuid);
                RoleData roleData = DataManager.Instance.GetRoleData();
                int preLv = roleData.lv;
                roleData.lv = msg.PlayerLevel;
                roleData.exp = msg.PlayerExp;
                DataManager.Instance.mRoleData.gold = msg.Finance.Golds;
                DataManager.Instance.mRoleData.dia = msg.Finance.Diamonds;
                DataManager.Instance.mRoleData.dia2 = msg.Finance.FreeDiamonds;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PART_DECOMPOSE_RES);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_SIGLE_EQUIP_WEAR);
                EquipManager.Instance.HasNewEquipToStopAutopack = false;
                
                if (roleData.lv > preLv)
                {
                    UIManager.Instance.ToastByKey(10161);
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_level_up);
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
            msg = ReplaceEquipAutoSell_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
