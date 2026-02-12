using System.Collections.Generic;
using msg;

namespace Engine
{
    public class OnlineAwardClick_SCRecv : IReceiver
    {
        public OnlineAwardClick_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_OnlineAwardClick_SC;
        }

        public void Process()
        {
            OfflineRewardData offlineRewardData = new OfflineRewardData();
            List<ItemData> itemDatas = new List<ItemData>(); 
            double gold = 0;
            double boxKey = 0;
            foreach (var item in msg.RewardItemsList)
            {
                if (item.Id == ConstDefine.CONST_MAGIC_KEY)
                {
                    boxKey = item.Num;
                }
                else if (item.Id == (int) eCurrencyItemID.eCurrencyItemID_Gold)
                {
                    gold = item.Num;
                }
                else
                {
                    ItemData itemData = new ItemData()
                    {
                        id = (int)item.Id,
                        count = item.Num
                    };
                    itemDatas.Add(itemData);
                }
            }

            offlineRewardData.gold = gold;
            offlineRewardData.boxKeyNum = boxKey;
            offlineRewardData.ItemDatas = itemDatas;
            DataManager.Instance.GetRoleData().OnlineAwardCdTime = msg.OnlineAwardCount;
            if(!GuideManager.Instance.IsShowGuiding)
            {
                UIManager.Instance.ShowUIPanel("OfflineReward",1, offlineRewardData);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = OnlineAwardClick_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
