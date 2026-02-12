using msg;

namespace Engine
{
    using EngineBase;
    
    public class BeginRuinMosterFight_SCRecv : IReceiver
    {
        public BeginRuinMosterFight_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BeginRuinMosterFight_SC;
        }

        public void Process()
        {   // 开始打怪返回
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                // msg.EventGuid;//事件guid
                // msg.MonsterId;//选中挑战的boss

                // DungeonMapManager.Instance.GuanKaStage = 1;
                // var stageUnit = ConfigUtils.GetDungeonStageByNandu(DungeonMapManager.Instance.GuanKaStage, (int)DungeonType.RuinBoss);
                //
                // //进入挑战界面，发起挑战
                // UIManager.Instance.ShowUIPanel("DungeonMap", stageUnit);

                MapChapterManager.Instance.EnterRandomEventCopyBattle();

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
            msg = BeginRuinMosterFight_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}