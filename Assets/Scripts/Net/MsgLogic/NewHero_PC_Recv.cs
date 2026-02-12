using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class NewHero_PC_Recv : IReceiver
    {
        public NewHero_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_NewHero_PC;
        }

        public void Process()
        {
            foreach (var item in msg.HeroInfoList)
            {
                HeroInfo heroInfo = new HeroInfo();
                heroInfo.HeroUnit = ConfigUtils.GetHeroById( (int) item.HeroId);
                heroInfo.Exp = item.Exp;
                heroInfo.Level = (int) item.Level;
                heroInfo.BreakLevel = (int) item.BreakLevel;
                heroInfo.BreakLevelLayer = (int) item.BreakTierLevel;
                // heroInfo.SkillDamageRate = item.ExtraSkillDamageRate;
                heroInfo.SkillDamageRate = item.SkillDamage;
                HeroInfoManager.Instance.UpdateHeroInfos(heroInfo);  
                HeroInfoManager.Instance.UpdataHeroSkillLevelUp((int)item.HeroId, (int)item.MainSkillLevel);
            }

            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_HEROInfo);
          
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewHero_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
