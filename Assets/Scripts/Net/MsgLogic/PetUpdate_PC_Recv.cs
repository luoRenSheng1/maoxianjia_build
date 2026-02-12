using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PetUpdate_PC_Recv : IReceiver
    {
        public PetUpdate_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetUpdate_PC;
        }

        public void Process()
        {
            PetItemInfo pet = PetInfoManager.Instance.GetPetInfoBySever(msg.Pet);
            PetInfoManager.Instance.AddPet(pet);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PetUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
