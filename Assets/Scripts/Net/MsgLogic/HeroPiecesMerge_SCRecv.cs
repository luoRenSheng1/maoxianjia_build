using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class HeroPiecesMerge_SCRecv : IReceiver
    {
        public HeroPiecesMerge_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroPiecesMerge_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                foreach (var item in msg.CostItemsList)
                {
                    ItemInfoManager.Instance.ReduceItem((int) item.Id, (int) item.Num);
                }
                HeroInfo heroInfo = new HeroInfo();
                heroInfo.HeroUnit = ConfigUtils.GetHeroById( (int) msg.HeroInfo.HeroId);
                heroInfo.Exp = msg.HeroInfo.Exp;
                heroInfo.Level = (int) msg.HeroInfo.Level;
                heroInfo.BreakLevel = (int) msg.HeroInfo.BreakLevel;
                heroInfo.BreakLevelLayer = (int) msg.HeroInfo.BreakTierLevel;
                // heroInfo.SkillDamageRate = msg.HeroInfo.ExtraSkillDamageRate;
                heroInfo.SkillDamageRate = msg.HeroInfo.SkillDamage;
                HeroInfoManager.Instance.UpdateHeroInfos(heroInfo);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_HEROInfo_Merge, heroInfo);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_REDPOINT_UPDATE);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = HeroPiecesMerge_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
