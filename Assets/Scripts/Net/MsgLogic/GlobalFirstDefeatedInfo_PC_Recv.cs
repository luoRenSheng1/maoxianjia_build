using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GlobalFirstDefeatedInfo_PC_Recv : IReceiver
    {
        public GlobalFirstDefeatedInfo_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalFirstDefeatedInfo_PC;
        }

        public void Process()
        {
            PvpRankVo pvpRankVo = new PvpRankVo();
            pvpRankVo.Fight = msg.RankData.BattlePower;
            pvpRankVo.Rank = (int) msg.RankData.Rank;
            pvpRankVo.PlayerId = msg.RankData.UserId.ToString();
            pvpRankVo.PlayerName = msg.RankData.UserName;
            pvpRankVo.HeroId = (int) msg.RankData.HeroId;
            // pvpRankVo.PlayerHeadIcon = msg.RankData.HeroId.ToString();
            pvpRankVo.PlayerHeadIcon = msg.RankData.AvatarId.ToString();
            PvpRankDataManager.Instance.SetPvpRankVo(pvpRankVo);
            
            BattleReportVo battleReportVo = new BattleReportVo()
            {
                ReportType = msg.WarReports.WarReport,
                Param = msg.WarReports.ParamsList.ToList(),
                TimeStamp = msg.WarReports.TimeStamp
            };
                
            PvpRankDataManager.Instance.SetBattleReport(battleReportVo);
            
            PvpRankDataManager.Instance.FightLeftCount = msg.PlayerDailyData.ChallengeOpportunity;
            PvpRankDataManager.Instance.YestdayMyRank = msg.PlayerDailyData.YesterdayRank;
            PvpRankDataManager.Instance.YestdayMyRankAwardGet = msg.PlayerDailyData.HasClaimAward;
            PvpRankDataManager.Instance.YesterdayGFPlayCounter = (int) msg.PlayerDailyData.YesterdayPlayCounter;
            PvpRankDataManager.Instance.TodayGFPlayCounter = (int) msg.PlayerDailyData.TodayPlayCounter;
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PVP_UPDATE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PVP_FIGHT_COUNT_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GlobalFirstDefeatedInfo_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
