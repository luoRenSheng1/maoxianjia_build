using System.Collections.Generic;
using System.Linq;
using EngineBase;
using msg;
using UnityEngine;

namespace Engine
{
    public class PlayerBasicInfo4Login_PC_Recv : IReceiver
    {
        public PlayerBasicInfo4Login_PC msg;
        public int MsgID()
        {
            return (int) eMsgID.eMsg_PlayerBasicInfo4Login_PC;
        }

        public void Process()
        {
            LogUtils.LogWarningFormat("PlayerBasicInfo4Login_PC_Recv Process {0} {1}", msg.UserId, msg.Name);
            RoleData roleData = DataManager.Instance.GetRoleData();
            roleData.userName = msg.Name;
            roleData.userID = msg.UserId.ToString();
            PlayerAttrUtils.UpdatePlayerAttr(msg.PlayerAttrList.ToList());
            TalentInfoManager.Instance.UpdateTalentPoints(DataManager.Instance.mRoleData.talentPoints);
            foreach (var item in msg.EquipSlotsList) //装备栏
            {
                // item.SlotType;
                // item.EquipGuid;
                // item
                EquipSlotInfo slotInfo = new EquipSlotInfo();
                slotInfo.SlotType = (int)item.SlotType;
                slotInfo.equipGuid = item.EquipId;
                slotInfo.Status = item.SlotStatus;
                slotInfo.EquipCfgId = (int) item.BaseItemId;
                RoleManager.Instance.AddEquipSlotInfo(slotInfo);
            }

            foreach (var item in msg.LoreEquipSlotsList) //传承装备栏
            {
                EquipSlotInfo slotInfo = new EquipSlotInfo();
                slotInfo.SlotType = (int)item.SlotType;
                slotInfo.equipGuid = item.EquipId;
                slotInfo.Status = item.SlotStatus;
                slotInfo.EquipCfgId = (int) item.BaseItemId;
                RoleManager.Instance.AddEquipSlotInfo(slotInfo);
            }
            
            foreach (var item in msg.RuneSlotsList)  //符石栏
            {
                RuneSlotInfo runeInfo = new RuneSlotInfo();
                runeInfo.RuneGuid = item.RuneGuid;
                runeInfo.ItemId = (int) item.ItemId;
                runeInfo.SlotId = (int) item.SlotId;
                RoleManager.Instance.AddRuneSlotInfo(runeInfo);
            }

            foreach (var item in msg.PetSlotsList)
            {
                PetSlotInfo slotInfo = new PetSlotInfo();
                slotInfo.SlotId = (int)item.SlotId;
                slotInfo.PetId = (int)item.PetId;
                slotInfo.Status = item.SlotStatus;
                slotInfo.PetGuid = item.PetGuid;
                RoleManager.Instance.AddPetSlotInfo(slotInfo);
            }
            
            foreach (var item in msg.SkillSlotsList)
            {
                SkillSlotInfo slotInfo = new SkillSlotInfo();
                slotInfo.SlotId = (int)item.SlotId;
                slotInfo.SkillId = (int)item.SkillId;
                slotInfo.Status = item.SlotStatus;
                RoleManager.Instance.AddSkillSlotInfo(slotInfo);
            }

            foreach (var item in msg.HolySlotsList)
            {
                HolySlotInfo holySlotInfo = new HolySlotInfo();
                holySlotInfo.SlotId = (int)item.SlotId;
                holySlotInfo.ItemId = (int)item.ItemId;
                holySlotInfo.Status = item.SlotStatus;
                RoleManager.Instance.AddHolySlotInfo(holySlotInfo);
            }

            DataManager.Instance.mRoleData = roleData;
            //同步服务器时间
            ServerTimeManager.Instance.LoginTimeStamp = msg.ServerTimeStamp;
            ServerTimeManager.Instance.SyncServerTime(msg.ServerTimeStamp);
            ServerTimeManager.Instance.IsStart = true;
            ServerTimeManager.Instance.SetNextToZeroServerTime(msg.ServerDailyResetTimeStamp);
            ServerTimeManager.Instance.SetNextWeekServerTime(msg.ServerWeeklyResetTimeStamp);
            //读取本地记录
            string autoPackSt = LocalSave.GetStringWithAccount(DataManager.Instance.GetRoleData().userID, "");
            if (autoPackSt != "")
            {
                AutoOpenEquipStruct openEquipStruct = new AutoOpenEquipStruct();
                JsonObject dataJson = (JsonObject)SimpleJson.DeserializeObject(autoPackSt);
                openEquipStruct.ReadJson(dataJson);
                EquipManager.Instance.OpenEquipStruct = openEquipStruct;
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_LOGIN_PLAYER_SUCCESS);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_PET_UNLOCK);

            var lobby = UIManager.Instance.FindByName("Lobby");
            if (lobby != null && lobby.IsShow())
            {
                UIManager.Instance.HideLoading();
            }
            RoleManager.Instance.InitSkillInfo();
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_GO_TO_Lobby);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("PlayerBasicInfo4Login_PC_Recv Read " + mRecv.Length);
            msg = PlayerBasicInfo4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}