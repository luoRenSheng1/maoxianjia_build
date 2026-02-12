using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class AttackWildPetBegin_SCRecv : IReceiver
    {
        public AttackWildPetBegin_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AttackWildPetBegin_SC;
        }

        public void Process()
        {
            // 逃跑的宠物 开始攻击
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                MapChapterManager.Instance.EnterRandomEventCopyBattle();
                /*
                DungeonMapManager.Instance.GuanKaStage = 1;
                
                 ConfigDungeonChapterUnit dungeonChapterUnit = ConfigUtils.GetDungeonChapterById(DungeonType.BossWithPet);
                var stageUnit = ConfigUtils.GetDungeonStageByNandu(DungeonMapManager.Instance.GuanKaStage, (int)DungeonType.BossWithPet);
                
                // DungeonMapManager.Instance.EndTime = msg.BeginTime + (ulong)stageUnit.Time;
                // DungeonMapManager.Instance.normalStageId = (int)msg.ParamStageId;
                
                UIManager.Instance.ShowUIPanel("DungeonMap", stageUnit);
                
                // MapObjectManager.Instance.ClearBattleScreen();
                // MapObjectManager.Instance.InitGuanKaFSM();
                */
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = AttackWildPetBegin_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
