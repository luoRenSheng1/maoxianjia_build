using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class SellLoreEquip_SCRecv : IReceiver
    {
        public SellLoreEquip_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_SellLoreEquip_SC;
        }

        public void Process()
        {//卖的装备-成功的话，客户端可以删除
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GoldPickupSE);
                
                //这个装备 移除
                EquipManager.Instance.DelEquip(msg.EquipGuid);
                
                PlayerAttrUtils.UpdateFinance(msg.AccountFinance);
                ulong equipGuid = msg.EquipGuid; 
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_LOREEQUIP_UPDATE);
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_LORE_EQUIP_REMOVE, equipGuid);
                
                // if (equipGuid > 0)
                // {
                //     EquipManager.Instance.GetEquipById(equipGuid).isWear = false;
                //     EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_LORE_EQUIP_REMOVE, equipGuid);
                //     EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_LOREEQUIP_UPDATE);
                // }
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = SellLoreEquip_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
