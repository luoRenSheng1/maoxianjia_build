using System.Collections.Generic;
using msg;

namespace Engine
{
    using EngineBase;
    
    public class EnterRuin_SCRecv : IReceiver
    {
        public EnterRuin_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_EnterRuin_SC;
        }

        public void Process()
        {   // 进入废墟返回
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                MapChapterManager.Instance.ResetRuinState();//重置遗迹地图状态
                
                RandomEventData randomEventData = new RandomEventData();
                randomEventData = MapChapterManager.Instance.GetRandomEventDataByGuid(msg.EventGuid);
                MapChapterManager.Instance._ruinEventData = randomEventData;
                
                //生成的怪物id
                List<RandomEventMonster> randomEventMonsterList = new List<RandomEventMonster>();
                foreach (var monster in msg.MonstersList)
                {
                    RandomEventMonster randomEventMonster = new RandomEventMonster();
                    randomEventMonster.guid = msg.EventGuid;
                    randomEventMonster.eventStageId = (int)monster.EventStageId;
                    randomEventMonster.indexId = (int)monster.IndexId;
                    randomEventMonsterList.Add(randomEventMonster);
                }
                MapChapterManager.Instance.AddRuinBossInfo(randomEventMonsterList);

                MapChapterManager.Instance.HideMapObject();
                UIManager.Instance.ShowUIPanel("RuinMap", msg.EventGuid, randomEventMonsterList);
                
                MapChapterManager.Instance.exitFlag = true;

                //通知刷新
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ENTER_AND_EXIT_RUIN);//标签栏显示
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = EnterRuin_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}