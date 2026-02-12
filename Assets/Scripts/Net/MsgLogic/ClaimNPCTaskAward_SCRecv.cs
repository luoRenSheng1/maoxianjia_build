using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ClaimNPCTaskAward_SCRecv : IReceiver
    {
        public ClaimNPCTaskAward_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimNPCTaskAward_SC;
        }

        public void Process()
        {   // NPC委托任务  领取任务奖励
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                //现在只有一条委托任务，领取后就可以删除了
                // NpcTaskData taskData = new NpcTaskData();
                // taskData.taskId = (int)msg.NpcTask.TaskId;
                // taskData.progress = (int)msg.NpcTask.Process;
                // taskData.startTime = (ulong)msg.NpcTask.StartTime;
                // taskData.endTime = (ulong)msg.NpcTask.EndTime;
                
                TaskInfoManager.Instance.SetNpcTask(null);
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                
                //最新账户npc积分
                DataManager.Instance.mRoleData.npcTaskPoints = (int)msg.NpcTaskPoints;
                //完成随机事件信息更新
                FinishEventCount finishEventCount = new FinishEventCount();
                finishEventCount.eventType = (int)msg.FinishEventInfo.EventType;
                finishEventCount.finishCount = (int)msg.FinishEventInfo.FinishCounter;
                MapChapterManager.Instance.UpdateFinishEventCountList(finishEventCount);
                
                //事件
                RandomEventData randomEventData = new RandomEventData();
                randomEventData = MapChapterManager.Instance.GetRandomEventDataByGuid(msg.EventGuid);
                if (randomEventData != null)
                {
                    BatchStuff stuff = new BatchStuff();
                    stuff.id = (int)msg.BatchStuff.Id;//服务器维护id
                    stuff.amount = (int)msg.BatchStuff.Amount;
                    stuff.eventStatus = (int)msg.BatchStuff.EventStatus;//状态
                    stuff.cfgId = (int)msg.BatchStuff.CfgId;//EventTask表的 主id
                    for (int i = 0; i < randomEventData.batchStuffList.Count; i++)
                    {
                        if (randomEventData.batchStuffList[i].id == stuff.id)
                        {
                            randomEventData.batchStuffList[i] = stuff;
                        }
                    }
                }
                
                // 关闭领取奖励界面
                UIManager.Instance.CloseUIPanel("HuntingTaskMain");
                
                //通知刷新
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DELEGATE_TASK_UPDATE);
                
                //不做弹窗表现了
                // List<ItemData> ret = new List<ItemData>();
                // PlayerAttrUtils.GetItemData(msg.RewardItems.ItemsList.ToList(), ref ret);
                // if (ret.Count > 0)
                // {
                //     foreach (var item in ret)
                //     {
                //         ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(item.id);
                //         if(itemTypeUnit != null)
                //             TipsManger.Instance.ShowTip(UIResource.GetItemUrl(itemTypeUnit.Icon),itemTypeUnit.Name + "x"+StringUtils.FormatCurrency(item.count));
                //     }
                //     UIManager.Instance.ShowUIPanel("GetReward", ret);
                // }
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ClaimNPCTaskAward_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
