using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GlobalFirstRank_SCRecv : IReceiver
    {
        public GlobalFirstRank_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalFirstRank_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                List<PvpRankVo> pvpRankVos = new List<PvpRankVo>();
                foreach (var item in msg.RankDataList)
                {
                    PvpRankVo pvpRankVo = new PvpRankVo();
                    pvpRankVo.HeroId = (int) item.HeroId;
                    pvpRankVo.Rank = (int)item.Rank;
                    pvpRankVo.PlayerId = item.UserId.ToString();
                    pvpRankVo.PlayerName = item.UserName;
                    // pvpRankVo.PlayerHeadIcon = item.HeroId.ToString();
                    pvpRankVo.Fight = item.BattlePower;
                    pvpRankVo.PlayerHeadIcon = item.AvatarId.ToString();
                    pvpRankVos.Add(pvpRankVo);
                }
                PvpRankDataManager.Instance.SetPvpRankList(pvpRankVos);

                if(msg.SplitPkgRankData.IsEnd)
                    UIManager.Instance.ShowUIPanel("RankingListMain");
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GlobalFirstRank_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
