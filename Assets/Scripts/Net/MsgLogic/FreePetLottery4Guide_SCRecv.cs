using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;

    /// <summary>
    /// 引导免费十次抽宠物    
    /// </summary>
    public class FreePetLottery4Guide_SCRecv : IReceiver
    {
        public FreePetLottery4Guide_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_FreePetLottery4Guide_SC;
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
                
                //显示奖励
                if(ret.Count > 0)
                    UIManager.Instance.ShowUIPanel("SummonGetReward", ret, 1);

                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PET_LOTTERY_SUCCESS);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000 + (int)msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = FreePetLottery4Guide_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
