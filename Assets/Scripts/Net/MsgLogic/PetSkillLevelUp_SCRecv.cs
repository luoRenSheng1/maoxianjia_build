using msg;

namespace Engine
{
    using EngineBase;
    
    public class PetSkillLevelUp_SCRecv : IReceiver
    {
        public PetSkillLevelUp_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetSkillLevelUp_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                PetItemInfo petItemInfo = PetInfoManager.Instance.GetPet( msg.PetId);
                petItemInfo.skillLevel = msg.SkillLevel;
                PetInfoManager.Instance.SetPetBattleAttr(petItemInfo);
                PetInfoManager.Instance.AddPet(petItemInfo);
                
                //更新货币 道具
                PlayerAttrUtils.UpdateFinance(msg.AccountFinance);
                foreach (var item in msg.ItemsList)
                    ItemInfoManager.Instance.ReduceItem((int) item.Id, (int) item.Num);
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PET_LEVELUP_SUCCESS);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PetSkillLevelUp_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}