
    using System.Collections;
    using System.Collections.Generic;
    using BigMap;
    using Config;
    using Engine;
    using EngineBase;
    using FairyGUI;
    using msg;
    using UnityEngine;
    using EventDispatcher = EngineBase.EventDispatcher;

    public class RuinSelectMainView : UIViewBase
    {
        private UI_RuinSelectMain _ruinSelectMain => this.main as UI_RuinSelectMain;
        
        private ulong eventGuid;
        private uint monsterIndex;
        private List<int> _ruinBuffList =  new List<int>();
        
        private Coroutine _timerCoroutine;// 添加协程引用
        private int _selectedBuffId;//选中的buff
        private int _selectedTime = 30;//选择时间倒计时时长
        
        private UI_RuinItem _selectedItemUI;

        public RuinSelectMainView()
        {
            this.name = "RuinSelectMain";
            this.package = "BigMap";
            this.component = "RuinSelectMain";
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
            _ruinSelectMain.ruinList.itemRenderer = RuinBuffRender;
            _ruinSelectMain.submitBtn.onClick.Add(this.OnSubmitBtn);
            
            // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_AUTOEXIT_RUIN, AutoExitRuin);
        }
        
        protected override void OnDispose()
        {
            base.OnDispose();
            // 停止计时器协程
            StopTimer();

            // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_AUTOEXIT_RUIN, AutoExitRuin);
        }

        protected override void OnHide()
        {
            base.OnHide();
            StopTimer();
        }

        private void StopTimer()
        {
            if (_timerCoroutine != null)
            {
                GameManager.Instance.StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }

        protected override void OnUpdateParams(params object[] values)
        {
            base.OnUpdateParams(values);
            eventGuid = (ulong)values[0];
            monsterIndex = (uint)values[1];
            _ruinBuffList = values[2] as List<int>;
        }
        
        protected override void OnShow()
        {
            base.OnShow();
            _selectedBuffId = 0;
            _selectedTime = 30;
            _ruinSelectMain.submitBtn.touchable = true;
            UpdateRuinBuffInfo();

            StopTimer();
            _timerCoroutine = GameManager.Instance.StartCoroutine(UpdateTimerCoroutine());
        }
        
        // 计时器协程
        private IEnumerator UpdateTimerCoroutine()
        {
            while (_selectedTime > 0)
            {
                UpdateBuffSelectedTime(); // 更新时间显示
                yield return new WaitForSeconds(1f); // 每秒更新一次
            }
            
            UpdateBuffSelectedTime(); // 再执行一次，确保自动提交
            _timerCoroutine = null;
        }

        private void UpdateBuffSelectedTime()
        {
            // 倒计时结束，自动随机选择一个buff
            // 策划要求：只有打开buff三选一界面时，才能领取buff，其他情况都不给buff
            if (_selectedTime <= 0)
            {
                if (_selectedBuffId == 0)
                {
                    // 还没有选择buff,随机选择一个buff
                    System.Random random = new System.Random();
                    int randomIndex = random.Next(_ruinBuffList.Count);
                    _selectedBuffId = _ruinBuffList[randomIndex];
                    _selectedItemUI = FindFirstRuinItemByBuffId(_selectedBuffId);
                }

                ShowBuffFly();
                
                SendClaimBuffCS(_selectedBuffId);
                // UIManager.Instance.CloseUIPanel("RuinSelectMain");
                return;
            }

            //倒计时时长为30秒
            _ruinSelectMain.time.text = _selectedTime.ToString() + "s";
            _selectedTime--;

        }

        private void UpdateRuinBuffInfo()
        {
            _ruinSelectMain.ruinList.numItems = _ruinBuffList.Count;
            
            for (int i = 0; i < _ruinSelectMain.ruinList.numChildren; i++)
            {
                UI_RuinItem item = (UI_RuinItem)_ruinSelectMain.ruinList.GetChildAt(i);
                item.isSelected.selectedIndex = 0;
            }
        }

        private void RuinBuffRender(int index, GObject item)
        {
            ConfigRuinsBuffUnit ruinsBuffUnit = ConfigUtils.GetRuinsBuffDataById(_ruinBuffList[index]);
            
            ((UI_RuinItem)item).ruinBtn.icon = UIResource.GetTalentUrl(ruinsBuffUnit.Icon.ToString());
            ((UI_RuinItem)item).name.text = ConfigUtils.GetTextById(ruinsBuffUnit.Name);
            ((UI_RuinItem)item).ruinTime.SetVar("value", ruinsBuffUnit.Time.ToString()).FlushVars();
            
            //配置表没有描述字段，展示属性名及属性值
            ((UI_RuinItem)item).attrList.itemRenderer = BuffAttrRender;
            string[] attrs = ruinsBuffUnit.Attr.Split('|');
            List<ItemData> itemDataList = new List<ItemData>();
            for (int i = 0; i < attrs.Length; i++)
            {
                ItemData itemData = new ItemData();
                itemData.id = int.Parse(attrs[i].Split(',')[0]);
                itemData.count = int.Parse(attrs[i].Split(',')[1]);
                
                itemDataList.Add(itemData);
            }
            ((UI_RuinItem)item).attrList.data = itemDataList;
            ((UI_RuinItem)item).attrList.numItems = itemDataList.Count;

            ((UI_RuinItem)item).data = ruinsBuffUnit.Id;
            ((UI_RuinItem)item).onClick.Add(OnClickRuinBuffItem);
        }

        private void BuffAttrRender(int index, GObject item)
        {
            List<ItemData> itemDataList = item.parent.data as List<ItemData>;
            string attrName = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(itemDataList[index].id).AttrName);
            string value = EquipManager.Instance.SetAttributeValue(itemDataList[index].id, itemDataList[index].count, true);
            ((UI_BuffAttrItem) item).attr.SetVar("name", attrName).SetVar("value", value).FlushVars();
        }

        /// <summary>
        /// 领取buff
        /// </summary>
        /// <param name="context"></param>
        private void OnClickRuinBuffItem(EventContext context)
        {
            UI_RuinItem clickedItem  = (UI_RuinItem)context.sender;
            int buffId = (int)clickedItem.data;

            GComponent listContainer = clickedItem.parent;
            for (int i = 0; i < listContainer.numChildren; i++)
            {
                UI_RuinItem item = (UI_RuinItem)listContainer.GetChildAt(i);
                item.isSelected.selectedIndex = (item == clickedItem) ? 1 : 0;
            }
            
            if (buffId == 0) return;
            
            _selectedBuffId = buffId;
            _selectedItemUI = clickedItem;
        }

        private void OnSubmitBtn()
        {
            if (_selectedBuffId <= 0)
            {
                UIManager.Instance.Toast("你还未选择buff！");
                return;
            }
            
            _ruinSelectMain.submitBtn.touchable = false;

            ShowBuffFly();
            
            SendClaimBuffCS(_selectedBuffId);
            
            // UIManager.Instance.CloseUIPanel("RuinSelectMain");
        }

        private void SendClaimBuffCS(int buffId)
        {
            var builder = ClaimBuff_CS.CreateBuilder();
            builder.EventGuid = eventGuid;
            builder.MonsterIndex = (uint)monsterIndex;
            builder.BuffId = (uint)buffId;
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ClaimBuff_CS, builder.Build());
        }

        /// <summary>
        /// buff图标飞行效果
        /// </summary>
        private void ShowBuffFly()
        {
            if (_selectedBuffId == 0 || _selectedItemUI == null) return;
            
            ConfigRuinsBuffUnit ruinsBuffUnit = ConfigUtils.GetRuinsBuffDataById(_selectedBuffId);
            if (ruinsBuffUnit == null) return;
            
            RuinMapView ruinMapView = UIManager.Instance.FindByName("RuinMap") as RuinMapView;
            UIItemsGain itemsGain = UIGainBasePool.CreateUIGainBase();
            itemsGain.ApplyItemSourceToDestinationEx(ruinMapView?.GetRuinBuffList().asCom, UIResource.GetTalentUrl(ruinsBuffUnit.Icon.ToString()));
            
            // 获取当前选中 UI 项中 ruinBtn 的坐标
            GObject ruinBtn = _selectedItemUI.ruinBtn;
            Vector2 localCenter = new Vector2(ruinBtn.width / 2f, ruinBtn.height / 2f);
            Vector2 globalPos = ruinBtn.LocalToGlobal(localCenter);
            
            itemsGain.StartItemFly(globalPos, 1, false);
            
            GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 11, 1,  () =>
            {
                UIManager.Instance.CloseUIPanel("RuinSelectMain");
            });

        }
        
        private UI_RuinItem FindFirstRuinItemByBuffId(int buffId)
        {
            for (int i = 0; i < _ruinSelectMain.ruinList.numChildren; i++)
            {
                UI_RuinItem item = (UI_RuinItem)_ruinSelectMain.ruinList.GetChildAt(i);
                if ((int)item.data == buffId)
                    return item;
            }
            return null;
        }

    }
