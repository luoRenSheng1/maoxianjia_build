using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class HeroLottery_SCRecv : IReceiver
    {
        public HeroLottery_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroLottery_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.ItemsList.ToList(), ref ret);
                
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                if(ret.Count > 0)
                    UIManager.Instance.ShowUIPanel("SummonHeroReward", ret);
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_HERO_LOTTERY_SUCCESS);
                ItemInfoManager.Instance.ReduceItem((int) msg.CostItems.Id, (int) msg.CostItems.Num);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_HERO_LOTTERY_SUCCESS);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = HeroLottery_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
