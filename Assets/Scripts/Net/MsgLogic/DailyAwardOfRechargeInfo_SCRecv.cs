using System.Collections.Generic;
using EngineBase;
using msg;

namespace Engine
{
    public class DailyAwardOfRechargeInfo_SCRecv : IReceiver
    {
        public DailyAwardOfRechargeInfo_SC msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_DailyAwardOfRechargeInfo_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                int getTag = 0;
                for (int day = 1; day <= (int)msg.RechargeDaysOfDailyaward; day++)
                {
                    if (msg.ClaimedRechargedDailyAwardList.Contains((uint)day))
                    {
                        getTag = 2;//已领取
                        ActivityManager.Instance.UpdateDailyAwardInfo(day,getTag);
                    }
                    else
                    {
                        getTag = 1;//未领取
                        ActivityManager.Instance.UpdateDailyAwardInfo(day,getTag);
                    }
                }

                ActivityManager.Instance.GetWaitingAssignDays((int)msg.WaitingAssignDays);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_FREE_GETITEM);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DailyAwardOfRechargeInfo_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}