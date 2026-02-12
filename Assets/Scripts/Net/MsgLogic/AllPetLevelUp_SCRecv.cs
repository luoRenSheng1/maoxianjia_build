using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class AllPetLevelUp_SCRecv : IReceiver
    {
        //public AllPetLevelUp_SC msg;

        public int MsgID()
        {
            return 0; //(int)eMsgID.eMsg_AllPetLevelUp_SC;
        }

        public void Process()
        {
            /*
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                List<PetStrengthVo> petStrengthVos = new List<PetStrengthVo>();
                foreach (var item in msg.PetInfoList)
                {
                    PetItemInfo petItemInfo = PetInfoManager.Instance.GetPet( item.PetGuid);
                    int preLevel = petItemInfo.PetLv;
                    petItemInfo.PetLv = (int) item.Level;
                    
                    
                    petItemInfo.talentsList.Clear();
                    foreach (var attr in item.TalentsList)
                    {
                        petItemInfo.talentsList.Add((int)attr);
                    }
                    petItemInfo.bookSlotsList.Clear();
                    foreach (var attr in item.BookSlotsList)
                    {
                        PetSkillBookSlot bookSlot = new PetSkillBookSlot()
                        {
                            SlotId = (int) attr.SlotId,
                            SlotStatus = attr.SlotStatus,
                            BookId = (int)attr.BookId
                        };
                        petItemInfo.bookSlotsList.Add(bookSlot);
                    }
                    
                    // petItemInfo.CardNumber = (int) item.Amount;
                    // petItemInfo.CarryAttrs.Clear();
                    // foreach (var attr in item.CarryAttrsList)
                    // {
                    //     PetBattleAttr battleAttr = new PetBattleAttr()
                    //     {
                    //         AttrId = (int) attr.AttrId,
                    //         AttrVal = attr.AttrVal
                    //     };
                    //     petItemInfo.CarryAttrs.Add(battleAttr);
                    // }
                    // petItemInfo.OwnerAttrs.Clear();
                    // foreach (var attr in item.OwnedAttrsList)
                    // {
                    //     PetBattleAttr battleAttr = new PetBattleAttr()
                    //     {
                    //         AttrId = (int) attr.AttrId,
                    //         AttrVal = attr.AttrVal
                    //     };
                    //     petItemInfo.OwnerAttrs.Add(battleAttr);
                    // }
                    
                    petStrengthVos.Add(new PetStrengthVo()
                    {
                        PetBasisUnit = petItemInfo.petCfg,
                        CurLv = petItemInfo.PetLv,
                        PreLv = preLevel
                    });
                }

                if (msg.SplitPkg.IsEnd)
                {
                    UIManager.Instance.ShowUIPanel("PetStrength", petStrengthVos);
                }
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PET_LEVELUP_SUCCESS);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            */
        }

        public bool Read(BaseStructRecv mRecv)
        {
            //msg = AllPetLevelUp_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
