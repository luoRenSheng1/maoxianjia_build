using System.Collections.Generic;
using System.Linq;
using Engine;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class OfflineAward_PC_Recv : IReceiver
    {
        public OfflineAward_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_OfflineAward_PC;
        }

        public void Process()
        {
            OfflineRewardData offlineRewardData = new OfflineRewardData();
            List<ItemData> itemDatas = new List<ItemData>(); 
            double gold = 0;
            double boxKey = 0;
            foreach (var item in msg.RewardItems.ItemsList)
            {
                if (item.Id == ConstDefine.CONST_MAGIC_KEY)
                {
                    boxKey = (double) item.Num;
                }
                else if (item.Id == (int) eCurrencyItemID.eCurrencyItemID_Gold)
                {
                    gold = (double) item.Num;
                }
                else
                {
                    ItemData itemData = new ItemData()
                    {
                        id = (int)item.Id,
                        count = (double)  item.Num
                    };
                    itemDatas.Add(itemData);
                }
            }

            offlineRewardData.gold = gold;
            offlineRewardData.boxKeyNum = boxKey;
            offlineRewardData.ItemDatas = itemDatas;
            DataManager.Instance.GetRoleData().OfflineTotalSecond = msg.LatestOfflineSeconds;
            DataManager.Instance.GetRoleData().OfflineRewardData = offlineRewardData;
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = OfflineAward_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}

public class OfflineRewardData
{
    public double gold;
    public double boxKeyNum;
    public List<ItemData> ItemDatas;
}
