using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PetLevelUp_SCRecv : IReceiver
    {
        public PetLevelUp_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetLevelUp_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                PetItemInfo pet = PetInfoManager.Instance.GetPet(msg.PetId);
                if (pet == null) return;
                pet.PetLv = msg.PetLevel;
                // PetItemInfo pet = PlayerAttrUtils.GetPetInfoBySever(msg.PetInfo);
                PetInfoManager.Instance.SetPetBattleAttr(pet);
                PetInfoManager.Instance.AddPet(pet);
                
                //更新货币 道具
                PlayerAttrUtils.UpdateFinance(msg.AccountFinance);
                foreach (var item in msg.ItemsList)
                    ItemInfoManager.Instance.ReduceItem((int) item.Id, (int) item.Num);

                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PET_LEVELUP_SUCCESS);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
                
                // foreach (var item in msg.UsedItemsList)
                // {
                //     ItemInfoManager.Instance.ReduceItem((int) item.Id, (int) item.Num);
                // }
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
                // PetItemInfo petItemInfo = PetInfoManager.Instance.GetPet((int)msg.PetId, 0);
                // int startLv = petItemInfo.PetLv;
                // petItemInfo.PetLv = msg.Level;
                // petItemInfo.CurExp = (int) msg.Exp;
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PET_LEVELUP_SUCCESS,(int) msg.PetId, startLv);
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PetLevelUp_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
