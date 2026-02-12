using System.Collections.Generic;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class CampEnter_SCRecv : IReceiver
    {
        public CampEnter_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Camp_Enter_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                DataManager.Instance.GetRoleData().battleStatus = (int) msg.Status;
                
                var lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
                if (lobbyView != null)
                    lobbyView.OpenBottomMapPanel();
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("CampEnter_SCRecv Read " + mRecv.Length);
            
            msg = CampEnter_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
