using BigMap;
using Config;
using Engine;
using EngineBase;
using msg;

    public class SubmitGoldAndDiaMainView : UIViewBase
    {
        private UI_SubmitGoldAndDiaMain _submitGoldAndDiaMain => this.main as UI_SubmitGoldAndDiaMain;
        
        private int taskId;
        private ConfigEventTaskUnit eventTaskUnit;

        public SubmitGoldAndDiaMainView()
        {
            this.name = "SubmitGoldAndDiaMain";
            this.package = "BigMap";
            this.component = "SubmitGoldAndDiaMain";
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
            // this._submitGoldAndDiaMain.closeBtn.onClick.Add(this.Hide);
            this._submitGoldAndDiaMain.closeBtn.onClick.Add(this.HideWithSoundEffect);
            this._submitGoldAndDiaMain.giveUpBtn.onClick.Add(this.OnClickGiveUp);
            this._submitGoldAndDiaMain.submitBtn.onClick.Add(this.OnClickSubmit);
            
            // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DELEGATE_TASK_POINTS_UPDATE, UpdateShopInfo);
        }
        
        protected override void OnDispose()
        {
            base.OnDispose();
            // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DELEGATE_TASK_POINTS_UPDATE, UpdateShopInfo);
        }
        
        protected override void OnUpdateParams(params object[] values)
        {
            base.OnUpdateParams(values);
            taskId = (int)values[0];
        }
        
        protected override void OnShow()
        {
            base.OnShow();
            eventTaskUnit = ConfigUtils.GetEventTaskUnitById(taskId);
            if (eventTaskUnit == null) return;

            if (eventTaskUnit.Type == (int)eMainTaskType.eMainTaskType_OfferGoldOnAltar)
            {
                _submitGoldAndDiaMain.typeCtrl.selectedIndex = 0;//金币
            }
            else
            {
                _submitGoldAndDiaMain.typeCtrl.selectedIndex = 1;//砖石
            }
            
            _submitGoldAndDiaMain.desc.SetVar("value",eventTaskUnit.Param).FlushVars();
        }

        /// <summary>
        /// 放弃任务
        /// </summary>
        private void OnClickGiveUp()
        {
            // CloseSubmitUI();
            
            if (taskId > 0)
            {
                var builder = DropNPCTask_CS.CreateBuilder();
                builder.TaskId = (uint)taskId;
                GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_DropNPCTask_CS, builder.Build());
            }
        }

        /// <summary>
        /// 上供
        /// </summary>
        private void OnClickSubmit()
        {
            CloseSubmitUI();
            
            //金币或砖石是否充足
            if (DataManager.Instance.GetRoleData().gold < int.Parse(eventTaskUnit.Param) && eventTaskUnit.Type == (int)eMainTaskType.eMainTaskType_OfferGoldOnAltar)
            {
                UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8038, ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(2000).Name)));
                return;
            }
            if (DataManager.Instance.GetRoleData().dia < int.Parse(eventTaskUnit.Param) && eventTaskUnit.Type == (int)eMainTaskType.eMainTaskType_OfferDiamondOnAltar)
            {
                UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8038, ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(1000).Name)));
                return;
            }
            
            if (taskId > 0)
            {
                var builder = OfferItem4Task_CS.CreateBuilder();
                builder.TaskId = (uint)taskId;
                builder.TaskType = eTaskType.eTaskType_NPC;
                GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_OfferItem4Task_CS, builder.Build());
            }
        }

        private void CloseSubmitUI()
        {
            UIManager.Instance.CloseUIPanel("SubmitGoldAndDiaMain");
        }

    }
