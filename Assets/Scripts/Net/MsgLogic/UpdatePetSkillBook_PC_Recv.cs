using System.Collections.Generic;
using msg;

namespace Engine
{
    using EngineBase;
    
    public class UpdatePetSkillBook_PC_Recv : IReceiver
    {
        public UpdatePetSkillBook_PC msg;
        
        public int MsgID()
        {
            return (int)eMsgID.eMsg_UpdatePetSkillBook_PC;
        }

        public void Process()
        {
            foreach (var item in msg.PetBookInfoList)
            {
                PetSkillBook petSkillBook = new PetSkillBook();
                petSkillBook.BookId = (int)item.BookId;
                petSkillBook.Amount = (int)item.Amount;
                petSkillBook.Gold = (int)item.Gold;
                petSkillBook.Guid = (long)item.Guid;
                petSkillBook.Quality = (int)item.Quality;
                
                List<PetBattleAttr> petAttrList = new List<PetBattleAttr>();
                foreach (var attr in item.BattleAttrList)
                {
                    PetBattleAttr petAttr = new PetBattleAttr();
                    petAttr.AttrId = (int)attr.AttrId;
                    petAttr.AttrVal = attr.AttrValue;
                    petAttrList.Add(petAttr);
                }
                petSkillBook.battleAttrList = petAttrList;
                
                PetInfoManager.Instance.UpdatePetSkillBookInfo(petSkillBook);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_GET_PET_SKILLBOOK);
        }
        
        public bool Read(BaseStructRecv mRecv)
        {
            msg = UpdatePetSkillBook_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}