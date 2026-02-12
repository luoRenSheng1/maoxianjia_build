using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class EquipBox_AccLevelupByAD_SCRecv : IReceiver
    {
        public EquipBox_AccLevelupByAD_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_EquipBox_AccLevelupByAD_SC;
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
                AdManager.Instance.SetAdFreeTime((int) ePlayerAttrID.ePlayerAttrID_BoxAccFreeTimes, (int) msg.BoxInfo.RemainFreeAdTimes);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TREASURE_PROGRESS_UPSUCC, preLv<roleData.equipBoxLv);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = EquipBox_AccLevelupByAD_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
