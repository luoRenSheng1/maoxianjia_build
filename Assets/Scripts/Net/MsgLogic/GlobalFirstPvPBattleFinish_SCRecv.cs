using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GlobalFirstPvPBattleFinish_SCRecv : IReceiver
    {
        public GlobalFirstPvPBattleFinish_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalFirstPvPBattleFinish_SC;
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
            
                BattleReportVo battleReportVo = new BattleReportVo()
                {
                    ReportType = msg.WarReports.WarReport,
                    Param = msg.WarReports.ParamsList.ToList(),
                    TimeStamp = msg.WarReports.TimeStamp
                };
                
                PvpRankDataManager.Instance.SetBattleReport(battleReportVo);
                
                bool fightLose = msg.IsWin;
                if (!fightLose)
                {
                    UIManager.Instance.ShowUIPanel("PVPLose", DataManager.Instance.GetRoleData().userName, DataManager.Instance.GetRoleData().GetAvatarUrl(),
                        PvpRankDataManager.Instance.OtherPvpRankVo.PlayerName, pvpRankVo.PlayerHeadIcon);
                }
                else
                {
                    UIManager.Instance.ShowUIPanel("PVPWin", DataManager.Instance.GetRoleData().userName,
                        DataManager.Instance.GetRoleData().GetAvatarUrl(),
                        PvpRankDataManager.Instance.OtherPvpRankVo.PlayerName, PvpRankDataManager.Instance.OtherPvpRankVo.PlayerHeadIcon);
                }
                
                PvpRankDataManager.Instance.FightLeftCount = msg.PlayerDailyData.ChallengeOpportunity;
                PvpRankDataManager.Instance.YestdayMyRank = msg.PlayerDailyData.YesterdayRank;
                PvpRankDataManager.Instance.YestdayMyRankAwardGet = msg.PlayerDailyData.HasClaimAward;
                PvpRankDataManager.Instance.YesterdayGFPlayCounter = (int) msg.PlayerDailyData.YesterdayPlayCounter;
                PvpRankDataManager.Instance.TodayGFPlayCounter = (int) msg.PlayerDailyData.TodayPlayCounter;
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
            msg = GlobalFirstPvPBattleFinish_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
