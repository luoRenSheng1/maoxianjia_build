using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class EquipBox_Info_PC_Recv : IReceiver
    {
        public EquipBox_Info_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_EquipBox_Info_PC;
        }

        public void Process()
        {
            RoleData roleData = DataManager.Instance.GetRoleData();
            int preLv = roleData.equipBoxLv;
            roleData.equipBoxLv = msg.BoxInfo.BoxLevel;
            roleData.equipBoxExp = msg.BoxInfo.BoxLevelupMilestone;
            roleData.equipBoxLvUpTime = msg.BoxInfo.NextLevelupTime;
            JsonObject jsObj = new JsonObject();
            jsObj["errCode"] = MsgCode.SUCCESS;
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TREASURE_LEVELUP_RES, MsgCode.SUCCESS, jsObj.ToString());
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TREASURE_PROGRESS_UPSUCC, preLv<roleData.equipBoxLv);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = EquipBox_Info_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
