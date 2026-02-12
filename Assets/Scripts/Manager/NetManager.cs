using System;
using System.Collections;
using static GameManager;
using UnityEngine;
using System.Collections.Generic;
using Engine;
using EngineBase;

namespace Engine
{
    public class NetManager : TSingleton<NetManager>
    {
        public class TrifleServiceUnit
        {
            public JsonObject jsMsg;
        }

        // 消息合集，不重要的消息累计后统一发送
        private HashSet<string> setTrifleService = new HashSet<string>();
        private List<TrifleServiceUnit> lstTrifleServiceMsg = new List<TrifleServiceUnit>();
        private CTimer timerTrifleServiceMsg = new CTimer();

        public void OnInit()
        {
            this.Startup();

            timerTrifleServiceMsg.Startup(20.0f);
            lstTrifleServiceMsg.Clear();
            //setTrifleService.Add(MsgDefine.MSG_WORK_REMOTE_REWARD_FOODS);
        }

        public bool SendServiceMessage(string service, JsonObject jsonParams = null)
        {
            return true;
        }

        public bool SendMessage(int id, JsonObject json)
        {
            var msgData = SimpleJson.SerializeObjectInHeap(json);
            return SendMessage(id, msgData);
        }

        public bool SendMessage(int id, string jsonData)
        {
            if (null == GameManager.Instance)
            {
                return false;
            }

            var Connection = GameManager.Instance.Connection;
            
            if (Connection == null || !Connection.IsConnected())
            {
                LogUtils.LogWarning("connection null exception");
                return false;
            }

            return Connection.SendMessage(id, jsonData, null);
        }

        public void OnNetMsg(int id, JsonObject json, JsonObject reqJson)
        {
        }

        public void Tick(float deltaSeconds)
        {
            CheckTrifleServiceMsg();
            TreasureChesManager.Instance.Tick(deltaSeconds);
            OnlineManager.Instance.Tick(deltaSeconds);
            RoleManager.Instance.Tick(deltaSeconds);
            TaskInfoManager.Instance.Tick(deltaSeconds);
            MapChapterManager.Instance.Tick(deltaSeconds);
        }

        private void CheckTrifleServiceMsg()
        {
            if (timerTrifleServiceMsg.ToNextTime())
            {
                CheckAndSendTrifleServiceMsg();
            }
        }

        private void CheckAndSendTrifleServiceMsg()
        {
        }
        
        public void PushTrifleServiceMsgAfterReconnect()
        {
            // 临时处理，服务器要求补包数目不能太多
            if (lstTrifleServiceMsg.Count <= 50)
            {
                CheckAndSendTrifleServiceMsg();
            }
        }
        
        /// <summary>
        /// 主动推送
        /// </summary>
        public void PushTrifleServiceMsg()
        {
            CheckAndSendTrifleServiceMsg();
        }
        
        public void OnMsgRoleUpdate(JsonObject jsonMsg)
        {
            //DataManager.Instance.UpdateData(jsonMsg);
        }

        public void Startup()
        {
            MapObjectManager.Instance.OnInit();
            ServicesHelper.Instance.OnInit();
            EquipManager.Instance.OnInit();
            PetInfoManager.Instance.OnInit();
            HeroInfoManager.Instance.OnInit();
            ShopInfoManager.Instance.OnInit();
            ItemInfoManager.Instance.OnInit();
            FuncPreviewManger.Instance.OnInit();
            TaskInfoManager.Instance.OnInit();
            OnlineManager.Instance.OnInit();
            GuideManager.Instance.OnInit();
            ChatManager.Instance.OnInit();
            PayManager.Instance.OnInit();
            ActivityManager.Instance.OnInit();
            AdManager.Instance.OnInit();
            SkillInfoManager.Instance.OnInit();
            RuneInfoManager.Instance.OnInit();
            PVPMapManager.Instance.OnInit();
            MapChapterManager.Instance.OnInit();
            BuffInfoManager.Instance.OnInit();
        }

        public void Cleanup()
        {
            MapObjectManager.Instance.Dispose();
            ServicesHelper.Instance.Dispose();
            EquipManager.Instance.Dispose();
            TreasureChesManager.Instance.Dispose();
            PetInfoManager.Instance.Dispose();
            HeroInfoManager.Instance.Dispose();
            ShopInfoManager.Instance.Dispose();
            ItemInfoManager.Instance.Dispose();
            VillageInfoManager.Instance.Dispose();
            FuncPreviewManger.Instance.Dispose();
            TaskInfoManager.Instance.Dispose();
            OnlineManager.Instance.Dispose();
            GuideManager.Instance.Dispose();
            ChatManager.Instance.Dispose();
            MapVillagePetManager.Instance.Dispose();
            PayManager.Instance.Dispose();
            ActivityManager.Instance.Dispose();
            AdManager.Instance.Dispose();
            SkillInfoManager.Instance.Dispose();
            RuneInfoManager.Instance.Dispose();
            PVPMapManager.Instance.Dispose();
            MapChapterManager.Instance.Dispose();
            BuffInfoManager.Instance.Dispose();
        }
    }
}