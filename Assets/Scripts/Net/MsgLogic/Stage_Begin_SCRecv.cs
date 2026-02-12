using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class Stage_Begin_SCRecv : IReceiver
    {
        public Stage_Begin_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Stage_Begin_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                int stageId = (int) msg.StartStageId;
                List<MonsterGroup> monsterAwards = new List<MonsterGroup>();
                foreach (var item in msg.MonsterGroupAwardsList)
                {
                    monsterAwards.Add(item);
                }
                MapObjectManager.Instance.SetMonsterGold(stageId, monsterAwards);
                
                DataManager.Instance.mRoleData.battleStatus = (int) msg.IsIdle;
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("Stage_Begin_SCRecv Read " + mRecv.Length);
            
            msg = Stage_Begin_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
