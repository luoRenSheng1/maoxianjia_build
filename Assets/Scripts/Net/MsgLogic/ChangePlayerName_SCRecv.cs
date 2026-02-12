using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ChangePlayerName_SCRecv : IReceiver
    {
        public ChangePlayerName_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ChangePlayerName_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                RoleData roleData = DataManager.Instance.GetRoleData();
                roleData.userName = msg.ChangeName;
                roleData.ChangeNameCounter = msg.ChangeCounter;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CHANGENAMECOUNTER);
                
                DataManager.Instance.mRoleData.gold = msg.Finance.Golds;
                DataManager.Instance.mRoleData.dia = msg.Finance.Diamonds;
                DataManager.Instance.mRoleData.dia2 = msg.Finance.FreeDiamonds;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ChangePlayerName_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
