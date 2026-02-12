using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class SkillInSlot_SCRecv : IReceiver
    {
        public SkillInSlot_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_SkillInSlot_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                if (msg.SkillId > 0)
                {
                    SkillInfo skillInfo =  SkillInfoManager.Instance.GetSkill((int)msg.SkillId);
                    skillInfo.BattleIndex = -1;
                }

                SkillSlotInfo slotInfo = new SkillSlotInfo();
                slotInfo.SlotId = (int)msg.SkillSlot.SlotId;
                slotInfo.SkillId = (int)msg.SkillSlot.SkillId;
                slotInfo.Status = msg.SkillSlot.SlotStatus;
                RoleManager.Instance.AddSkillSlotInfo(slotInfo);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SKILL_SC_SUCC);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_SKILL_LIST);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = SkillInSlot_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
