using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    /// <summary>
    /// 引导免费十次抽技能
    /// </summary>
    public class FreeSkillLottery4Guide_SCRecv : IReceiver
    {
        public FreeSkillLottery4Guide_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_FreeSkillLottery4Guide_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.ItemsList.ToList(), ref ret);
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                
                DataManager.Instance.mRoleData.SkillLotteryLv = msg.Lottery.LotteryLevel;
                DataManager.Instance.mRoleData.SkillLotteryExp = (int)msg.Lottery.LotteryExp;
                
                if (ret.Count > 0)
                    UIManager.Instance.ShowUIPanel("SummonGetReward", ret, 2);

                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_SKILL_LOTTERY_SUCCESS);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_Skill_LIST_Lobby);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = FreeSkillLottery4Guide_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
