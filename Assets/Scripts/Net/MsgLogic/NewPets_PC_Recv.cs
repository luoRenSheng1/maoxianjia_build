using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class NewPets_PC_Recv : IReceiver
    {
        public NewPets_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_NewPets_PC;
        }

        public void Process()
        {
            foreach (var item in msg.PetsList)
            {
                PetItemInfo pet = PetInfoManager.Instance.GetPetInfoBySever(item);
                
                PetInfoManager.Instance.AddPet(pet);
            }
            
            if(msg.SplitPkg.IsEnd)
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewPets_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
