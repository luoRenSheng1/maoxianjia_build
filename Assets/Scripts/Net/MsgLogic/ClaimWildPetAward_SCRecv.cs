using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ClaimWildPetAward_SCRecv : IReceiver
    {
        public ClaimWildPetAward_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimWildPetAward_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                PlayerAttrUtils.UpdateFinance(msg.Finance);  //更新 最新账户余额
                
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.RewardItems.ItemsList.ToList(), ref ret);
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
                            var lobbyView1 = UIManager.Instance.FindByName("Lobby") as LobbyView;
                            if (lobbyView1 != null)
                                lobbyView1.OpenBottomMapPanel();
                            return;
                        }
                    }
                    UIManager.Instance.ShowUIPanel("GetReward", ret, boxShowItems);
                }
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ClaimWildPetAward_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
