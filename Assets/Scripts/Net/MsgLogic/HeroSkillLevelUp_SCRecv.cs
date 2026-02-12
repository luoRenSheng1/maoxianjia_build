using msg;

namespace Engine
{
    using EngineBase;
    
    public class HeroSkillLevelUp_SCRecv : IReceiver
    {
        public HeroSkillLevelUp_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroSkillLevelUp_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                foreach (var item in msg.ItemsList)
                {
                    ItemInfoManager.Instance.ReduceItem((int) item.Id, (int) item.Num);
                }

                HeroInfoManager.Instance.UpdataHeroSkillLevelUp((int)msg.HeroId, (int)msg.SkillLevel);
                
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                
                HeroInfoManager.Instance.UpdateHeroSkillDamageRate((int)msg.MainSkillDamage);
                
                HeroInfo heroInfo = HeroInfoManager.Instance.GetThisHero((int) msg.HeroId);
                heroInfo.SkillDamageRate = (int)msg.MainSkillDamage;
                
                HeroInfoManager.Instance.UpdateHeroInfos(heroInfo);
                
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_HEROInfo);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_HeroSkillLV_INFO);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = HeroSkillLevelUp_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}