using Config;
using msg;

namespace Engine
{
    using EngineBase;
    
    public class AttackFishingBossBegin_SCRecv : IReceiver
    {
        public AttackFishingBossBegin_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AttackFishingBossBegin_SC;
        }

        public void Process()
        {   // 钓鱼boss开始攻击回复
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                // MapChapterManager.Instance.EnterRandomEventCopyBattle();
                
                DungeonMapManager.Instance.GuanKaStage = 1;
                if (DungeonMapManager.Instance.mapEventData != null)
                {
                    int cfgId = 0;
                    if (DungeonMapManager.Instance.batchStuffId == 0)
                    {
                        // 普通boss
                        foreach (var stuffData in DungeonMapManager.Instance.mapEventData.batchStuffList)
                        {
                            if (stuffData.id == DungeonMapManager.Instance.batchStuffId)
                            {
                                cfgId = stuffData.cfgId;
                                break;
                            }
                        }

                        ConfigStageMonsterAttrUnit stageMonsterAttrUnit = ConfigUtils.GetStageMonsterAttrUnitByIndexId(cfgId);
                        UIManager.Instance.ShowUIPanel("DungeonMap", FishingBossType.CommomBoss, stageMonsterAttrUnit);
                    }

                    if (DungeonMapManager.Instance.batchStuffId == 1)
                    {
                        //传承boss
                        foreach (var stuffData in DungeonMapManager.Instance.mapEventData.batchStuffList)
                        {
                            if (stuffData.id == DungeonMapManager.Instance.batchStuffId)
                            {
                                cfgId = stuffData.cfgId;
                                break;
                            }
                        }
                        
                        ConfigEventStageUnit stageUnit = ConfigUtils.GetEventStageUnitById(cfgId);
                        UIManager.Instance.ShowUIPanel("DungeonMap", FishingBossType.InheritBoss, stageUnit);
                    }
                }
                
                //通知刷新
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DELEGATE_TASK_UPDATE);
            }
            else
            {
                DungeonMapManager.Instance.mapEventData = null;
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = AttackFishingBossBegin_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}