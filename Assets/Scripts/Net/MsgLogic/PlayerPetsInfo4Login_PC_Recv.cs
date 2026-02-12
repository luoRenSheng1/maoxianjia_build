using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    public class PlayerPetsInfo4Login_PC_Recv : IReceiver
    {
        public PlayerPetsInfo4Login_PC msg;
        public int MsgID()
        {
            return (int) eMsgID.eMsg_PlayerPetsInfo4Login_PC;
        }

        public void Process()
        {
            foreach (var item in msg.PetsList)
            {
                PetItemInfo pet = PetInfoManager.Instance.GetPetInfoBySever(item);
                PetInfoManager.Instance.AddPet(pet);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            // LogUtils.LogWarning("PlayerPetsInfo4Login_PC_Recv Read " + mRecv.Length);
            msg = PlayerPetsInfo4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}