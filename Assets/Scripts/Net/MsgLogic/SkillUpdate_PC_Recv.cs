using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class SkillUpdate_PC_Recv : IReceiver
    {
        public SkillUpdate_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_SkillUpdate_PC;
        }

        public void Process()
        {
            SkillInfo skillInfo = new SkillInfo();
            skillInfo.SkillId = (int) msg.Skill.SkillId;
            skillInfo.SkillUnit = ConfigUtils.GetSkillById((int) msg.Skill.SkillId);
            skillInfo.Level = (int) msg.Skill.Level;
            skillInfo.CardNumber = (int) msg.Skill.Amount;
            skillInfo.CarryAtkValue = msg.Skill.CarryAtkValue;
            skillInfo.OwnerAtkValue = msg.Skill.OwnedAtkValue;
            skillInfo.SkillAtkValue = msg.Skill.SkillAtkValue;
            SkillInfoManager.Instance.UpdateSkillInfo(skillInfo);
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SKILLInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = SkillUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
