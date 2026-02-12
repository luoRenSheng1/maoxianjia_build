using System;
using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerNPCTask4Login_PC_Recv : IReceiver
    {
        public PlayerNPCTask4Login_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerNPCTask4Login_PC;
        }

        public void Process()
        {   //委托任务信息--如果数组为0或 任务id为0或结束时间大于当前时间，表示没有委托任务
            if (msg.NpcTasksList.Count > 0)
            {
                
                for (int i = 0; i < msg.NpcTasksList.Count; i++)
                {
                    // if (msg.NpcTasksList[i].TaskId > 0 && msg.NpcTasksList[i].EndTime > ServerTimeManager.Instance.CurServerTime)
                    // {
                    //     NpcTaskData taskData = new NpcTaskData();
                    //     taskData.taskId = (int)msg.NpcTasksList[i].TaskId;
                    //     taskData.progress = (int)msg.NpcTasksList[i].Process;
                    //     taskData.startTime = msg.NpcTasksList[i].StartTime;
                    //     taskData.endTime = msg.NpcTasksList[i].EndTime;
                    //     
                    //     TaskInfoManager.Instance.SetNpcTask(taskData);
                    // }
                    
                    // 服务器说：不管该事件任务是否过期，都下发该事件任务信息，客户端显示出任务信息
                    if (msg.NpcTasksList[i].TaskId > 0)
                    {
                        NpcTaskData taskData = new NpcTaskData();
                        taskData.taskId = (int)msg.NpcTasksList[i].TaskId;
                        taskData.progress = (int)msg.NpcTasksList[i].Process;
                        taskData.startTime = msg.NpcTasksList[i].StartTime;
                        taskData.endTime = msg.NpcTasksList[i].EndTime;
                        taskData.taskUnit = ConfigUtils.GetEventTaskUnitById((int)msg.NpcTasksList[i].TaskId);
                        
                        TaskInfoManager.Instance.SetNpcTask(taskData);
                    }
                    
                }

                //积分兑换信息
                foreach (var item in msg.ExchangeInfoList)
                {
                    PointExchangeInfo pointExchangeInfo = new PointExchangeInfo();
                    pointExchangeInfo.itemId = (int)item.ItemId;
                    pointExchangeInfo.changeCounter = (int)item.ChangeCounter;
                    TaskInfoManager.Instance.UpdateNpcTaskExchangeInfo(pointExchangeInfo);
                }
                
                // if (npcTaskDatas.Count > 0)
                // {
                //     TaskInfoManager.Instance.SetNpcTaskList(npcTaskDatas);
                // }
            }
            else
            {
                Debug.Log("");
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerNPCTask4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
