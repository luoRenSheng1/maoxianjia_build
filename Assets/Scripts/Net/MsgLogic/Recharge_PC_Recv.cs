using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class Recharge_PC_Recv : IReceiver
    {
        public Recharge_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Recharge_PC;
        }

        public void Process()
        {
            foreach (var payListId in msg.RechargedIdList)
            {
                RoleManager.Instance.SetRechargeFlag(payListId, true);
            }
            PlayerAttrUtils.UpdateFinance(msg.Finance);

            if (msg.RechargDiamond > 0 && msg.RechargeStatus == eRechargeStatus.eRechargeStatus_Success)
                //TipsManger.Instance.ShowTip(UIResource.DiamondIcon,"+" + (msg.RechargDiamond+msg.FirstRechargeDiamond));
            {
                List<ItemData> ret = new List<ItemData>();
                ret.Add(new ItemData()
                {
                    id = ConstDefine.Item_Diamond,
                    count = msg.RechargDiamond+msg.FirstRechargeDiamond
                });
                if (ret.Count > 0)
                {
                    // foreach (var item in ret)
                    // {
                    //     ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(item.id);
                    //     if(itemTypeUnit != null)
                    //         TipsManger.Instance.ShowTip(UIResource.GetItemUrl(itemTypeUnit.Icon),itemTypeUnit.Name + "x"+StringUtils.FormatCurrency(item.count));
                    // }
                    UIManager.Instance.ShowUIPanel("GetReward", ret);
                }
            }

            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RECHARGE_SUCCESS);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = Recharge_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
