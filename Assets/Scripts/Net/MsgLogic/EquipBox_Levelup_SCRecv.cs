using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class EquipBox_Levelup_SCRecv : IReceiver
    {
        public EquipBox_Levelup_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_EquipBox_Levelup_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                RoleData roleData = DataManager.Instance.GetRoleData();
                int preLv = roleData.equipBoxLv;
                roleData.equipBoxLv = msg.BoxInfo.BoxLevel;
                roleData.equipBoxExp = msg.BoxInfo.BoxLevelupMilestone;
                roleData.equipBoxLvUpTime = msg.BoxInfo.NextLevelupTime;
                
                DataManager.Instance.mRoleData.gold = msg.AccountFinance.Golds;
                DataManager.Instance.mRoleData.dia = msg.AccountFinance.Diamonds;
                DataManager.Instance.mRoleData.dia2 = msg.AccountFinance.FreeDiamonds;
                TreasureChesManager.Instance.PushLevelUpSuccess();
                TreasureChesManager.Instance.StartTreasureCd();
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TREASURE_PROGRESS_UPSUCC, preLv<roleData.equipBoxLv);
            }   
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = EquipBox_Levelup_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
