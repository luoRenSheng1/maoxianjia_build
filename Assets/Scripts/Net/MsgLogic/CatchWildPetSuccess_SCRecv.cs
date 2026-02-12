using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class CatchWildPetSuccess_SCRecv : IReceiver
    {
        public CatchWildPetSuccess_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_CatchWildPetSuccess_SC;
        }

        public void Process()
        {
            // 逃跑的宠物 捕捉成功
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                RandomEventData eventData = MapChapterManager.Instance.GetRandomEventDataByGuid(msg.EventGuid);
                foreach (var stuffData in eventData.batchStuffList)
                {
                    if (stuffData.id == msg.BatchStuffId)
                    {
                        stuffData.eventStatus = (int)msg.PetStatus;
                        break;
                    }
                }
                MapChapterManager.Instance.UpdateRandomEventList(eventData);

                Debug.Log($"抓宠物的三选一奖励数量 = {msg.SelectItemsList.Count}");

                UIManager.Instance.ShowUIPanel("GetSingleReward", msg.EventGuid, msg.BatchStuffId, msg.SelectItemsList.ToList());
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = CatchWildPetSuccess_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
