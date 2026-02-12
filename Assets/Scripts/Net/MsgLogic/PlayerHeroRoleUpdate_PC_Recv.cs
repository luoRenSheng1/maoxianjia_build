using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerHeroRoleUpdate_PC_Recv : IReceiver
    {
        public PlayerHeroRoleUpdate_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerHeroRoleUpdate_PC;
        }

        public void Process()
        {
            foreach (var item in msg.HeroesList)
            {
                HeroInfo heroInfo = new HeroInfo();
                heroInfo.HeroUnit = ConfigUtils.GetHeroById((int) item.HeroId);
                heroInfo.Exp = item.Exp;
                heroInfo.Level = (int) item.Level;
                heroInfo.BreakLevel = (int) item.BreakLevel;
                heroInfo.BreakLevelLayer = (int) item.BreakTierLevel;
                // heroInfo.SkillDamageRate = item.ExtraSkillDamageRate;
                heroInfo.SkillDamageRate = item.SkillDamage;
                heroInfo.SkillLevel = (int) item.MainSkillLevel;//主动技能等级
                HeroInfoManager.Instance.UpdateHeroInfos(heroInfo);
                HeroInfoManager.Instance.UpdataHeroSkillLevelUp((int)item.HeroId, (int)item.MainSkillLevel);
                HeroInfoManager.Instance.UpdateHeroSkillDamageRate((int)item.SkillDamage);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_HEROInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerHeroRoleUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
