using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerInfoUpdate_PC_Recv : IReceiver
    {
        public PlayerInfoUpdate_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerInfoUpdate_PC;
        }

        public void Process()
        {
            PlayerAttrUtils.UpdatePlayerAttr(msg.PlayerAttrList.ToList());
            TalentInfoManager.Instance.UpdateTalentPoints(DataManager.Instance.mRoleData.talentPoints);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TALENT_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerInfoUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
