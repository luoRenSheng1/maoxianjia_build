using System.Collections.Generic;
using System.Linq;
using Engine;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PassTaskFinish_PC_Recv : IReceiver
    {
        public PassTaskFinish_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PassTaskFinish_PC;
        }

        public void Process()
        {
            PassPortInfo passPortInfo = ActivityManager.Instance.GetPassPortInfo();
            if(passPortInfo == null) return;
            foreach (var item in passPortInfo.DailySubTasks)
            {
                if (item.TaskId == msg.SubTask.TaskId)
                {
                    item.Progress = (ulong) msg.SubTask.Process;
                    item.HasGetReward = msg.SubTask.HasClaimed;
                    break;
                }
            }
            
            foreach (var item in passPortInfo.WeekSubTasks)
            {
                if (item.TaskId == msg.SubTask.TaskId)
                {
                    item.Progress = (ulong) msg.SubTask.Process;
                    item.HasGetReward = msg.SubTask.HasClaimed;
                    break;
                }
            }

            passPortInfo.PassPortLv = msg.Level;
            passPortInfo.PassPortExp = msg.Exp;
            passPortInfo.CounterAfterTopLevel = msg.CounterAfterToplevel;
            passPortInfo.ClaimedCounterAfterTopLevel = msg.ClaimedCounterAfterToplevel;
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_PASSPORT);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PASSPORT_GetSubTask);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PassTaskFinish_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}