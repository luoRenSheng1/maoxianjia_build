using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class AttackWildBossBegin_SCRecv : IReceiver
    {
        public AttackWildBossBegin_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AttackWildBossBegin_SC;
        }

        public void Process()
        {
            // 野外boss 开始攻击
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                MapChapterManager.Instance.EnterRandomEventCopyBattle();
                /*
                DungeonMapManager.Instance.GuanKaStage = 1;
                
                ConfigDungeonChapterUnit dungeonChapterUnit = ConfigUtils.GetDungeonChapterById(DungeonType.BossWithPet);
                var stageUnit = ConfigUtils.GetDungeonStageByNandu(DungeonMapManager.Instance.GuanKaStage, (int)DungeonType.BossWithPet);

                //DungeonMapManager.Instance.EndTime = ServerTimeManager.Instance.CurServerTime + (ulong)stageUnit.Time;//msg.BeginTime + (ulong)stageUnit.Time;
                // DungeonMapManager.Instance.normalStageId = (int)msg.ParamStageId;
                
                UIManager.Instance.ShowUIPanel("DungeonMap", stageUnit);
                
                // MapObjectManager.Instance.ClearBattleScreen();
                // MapObjectManager.Instance.InitGuanKaFSM();
                */
            }
            else
            {
                DungeonMapManager.Instance.mapEventData = null;
                // 事件时间到期，也要刷新怪物状态
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = AttackWildBossBegin_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
