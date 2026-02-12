using System.Collections.Generic;
using System.Linq;
using Config;
using msg;

namespace Engine
{
    using EngineBase;
    
    public class ExchangeItemByNPCPoints_SCRecv : IReceiver
    {
        public ExchangeItemByNPCPoints_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ExchangeItemByNPCPoints_SC;
        }

        public void Process()
        {
            //兑换npc积分道具反馈
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                //最新账户npc积分
                DataManager.Instance.mRoleData.npcTaskPoints = (int)msg.NpcPoints;
                
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.RewardItems.ItemsList.ToList(), ref ret);
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                
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
                            UIManager.Instance.ShowUIPanel("GetReward", boxShowItems, 0, 1);
                            return;
                        }
                    }
                    UIManager.Instance.ShowUIPanel("GetReward", ret, boxShowItems, 0, 1);
                }
                
                PointExchangeInfo pointExchangeInfo = new PointExchangeInfo();
                pointExchangeInfo.itemId = (int)msg.ExchangeItemInfo.ItemId;
                pointExchangeInfo.changeCounter = (int)msg.ExchangeItemInfo.ChangeCounter;
                TaskInfoManager.Instance.UpdateNpcTaskExchangeInfo(pointExchangeInfo);

                //通知刷新
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DELEGATE_TASK_POINTS_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ExchangeItemByNPCPoints_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}