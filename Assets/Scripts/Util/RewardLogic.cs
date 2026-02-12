using Config;
using msg;
using System.Collections.Generic;
using System.Linq;

#if !UNITY_WEBGL
#endif

namespace Engine
{

    public static class RewardLogic
    {
        /// <summary>
        /// 通用给奖励接口
        /// </summary>
        /// <param name="rewardItems">道具</param>
        /// <param name="finance">余额</param>
        public static void GiveReward(ItemInfoWithRandBox rewardItems, Finance finance, int title = 0)
        {
            //获得道具
            List<ItemData> ret = new List<ItemData>();
            PlayerAttrUtils.GetItemData(rewardItems.ItemsList.ToList(), ref ret);
            PlayerAttrUtils.UpdateFinance(finance);//更新最新账户余额货币
            if (ret.Count > 0)
            {
                List<ItemData> boxShowItems = new List<ItemData>();
                foreach (var item in rewardItems.BoxRandResultList)
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
                        UIManager.Instance.ShowUIPanel("GetReward", boxShowItems, title);
                        return;
                    }
                }
                UIManager.Instance.ShowUIPanel("GetReward", ret, boxShowItems, title);
            }
        }

        /// <summary>
        /// 通用给奖励接口2 (小框)
        /// </summary>
        /// <param name="rewardItems">道具</param>
        /// <param name="finance">余额</param>
        public static void GiveRewardMini(ItemInfoWithRandBox rewardItems, Finance finance)
        {
            //获得道具
            List<ItemData> ret = new List<ItemData>();
            PlayerAttrUtils.GetItemData(rewardItems.ItemsList.ToList(), ref ret);
            PlayerAttrUtils.UpdateFinance(finance);//更新最新账户余额货币
            if (ret.Count > 0)
            {
                List<ItemData> boxShowItems = new List<ItemData>();
                foreach (var item in rewardItems.BoxRandResultList)
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
                        UIManager.Instance.ShowUIPanel("SokobanReward", boxShowItems);
                        return;
                    }
                }
                UIManager.Instance.ShowUIPanel("SokobanReward", ret, boxShowItems);
            }
        }
    }
}