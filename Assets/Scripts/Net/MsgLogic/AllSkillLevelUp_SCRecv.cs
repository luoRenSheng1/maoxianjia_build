using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class AllSkillLevelUp_SCRecv : IReceiver
    {
        public AllSkillLevelUp_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AllSkillLevelUp_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                List<SkillStrengthVo> strengthVos = new List<SkillStrengthVo>();
                foreach (var item in msg.SkillInfoList)
                {
                    SkillInfo skillInfo = SkillInfoManager.Instance.GetSkill((int) item.SkillId);
                    int preLevel = skillInfo.Level;
                    skillInfo.Level = (int) item.Level;
                    skillInfo.CardNumber = (int) item.Amount;
                    skillInfo.CarryAtkValue = item.CarryAtkValue;
                    skillInfo.OwnerAtkValue = item.OwnedAtkValue;
                    skillInfo.SkillAtkValue = item.SkillAtkValue;
                    
                    strengthVos.Add(new SkillStrengthVo()
                    {
                        SkillUnit = skillInfo.SkillUnit,
                        CurLv = skillInfo.Level,
                        PreLv = preLevel
                    });
                }

                if (msg.SplitPkg.IsEnd)
                {
                    UIManager.Instance.ShowUIPanel("SkillStrength", strengthVos);
                }
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_SKILL_LEVELUP_SUCCESS);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = AllSkillLevelUp_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
