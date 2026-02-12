using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class HeroBreak_SCRecv : IReceiver
    {
        public HeroBreak_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroBreak_SC;
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
                heroInfo.HeroUnit = ConfigUtils.GetHeroById( (int) msg.HeroInfo.HeroId);
                heroInfo.Exp = msg.HeroInfo.Exp;
                heroInfo.Level = (int) msg.HeroInfo.Level;
                heroInfo.BreakLevel = (int) msg.HeroInfo.BreakLevel;
                heroInfo.BreakLevelLayer = (int) msg.HeroInfo.BreakTierLevel;
                HeroInfoManager.Instance.UpdateHeroInfos(heroInfo);

                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_HEROInfo_Break);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = HeroBreak_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
