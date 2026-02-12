using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ClaimRandomFinance_SCRecv : IReceiver
    {
        public ClaimRandomFinance_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimRandomFinance_SC;
        }

        public void Process()
        {   // 领取随机事件货币奖励
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                int type = MapChapterManager.Instance.GetEventTypeByGuid(msg.EventGuid);
                eRandomEventType eventType = (eRandomEventType)type;
                if (type == -1)
                {
                    Debug.LogError($"数据错误 金币 = {msg.Finance.Golds} 钻石 = {msg.Finance.Diamonds} Type = {type}");
                    Debug.Log(msg);
                    return;
                }

                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.RewardItems.ItemsList.ToList(), ref ret);
                PlayerAttrUtils.UpdateFinance(msg.Finance);  //更新 最新账户余额
                //标记金币堆或钻石堆完成的事件
                if (eventType == eRandomEventType.eRandomEventType_RandomDiamond || eventType == eRandomEventType.eRandomEventType_RandomGold)
                {//钻石和金币的随机事件
                    MapChapterManager.Instance.MarkingRandomEventCompleted(msg.EventGuid, (int)msg.BatchStuffId);
                }
                else
                {
                    if (ret.Count > 0)
                    {
                        List<ItemData> boxShowItems = new List<ItemData>();
                        foreach (var item in msg.RewardItems.BoxRandResultList)
                        {
                            boxShowItems.Add(new ItemData()
                            {
                                id = (int) item.ItemId,
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
                }

            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ClaimRandomFinance_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
