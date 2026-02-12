using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class HeroAddExp_SCRecv : IReceiver
    {
        public HeroAddExp_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroAddExp_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                foreach (var item in msg.CostItemsList)
                {
                    ItemInfoManager.Instance.ReduceItem((int) item.Id, (int) item.Num);
                }

                HeroInfo heroInfo = HeroInfoManager.Instance.GetThisHero((int) msg.HeroInfo.HeroId);
                int preLevel = heroInfo.Level;
                heroInfo.HeroUnit = ConfigUtils.GetHeroById( (int) msg.HeroInfo.HeroId);
                heroInfo.Exp = msg.HeroInfo.Exp;
                heroInfo.Level = (int) msg.HeroInfo.Level;
                heroInfo.BreakLevel = (int) msg.HeroInfo.BreakLevel;
                heroInfo.BreakLevelLayer = (int) msg.HeroInfo.BreakTierLevel;
                // heroInfo.SkillDamageRate = msg.HeroInfo.ExtraSkillDamageRate;
                heroInfo.SkillDamageRate = msg.HeroInfo.SkillDamage;
                HeroInfoManager.Instance.UpdateHeroInfos(heroInfo);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_HEROInfo_LevelUp, heroInfo, preLevel);

                if (heroInfo.Level > preLevel)
                {
                    // GameManager.Instance.SoundManager.PlayEffectWithoutLoop(15);
                }

                // 英雄每升30级，提示获得天赋点
                if (heroInfo.Level > preLevel && msg.HeroInfo.Level % 30 == 0)
                {
                    UIManager.Instance.ToastByKey(8040);
                }
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }


            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = HeroAddExp_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
