using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GlobalFirstRank_PC_Recv : IReceiver
    {
        public GlobalFirstRank_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalFirstRank_PC;
        }

        public void Process()
        {
            List<PvpRankVo> pvpRankVos = new List<PvpRankVo>();
            foreach (var item in msg.RankDataList)
            {
                PvpRankVo pvpRankVo = new PvpRankVo();
                pvpRankVo.Rank = (int)item.Rank;
                pvpRankVo.PlayerId = item.UserId.ToString();
                pvpRankVo.PlayerName = item.UserName;
                pvpRankVo.HeroId = (int) item.HeroId;
                // pvpRankVo.PlayerHeadIcon = item.HeroId.ToString();
                pvpRankVo.PlayerHeadIcon = item.AvatarId.ToString();
                pvpRankVo.Fight = item.BattlePower;
                pvpRankVos.Add(pvpRankVo);
            }
            PvpRankDataManager.Instance.SetPvpRankList(pvpRankVos);
            
            if(msg.SplitPkgRankData.IsEnd)
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PVP_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GlobalFirstRank_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
