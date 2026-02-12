using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GlobalFirstTakeOverRank_SCRecv : IReceiver
    {
        public GlobalFirstTakeOverRank_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalFirstTakeOverRank_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
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
                
                PvpRankDataManager.Instance.FightLeftCount = msg.PlayerDailyData.ChallengeOpportunity;
                PvpRankDataManager.Instance.YestdayMyRank = msg.PlayerDailyData.YesterdayRank;
                PvpRankDataManager.Instance.YestdayMyRankAwardGet = msg.PlayerDailyData.HasClaimAward;
                PvpRankDataManager.Instance.YesterdayGFPlayCounter = (int) msg.PlayerDailyData.YesterdayPlayCounter;
                PvpRankDataManager.Instance.TodayGFPlayCounter = (int) msg.PlayerDailyData.TodayPlayCounter;

                BattleReportVo battleReportVo = new BattleReportVo()
                {
                    ReportType = msg.WarReports.WarReport,
                    Param = msg.WarReports.ParamsList.ToList(),
                    TimeStamp = msg.WarReports.TimeStamp
                };
                
                PvpRankDataManager.Instance.SetBattleReport(battleReportVo);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PVP_FIGHT_COUNT_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PVP_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GlobalFirstTakeOverRank_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
