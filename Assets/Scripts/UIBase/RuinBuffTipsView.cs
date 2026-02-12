
    using System.Collections;
    using System.Collections.Generic;
    using BigMap;
    using Config;
    using Engine;
    using EngineBase;
    using FairyGUI;
    using UnityEngine;

    public class RuinBuffTipsView : UIViewBase
    {
        private UI_RuinBuffTips _ruinBuffTips => this.main as UI_RuinBuffTips;
        
        private BuffInfo _buffInfo;
        
        private Coroutine _timerCoroutine;// 添加协程引用

        public RuinBuffTipsView()
        {
            this.name = "RuinBuffTips";
            this.package = "BigMap";
            this.component = "RuinBuffTips";
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
            _ruinBuffTips.closeBtn.onClick.Add(ColseRuinBuffTips);
            _ruinBuffTips.attrList.itemRenderer = AttrListRender;

        }
        
        protected override void OnDispose()
        {
            base.OnDispose();
            // 停止计时器协程
            StopTimer();
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
            _buffInfo = values[0] as BuffInfo;
        }
        
        protected override void OnShow()
        {
            base.OnShow();
            UpdateBuffInfo();
            
            StopTimer();
            // 启动计时器协程
            _timerCoroutine = GameManager.Instance.StartCoroutine(UpdateTimerCoroutine());
        }
        
        // 计时器协程
        private IEnumerator UpdateTimerCoroutine()
        {
            ulong serverTime = ServerTimeManager.Instance.CurServerTime;
            
            while (serverTime < _buffInfo.endTime)
            {
                UpdateBuffTime(); // 更新时间显示
                yield return new WaitForSeconds(1f); // 每秒更新一次
            }
            
            UpdateBuffTime(); // 再执行一次，确保自动提交
            _timerCoroutine = null;
        }

        // buff时长
        private void UpdateBuffTime()
        {
            ulong serverTime = ServerTimeManager.Instance.CurServerTime;

            if (serverTime >= _buffInfo.endTime)
            {
                _ruinBuffTips.time.text = "00:00";
                UIManager.Instance.CloseUIPanel("RuinBuffTips");
                return;
            }
            
            ulong remainingSeconds = _buffInfo.endTime - serverTime;
            uint minutes = (uint)((remainingSeconds % 3600) / 60);
            uint seconds = (uint)(remainingSeconds % 60);

            _ruinBuffTips.time.text = $"{minutes:D2}:{seconds:D2}";

        }

        private void UpdateBuffInfo()
        {
            ConfigRuinsBuffUnit ruinsBuffUnit = ConfigUtils.GetRuinsBuffDataById(_buffInfo.cfgId);
            
            _ruinBuffTips.name.text = ConfigUtils.GetTextById(ruinsBuffUnit.Name);
            _ruinBuffTips.ruinBtn.icon = UIResource.GetTalentUrl(ruinsBuffUnit.Icon.ToString());
            
            string[] attrs = ruinsBuffUnit.Attr.Split('|');
            List<ItemData> itemDataList = new List<ItemData>();
            for (int i = 0; i < attrs.Length; i++)
            {
                ItemData itemData = new ItemData();
                itemData.id = int.Parse(attrs[i].Split(',')[0]);
                itemData.count = int.Parse(attrs[i].Split(',')[1]);
                
                itemDataList.Add(itemData);
            }
            _ruinBuffTips.attrList.data = itemDataList;
            _ruinBuffTips.attrList.numItems = itemDataList.Count;
            _ruinBuffTips.attrList.ResizeToFit();
        }

        private void AttrListRender(int index, GObject item)
        {
            List<ItemData> itemDataList = item.parent.data as List<ItemData>;
            string attrName = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(itemDataList[index].id).AttrName);
            string value = EquipManager.Instance.SetAttributeValue(itemDataList[index].id, itemDataList[index].count, true);
            
            ((UI_BuffAttrItem2)item).attrValue.SetVar("name", attrName).SetVar("value", value).FlushVars();
        }

        private void ColseRuinBuffTips()
        {
            UIManager.Instance.CloseUIPanel("RuinBuffTips");
        }

    }
