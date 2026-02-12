using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class AwardCode_SCRecv : IReceiver
    {
        public AwardCode_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AwardCode_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.AwardItems.ItemsList.ToList(), ref ret);
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                
                if (ret.Count > 0)
                {
                    List<ItemData> boxShowItems = new List<ItemData>();
                    foreach (var item in msg.AwardItems.BoxRandResultList)
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
                    UIManager.Instance.ShowUIPanel("GetReward", ret,boxShowItems);
                }
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = AwardCode_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
