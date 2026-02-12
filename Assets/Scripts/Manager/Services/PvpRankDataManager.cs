using System.Collections.Generic;
using EngineBase;
using msg;

namespace Engine
{
    public class PvpRankVo
    {
        public int Rank;
        public int HeroId;
        public string PlayerId;
        public string PlayerName;
        public string PlayerHeadIcon;
        public double Fight;
    }

    public class BattleReportVo
    {
        /**
         * enum GFWarReportType
            {
	            GFWarReportType_InRank_BeChallenged_Win = 1;  //在排行榜被挑战胜利
	            GFWarReportType_InRank_BeChallenged_Failed = 2;  //在排行榜被挑战失败
	            GFWarReportType_NotInRank_Challenged_Win = 3;  //未在排行榜挑战胜利
	            GFWarReportType_NotInRank_Challenged_Failed = 4;  //未在排行榜挑战失败
	            GFWarReportType_NotInRank_TakenRank = 5;  //未在排行榜,占领成功
	            GFWarReportType_Win_ButNotFirst = 6;  //挑战获胜，但是不是第一个获胜
            }
         */
        public GFWarReportType ReportType;
        public List<string> Param;
        public ulong TimeStamp;
    }

    public class PvpRankDataManager : TSingleton<PvpRankDataManager>
    {
        public FightAttrVo OtherFightAttrVo = new FightAttrVo();
        public GlobalFirstPlayerRankData OpponentData;
        public double OtherSkillDamageRate = 0;
        public PvpRankVo OtherPvpRankVo;
        private List<RuneInfo> _otherBattleRuneInfoList = new List<RuneInfo>();
        private List<SkillInfo> _otherBattleSkillInfoList = new List<SkillInfo>();
        private List<PetItemInfo> _otherBattlePetInfoList = new List<PetItemInfo>();
        
        private List<BattleReportVo> _battleReportVos = new List<BattleReportVo>();
        
        public int YestdayMyRank;//昨日排名（用来领取奖励）
        public bool YestdayMyRankAwardGet;//是否领取奖励
        public int FightLeftCount;
        public int FightBuyCnt;
        public int YesterdayGFPlayCounter;
        public int TodayGFPlayCounter;

        private List<PvpRankVo> _rankList = new List<PvpRankVo>();

        public int GetOtherSkillTimes(int skillId)
        {
            int skillTimes = 0;
            foreach (var item in _otherBattleRuneInfoList)
            {
                if (item.SkillId == skillId)
                    skillTimes += item.MagicTimes;
            }

            return skillTimes;
        }

        public void SetOtherBattleRuneInfoList(List<RuneInfo> battleRuneInfoList)
        {
            _otherBattleRuneInfoList = battleRuneInfoList;
        }

        public List<SkillInfo> GetOtherBattleSkillInfoList()
        {
            return _otherBattleSkillInfoList;
        }
        
        public void SetOtherBattleSkillInfoList(List<SkillInfo> battleSkillInfoList)
        {
            _otherBattleSkillInfoList = battleSkillInfoList;
        }
        
        public List<PetItemInfo> GetOtherBattlePetList()
        {
            return _otherBattlePetInfoList;
        }
        
        public void SetOtherBattlePetList(List<PetItemInfo> petItemInfos)
        {
            _otherBattlePetInfoList = petItemInfos;
        }
        
        public void SetPvpRankList(List<PvpRankVo> rankVos)
        {
            _rankList = rankVos;
        }
        
        public void SetPvpRankVo(PvpRankVo rankVo)
        {
            bool hasRank = false;
            foreach (var item in _rankList)
            {
                if (item.Rank == rankVo.Rank)
                {
                    item.Fight = rankVo.Fight;
                    item.HeroId = rankVo.HeroId;
                    item.PlayerId = rankVo.PlayerId;
                    item.PlayerName = rankVo.PlayerName;
                    item.PlayerHeadIcon = rankVo.PlayerHeadIcon;
                    hasRank = true;
                    break;
                }
            }
            
            if(!hasRank)
                _rankList.Add(rankVo);
        }

        public PvpRankVo ThisRankHasPlayer(int rank)
        {
            foreach (var item in _rankList)
            {
                if (item.Rank == rank && !string.IsNullOrEmpty(item.PlayerId))
                {
                    return item;
                }
            }

            return null;
        }
        
        public PvpRankVo IsMyInRank()
        {
            foreach (var item in _rankList)
            {
                if ( !string.IsNullOrEmpty(item.PlayerId) && item.PlayerId == DataManager.Instance.GetRoleData().userID)
                {
                    return item;
                }
            }

            return null;
        }
        
        public void SetBattleReport(List<BattleReportVo> battleReportList)
        {
            _battleReportVos = battleReportList;
        }
        
        public void SetBattleReport(BattleReportVo battleReport)
        {
            _battleReportVos.Add(battleReport);
        }

        public List<BattleReportVo> GetBattleReportList()
        {
            return _battleReportVos;
        }

    }
}