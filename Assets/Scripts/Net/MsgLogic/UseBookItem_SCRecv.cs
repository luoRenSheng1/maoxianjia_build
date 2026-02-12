using System.Collections.Generic;
using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class UseBookItem_SCRecv : IReceiver
    {
        public UseBookItem_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_UseBookItem_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                PetItemInfo pet = PetInfoManager.Instance.GetPet(msg.PetId);

                PetSkillBookSlot bookSlotInfo = new PetSkillBookSlot();
                bookSlotInfo.SlotId = (int)msg.BookSlots.SlotId;
                bookSlotInfo.SlotStatus = msg.BookSlots.SlotStatus;
                bookSlotInfo.BookId = (int)msg.BookSlots.BookId;
                
                List<PetBattleAttr> petAttrList = new List<PetBattleAttr>();
                foreach (var attr in msg.BookSlots.BattleAttrList)
                {
                    PetBattleAttr petAttr = new PetBattleAttr();
                    petAttr.AttrId = (int)attr.AttrId;
                    petAttr.AttrVal = attr.AttrValue;
                        
                    petAttrList.Add(petAttr);
                }
                bookSlotInfo.battleAttrList = petAttrList;
                
                for (int i = 0; i < pet.bookSlotsList.Count; i++)
                {
                    if (pet.bookSlotsList[i].SlotId == msg.BookSlots.SlotId)
                    {
                        pet.bookSlotsList[i] = bookSlotInfo;
                    }
                }
                
                // 技能书修改，重新设置宠物属性
                PetInfoManager.Instance.SetPetBattleAttr(pet);
                
                PetInfoManager.Instance.AddPet(pet);
                
                ItemInfoManager.Instance.ReduceItem((int)msg.CostItemId, msg.CostItemNum);
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PETBOOK_UPDATE);
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = UseBookItem_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}