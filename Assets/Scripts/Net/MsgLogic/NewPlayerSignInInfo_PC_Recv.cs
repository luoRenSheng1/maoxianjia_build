using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class NewPlayerSignInInfo_PC_Recv : IReceiver
    {
        public NewPlayerSignInInfo_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_NewPlayerSignInInfo_PC;
        }

        public void Process()
        {
            ActivityManager.Instance.HasSevenDay = true;
            foreach (var item in msg.SignInfoList)
            {
                ActivityManager.Instance.UpdateSevenDay(item.Id, item.ClaimStatus+1, item.SigninTime);
            }
          
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewPlayerSignInInfo_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
