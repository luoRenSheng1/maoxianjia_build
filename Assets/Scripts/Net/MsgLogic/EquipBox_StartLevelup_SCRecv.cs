using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class EquipBox_StartLevelup_SCRecv : IReceiver
    {
        public EquipBox_StartLevelup_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_EquipBox_StartLevelup_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                RoleData roleData = DataManager.Instance.GetRoleData();
                roleData.equipBoxLv = msg.BoxInfo.BoxLevel;
                roleData.equipBoxExp = msg.BoxInfo.BoxLevelupMilestone;
                roleData.equipBoxLvUpTime = msg.BoxInfo.NextLevelupTime;
                
                DataManager.Instance.mRoleData.gold = msg.AccountFinance.Golds;
                DataManager.Instance.mRoleData.dia = msg.AccountFinance.Diamonds;
                DataManager.Instance.mRoleData.dia2 = msg.AccountFinance.FreeDiamonds;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);
                TreasureChesManager.Instance.PushLevelUpSuccess();
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = EquipBox_StartLevelup_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
