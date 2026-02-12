using msg;

namespace Engine
{
    public class FishingClaimAward_SCRecv : IReceiver
    {
        public FishingClaimAward_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_FishingClaimAward_SC;
        }

        public void Process()
        {   // 钓鱼结果是直接奖励领取回复
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                PlayerAttrUtils.UpdateFinance(msg.Finance);  //更新 最新账户余额
                
                // 今日钓鱼次数
                // MapChapterManager.Instance.UpdateFishingCount((int)msg.TodayFishingCounter);
                FinishEventCount finishEventCount = new FinishEventCount();
                finishEventCount.eventType = (int)StageEventType.Fishing;
                finishEventCount.finishCount = (int)msg.TodayFishingCounter;
                MapChapterManager.Instance.UpdateFinishEventCountListInfo(finishEventCount);
                
                //通知刷新
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DELEGATE_TASK_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = FishingClaimAward_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}