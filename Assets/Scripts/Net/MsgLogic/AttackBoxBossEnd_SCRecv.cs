using System.Collections.Generic;
using System.Linq;
using Config;
using msg;

namespace Engine
{
    using EngineBase;
    using UnityEngine;

    //using System.Diagnostics;

    /// <summary>
    /// 深埋宝藏 战斗结束
    /// </summary>
    public class AttackBoxBossEnd_SCRecv : IReceiver
    {
        public AttackBoxBossEnd_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AttackBoxBossEnd_SC;
        }

        public void Process()
        {
            Debug.Log("AttackBoxBossEnd_SC msg.Result = " + msg.Result);
            // 野外boss 战斗结束
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.RewardItems.ItemsList.ToList(), ref ret);
                if (ret.Count > 0)
                {
                    List<ItemData> boxShowItems = new List<ItemData>();
                    foreach (var item in msg.RewardItems.BoxRandResultList)
                    {
                        boxShowItems.Add(new ItemData()
                        {
                            id = (int)item.ItemId,
                            count = item.ItemNum,
                            ItemGuid = item.ItemGuid
                        });
                    }

                    if (ret.Count == 1 && ret[0].count == 1)
                    {
                        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(ret[0].id);
                        if (itemTypeUnit.Type == 8) //宝箱
                        {
                            UIManager.Instance.ShowUIPanel("GetReward", boxShowItems);
                            return;
                        }
                    }
                    UIManager.Instance.ShowUIPanel("GetReward", ret, boxShowItems);
                }

                if (msg.IsWin)
                {
                    PlayerAttrUtils.UpdateFinance(msg.Finance);  //更新 最新账户余额

                    RandomEventData eventData = MapChapterManager.Instance.GetRandomEventDataByGuid(msg.EventGuid);
                    if (eventData != null)
                    {
                        foreach (var stuffData in eventData.batchStuffList)
                        {
                            if (stuffData.id == msg.StuffId)
                            {
                                stuffData.eventStatus = (int)eRandomEventStatus.eRandomEventStatus_Finished;
                            }
                        }
                    }
                }
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_RESULT_UPDATE, (bool)msg.IsWin, (int)msg.StuffId, (ulong)msg.EventGuid);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000 + (int)msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = AttackBoxBossEnd_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
