
    using System.Collections;
    using System.Collections.Generic;
    using BigMap;
    using Common;
    using Config;
    using Engine;
    using FairyGUI;
    using msg;
    using UnityEngine;
    using EventDispatcher = EngineBase.EventDispatcher;

    public class HuntingTaskMainView : UIViewBase
    {
        private UI_HuntingTaskMain _huntingTaskMain => this.main as UI_HuntingTaskMain;
        
        // private ulong eventGuid;
        private int taskId;
        private bool isShow;//是否需要展示任务选择界面
        private ConfigEventTaskUnit eventTaskUnit;

        private int taskType;
        private Coroutine _timerCoroutine;// 添加协程引用

        public HuntingTaskMainView()
        {
            this.name = "HuntingTaskMain";
            this.package = "BigMap";
            this.component = "HuntingTaskMain";
            this.removePackage = true;
            this.safeAreaInset = true;
        }
        
        public override void BindAll()
        {
            base.BindAll();
            BigMapBinder.BindAll();
        }

        protected override void OnInit()
        {
            base.OnInit();
            // _huntingTaskMain.closeBtn.onClick.Add(this.Hide);
            _huntingTaskMain.closeBtn.onClick.Add(this.HideWithSoundEffect);
            _huntingTaskMain.taskInfo.giveUpTaskBtn.onClick.Add(this.OnClickGiveUpTask);
            _huntingTaskMain.taskInfo.getRewardBtn.onClick.Add(this.OnClickGetReward);
            _huntingTaskMain.taskInfo.submitBtn.onClick.Add(this.OnClickSubmit);//上供
            _huntingTaskMain.huntingShopBtn.onClick.Add(this.OnClickHuntingShopBtn);
            
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DELEGATE_TASK_UPDATE, UpdateTaskInfo);
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, UpdateGoldAndDiaInfo);
        }
        
        protected override void OnDispose()
        {
            base.OnDispose();
            // 停止计时器协程
            if (_timerCoroutine != null) 
            {
                GameManager.Instance.StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DELEGATE_TASK_UPDATE, UpdateTaskInfo);
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ITEM_UPDATE, UpdateGoldAndDiaInfo);
        }
        
        protected override void OnUpdateParams(params object[] values)
        {
            base.OnUpdateParams(values);
            taskId = (int)values[0];
            isShow = (bool)values[1];
            // eventGuid = (ulong)values[1];
        }
        
        protected override void OnShow()
        {
            base.OnShow();
            UpdateGoldAndDiaInfo();
            UpdateTaskInfo();
            MapChapterManager.Instance.isShowTaskSelectUI = isShow;
            // 启动计时器协程
            _timerCoroutine = GameManager.Instance.StartCoroutine(UpdateTimerCoroutine());
        }
        
        // 计时器协程
        private IEnumerator UpdateTimerCoroutine()
        {
            ulong currentTime = ServerTimeManager.Instance.CurServerTime;
            NpcTaskData npcTaskData = TaskInfoManager.Instance.GetNpcTask();
            
            while (currentTime < npcTaskData.endTime)
            {
                UpdateTaskTime(); // 更新时间显示
                yield return new WaitForSeconds(1f); // 每秒更新一次
            }
        }
        
        // 任务时间
        private void UpdateTaskTime()
        {
            NpcTaskData npcTaskData = TaskInfoManager.Instance.GetNpcTask();
            if (npcTaskData == null) return;
            
            ulong currentTime = ServerTimeManager.Instance.CurServerTime;
            
            if (currentTime >= npcTaskData.endTime)
            {
                // 倒计时结束处理
                _huntingTaskMain.timeLb.text = "00:00:00";
                // UIManager.Instance.CloseUIPanel("HuntingTaskMain");
                if (eventTaskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityEquips || eventTaskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityLoreEquips || eventTaskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityPets)
                {
                    if (npcTaskData.progress < int.Parse(eventTaskUnit.Param.Split(',')[0]))
                    {
                        UIManager.Instance.CloseUIPanel("HuntingTaskMain");
                    }
                }
                else
                {
                    if (npcTaskData.progress < int.Parse(eventTaskUnit.Param))
                    {
                        UIManager.Instance.CloseUIPanel("HuntingTaskMain");
                    }
                }
                return;
            }
            
            ulong remainingSeconds = npcTaskData.endTime - currentTime;
            
            // 转换为时分秒
            uint hours = (uint)(remainingSeconds / 3600);
            uint minutes = (uint)((remainingSeconds % 3600) / 60);
            uint seconds = (uint)(remainingSeconds % 60);
            
            _huntingTaskMain.timeLb.text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
            
        }

        private void UpdateGoldAndDiaInfo()
        {
            ((UI_NewCurrency)_huntingTaskMain.goldCurrency).icon = UIResource.GetItemUrl(2000.ToString());
            ((UI_NewCurrency)_huntingTaskMain.goldCurrency).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().gold);
            
            ((UI_NewCurrency)_huntingTaskMain.diaCurrency).icon = UIResource.GetItemUrl(1000.ToString());
            ((UI_NewCurrency)_huntingTaskMain.diaCurrency).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().dia);
        }

        private void UpdateTaskInfo()
        {
            // _huntingTaskMain.currency.icon = UIResource.GetItemUrl();//积分图标
            ((UI_NewCurrency)_huntingTaskMain.currency).txtValue.text = DataManager.Instance.GetRoleData().npcTaskPoints.ToString();
            
            eventTaskUnit = ConfigUtils.GetEventTaskUnitById(taskId);
            if (eventTaskUnit == null) return;
            taskType = eventTaskUnit.Type;
            // _huntingTaskMain.taskInfo.item.icon = UIResource.GetItemUrl();//积分图标
            // ((UI_TaskItem)_huntingTaskMain.taskInfo).name.text = ConfigUtils.GetTextById(eventTaskUnit.Name);
            ((UI_TaskItem)_huntingTaskMain.taskInfo).item.num.text = eventTaskUnit.Reward;

            NpcTaskData npcTaskData = TaskInfoManager.Instance.GetNpcTask();
            if (npcTaskData == null) return;

            if (eventTaskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityEquips || eventTaskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityLoreEquips || eventTaskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityPets)
            {
                int num = int.Parse(eventTaskUnit.Param.Split(',')[0]);
                int quality = int.Parse(eventTaskUnit.Param.Split(',')[1]);
                string qualityName = EquipManager.Instance.GetQualityName((QualityType)quality);
                ((UI_TaskItem)_huntingTaskMain.taskInfo).name.text = StringUtils.Format(ConfigUtils.GetTextById(eventTaskUnit.Doc), num, qualityName);
                int progress = Mathf.Min(num, npcTaskData.progress);
                _huntingTaskMain.taskInfo.bar.min = 0;
                _huntingTaskMain.taskInfo.bar.max = num;
                _huntingTaskMain.taskInfo.bar.value = progress;
                
                if (npcTaskData.progress >= num)
                {
                    //已完成
                    _huntingTaskMain.taskInfo.statuCtrl.selectedIndex = 2;
                }
                else
                {
                    //未完成   上供类型任务特殊处理
                    _huntingTaskMain.taskInfo.statuCtrl.selectedIndex = 1;
                
                }
            }
            else
            {
                ((UI_TaskItem)_huntingTaskMain.taskInfo).name.text = StringUtils.Format(ConfigUtils.GetTextById(eventTaskUnit.Doc), eventTaskUnit.Param);
                int progress = Mathf.Min(int.Parse(eventTaskUnit.Param),npcTaskData.progress);
                _huntingTaskMain.taskInfo.bar.min = 0;
                _huntingTaskMain.taskInfo.bar.max = int.Parse(eventTaskUnit.Param);
                _huntingTaskMain.taskInfo.bar.value = progress;
                
                if (npcTaskData.progress >= int.Parse(eventTaskUnit.Param))
                {
                    //已完成
                    _huntingTaskMain.taskInfo.statuCtrl.selectedIndex = 2;
                }
                else
                {
                    //未完成   上供类型任务特殊处理
                    if (eventTaskUnit.Type == (int)eMainTaskType.eMainTaskType_OfferGoldOnAltar || eventTaskUnit.Type == (int)eMainTaskType.eMainTaskType_OfferDiamondOnAltar)
                    {
                        //上供类型
                        _huntingTaskMain.taskInfo.statuCtrl.selectedIndex = 3;
                    }
                    else
                    {
                        _huntingTaskMain.taskInfo.statuCtrl.selectedIndex = 1;
                    }
                
                }
            }

        }

        /// <summary>
        /// 放弃狩猎任务
        /// </summary>
        private void OnClickGiveUpTask()
        {
            if (taskId > 0)
            {
                var builder = DropNPCTask_CS.CreateBuilder();
                builder.TaskId = (uint)taskId;
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_DropNPCTask_CS, builder.Build());
                
            }
        }

        /// <summary>
        /// 领取奖励
        /// </summary>
        private void OnClickGetReward()
        {
            var builder = ClaimNPCTaskAward_CS.CreateBuilder();
            builder.TaskId = (uint)taskId;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimNPCTaskAward_CS, builder.Build());
        }

        /// <summary>
        /// 上供
        /// </summary>
        private void OnClickSubmit()
        {
            UIManager.Instance.ShowUIPanel("SubmitGoldAndDiaMain", taskId);
        }

        /// <summary>
        /// 打开狩猎商店
        /// </summary>
        private void OnClickHuntingShopBtn()
        {
            UIManager.Instance.ShowUIPanel("HuntingShop");
        }

    }
