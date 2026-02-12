using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class DailyTaskInfo_SCRecv : IReceiver
    {
        public DailyTaskInfo_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DailyTaskInfo_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                foreach (var item in msg.DailyTasksList)
                {
                    TaskInfoManager.Instance.UpdateDailyTaskInfo((int) item.TaskId, item.IsClaimAward, (int) item.TaskProcess);
                }
                TaskInfoManager.Instance.SetTotalActiveBox(msg.ActiveValue);

                foreach (var item in msg.ClaimedActiveAwardIdList)
                {
                    TaskInfoManager.Instance.SetActiveBoxGetRwId((int) item);
                }
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DAILY_TASK_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DailyTaskInfo_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
