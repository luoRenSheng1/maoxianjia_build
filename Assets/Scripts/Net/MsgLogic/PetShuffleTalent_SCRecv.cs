using System.Collections.Generic;
using msg;

namespace Engine
{
    using EngineBase;
    
    public class PetShuffleTalent_SCRecv : IReceiver
    {
        public PetShuffleTalent_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetShuffleTalent_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                // List<int> idList = new List<int>();
                // foreach (var talent in msg.TalentsList)
                // {
                //     int id = (int)talent.TalentId;
                //     idList.Add(id);
                // }

                List<PetTalent> petTalentList = new List<PetTalent>();
                foreach (var talent in msg.TalentsList)
                {
                    PetTalent petTalent = new PetTalent();
                    petTalent.talentId = (int)talent.TalentId;
                    petTalent.ObjType = (int)talent.ObjType;

                    List<PetBattleAttr> petAttrList = new List<PetBattleAttr>();
                    foreach (var attr in talent.BattleAttrList)
                    {
                        PetBattleAttr petAttr = new PetBattleAttr();
                        petAttr.AttrId = (int)attr.AttrId;
                        petAttr.AttrVal = attr.AttrValue;
                        petAttrList.Add(petAttr);
                    }
                    petTalent.BattleAttrs = petAttrList;
                    
                    petTalentList.Add(petTalent);
                }
                
                PetInfoManager.Instance.UpdatePetTalentIds(msg.PetId, petTalentList);
                
                PlayerAttrUtils.UpdateFinance(msg.AccountFinance);
                
                foreach (var item in msg.ItemsList)
                {
                    ItemInfoManager.Instance.ReduceItem((int) item.Id, (int) item.Num);
                }

                PetInfoManager.Instance.GetLockTalentIds().Clear();
                List<int> lockTalentIds = new List<int>();
                foreach (var item in msg.LockedTalentIdsList)
                {
                    lockTalentIds.Add((int)item);
                }
                PetInfoManager.Instance.AddLockTalentIds(lockTalentIds);
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RESET_PET_TELENT_INFO);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PetShuffleTalent_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}