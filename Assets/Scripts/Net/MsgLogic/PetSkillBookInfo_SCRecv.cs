using System.Collections.Generic;
using msg;

namespace Engine
{
    using EngineBase;
    
    public class PetSkillBookInfo_SCRecv : IReceiver
    {
        public PetSkillBookInfo_SC msg;
        
        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetSkillBookInfo_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                foreach (var item in msg.PetBookInfoList)
                {
                    PetSkillBook petSkillBook = new PetSkillBook();
                    petSkillBook.BookId = (int)item.BookId;
                    petSkillBook.Amount = (int)item.Amount;
                    petSkillBook.Gold = (int)item.Gold;
                    petSkillBook.Guid = (long)item.Guid;
                    petSkillBook.Quality = (int)item.Quality;
                    PetInfoManager.Instance.PetSkillBooksInPackage(petSkillBook);
                    
                    List<PetBattleAttr> petAttrList = new List<PetBattleAttr>();
                    foreach (var attr in item.BattleAttrList)
                    {
                        PetBattleAttr petAttr = new PetBattleAttr();
                        petAttr.AttrId = (int)attr.AttrId;
                        petAttr.AttrVal = attr.AttrValue;
                        petAttrList.Add(petAttr);
                    }
                    petSkillBook.battleAttrList = petAttrList;
                    
                }
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_GET_PET_SKILLBOOK);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }
        
        public bool Read(BaseStructRecv mRecv)
        {
            msg = PetSkillBookInfo_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}