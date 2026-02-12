using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GlobalBuyChallengeCounter_SCRecv : IReceiver
    {
        public GlobalBuyChallengeCounter_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalBuyChallengeCounter_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                PvpRankDataManager.Instance.FightLeftCount = msg.ChallengeOpportunity;
                PvpRankDataManager.Instance.FightBuyCnt = msg.TodayPurchaseCounter;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PVP_FIGHT_COUNT_UPDATE);
                UIManager.Instance.ToastByKey(10199);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GlobalBuyChallengeCounter_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
