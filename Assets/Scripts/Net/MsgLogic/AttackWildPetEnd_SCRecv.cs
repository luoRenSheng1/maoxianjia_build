using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class AttackWildPetEnd_SCRecv : IReceiver
    {
        public AttackWildPetEnd_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AttackWildPetEnd_SC;
        }

        public void Process()
        {
            // 逃跑的宠物 战斗结束
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                if (msg.IsWin)
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

                    UIManager.Instance.ShowUIPanel("GetSingleReward", msg.EventGuid, msg.BatchStuffId, msg.SelectItemsList.ToList());
                }
                else
                {
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_RESULT_UPDATE, (bool)msg.IsWin, (int)msg.BatchStuffId, (ulong)msg.EventGuid);
                }
            }
            else
            {
                //UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = AttackWildPetEnd_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
