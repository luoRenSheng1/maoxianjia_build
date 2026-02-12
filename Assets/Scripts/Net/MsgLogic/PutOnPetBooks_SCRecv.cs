using System.Collections.Generic;
using msg;

namespace Engine
{
    
    using EngineBase;
    
    public class PutOnPetBooks_SCRecv : IReceiver
    {
        public PutOnPetBooks_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PutOnPetBooks_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                PetItemInfo pet = PetInfoManager.Instance.GetPet(msg.PetId);
                List<PetSkillBookSlot> list = new List<PetSkillBookSlot>();

                foreach (var item in msg.BookSlotsList)
                {
                    PetSkillBookSlot bookSlot = new PetSkillBookSlot();
                    bookSlot.SlotId = (int)item.SlotId;
                    bookSlot.SlotStatus = item.SlotStatus;
                    bookSlot.BookId = (int)item.BookId;
                    
                    List<PetBattleAttr> attrList = new List<PetBattleAttr>();
                    foreach (var attr in item.BattleAttrList)
                    {
                        PetBattleAttr battleAttr = new PetBattleAttr();
                        battleAttr.AttrId = (int)attr.AttrId;
                        battleAttr.AttrVal = attr.AttrValue;
                        attrList.Add(battleAttr);
                    }
                    bookSlot.battleAttrList = attrList;
                    
                    list.Add(bookSlot);
                }
                
                pet.bookSlotsList = list;
                
                // 技能书修改，重新设置宠物属性
                PetInfoManager.Instance.SetPetBattleAttr(pet);
                
                PetInfoManager.Instance.AddPet(pet);

                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PETBOOK_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PutOnPetBooks_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}