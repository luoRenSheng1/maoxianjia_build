using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerSkillInfo4Login_PC_Recv : IReceiver
    {
        public PlayerSkillInfo4Login_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerSkillInfo4Login_PC;
        }

        public void Process()
        {
            foreach (var item in msg.SkillsList)
            {
                SkillInfo skillInfo = new SkillInfo();
                skillInfo.SkillUnit = ConfigUtils.GetSkillById((int) item.SkillId);
                skillInfo.SkillId = (int) item.SkillId;
                skillInfo.Level = (int) item.Level;
                skillInfo.CardNumber = (int) item.Amount;
                skillInfo.SkillAtkValue = item.SkillAtkValue;
                skillInfo.CarryAtkValue = item.CarryAtkValue;
                skillInfo.OwnerAtkValue = item.OwnedAtkValue;
                SkillInfoManager.Instance.UpdateSkillInfo(skillInfo);
            }
            
            if(msg.SplitInfo.IsEnd)
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SKILLInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerSkillInfo4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
