using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GlobalFirstPlayerInfo_PC_Recv : IReceiver
    {
        public GlobalFirstPlayerInfo_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalFirstPlayerInfo_PC;
        }

        public void Process()
        {
            PvpRankDataManager.Instance.FightLeftCount = msg.PlayerDailyData.ChallengeOpportunity;
            PvpRankDataManager.Instance.YestdayMyRank = msg.PlayerDailyData.YesterdayRank;
            PvpRankDataManager.Instance.YestdayMyRankAwardGet = msg.PlayerDailyData.HasClaimAward;
            PvpRankDataManager.Instance.YesterdayGFPlayCounter = (int) msg.PlayerDailyData.YesterdayPlayCounter;
            PvpRankDataManager.Instance.TodayGFPlayCounter = (int) msg.PlayerDailyData.TodayPlayCounter;
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PVP_FIGHT_COUNT_UPDATE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PVP_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GlobalFirstPlayerInfo_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
