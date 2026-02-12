using msg;

namespace Engine
{
    using EngineBase;
    
    public class FishingFailed_SCRecv : IReceiver
    {
        public FishingFailed_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_FishingFailed_SC;
        }

        public void Process()
        {   // 钓鱼失败请求返回
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                // 今日钓鱼次数
                FinishEventCount finishEventCount = new FinishEventCount();
                finishEventCount.eventType = (int)StageEventType.Fishing;
                finishEventCount.finishCount = (int)msg.TodayFishingCounter;
                MapChapterManager.Instance.UpdateFinishEventCountListInfo(finishEventCount);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = FishingFailed_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}