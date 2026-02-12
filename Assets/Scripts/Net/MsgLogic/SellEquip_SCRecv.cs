using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class SellEquip_SCRecv : IReceiver
    {
        public SellEquip_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_SellEquip_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                EquipManager.Instance.DelEquip(msg.EquipGuid);
                RoleData roleData = DataManager.Instance.GetRoleData();
                int preLv = roleData.lv;
                roleData.lv = msg.PlayerLevel;
                roleData.exp = msg.PlayerExp;
                DataManager.Instance.mRoleData.gold = msg.AccountFinance.Golds;
                DataManager.Instance.mRoleData.dia = msg.AccountFinance.Diamonds;
                DataManager.Instance.mRoleData.dia2 = msg.AccountFinance.FreeDiamonds;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PART_DECOMPOSE_RES);
                EquipManager.Instance.HasNewEquipToStopAutopack = false;
                
                if (roleData.lv > preLv)
                {
                    UIManager.Instance.ToastByKey(10161);
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_level_up);
                }
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = SellEquip_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
