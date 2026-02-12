using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class NewSkills_PC_Recv : IReceiver
    {
        public NewSkills_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_NewSkills_PC;
        }

        public void Process()
        {
            foreach (var item in msg.SkillsList)
            {
                SkillInfo skillInfo = new SkillInfo();
                skillInfo.SkillId = (int) item.SkillId;
                skillInfo.SkillUnit = ConfigUtils.GetSkillById((int) item.SkillId);
                skillInfo.Level = (int) item.Level;
                skillInfo.CardNumber = (int) item.Amount;
                skillInfo.CarryAtkValue = item.CarryAtkValue;
                skillInfo.OwnerAtkValue = item.OwnedAtkValue;
                skillInfo.SkillAtkValue = item.SkillAtkValue;
                SkillInfoManager.Instance.UpdateSkillInfo(skillInfo);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SKILLInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewSkills_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
