using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PetLottery_SCRecv : IReceiver
    {
        public PetLottery_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetLottery_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.ItemsList.ToList(), ref ret);
                
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                DataManager.Instance.mRoleData.petLotteryLv = msg.Lottery.LotteryLevel;
                DataManager.Instance.mRoleData.petLotteryExp = (int)msg.Lottery.LotteryExp;
                if(ret.Count > 0)
                    UIManager.Instance.ShowUIPanel("SummonGetReward", ret, 1);
                AdManager.Instance.SetAdFreeTime((int) ePlayerAttrID.ePlayerAttrID_PetLottoTimesByAd, msg.FreeAdTimes);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PET_LOTTERY_SUCCESS);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PetLottery_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
