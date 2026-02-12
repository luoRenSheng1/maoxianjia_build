
    using System.Collections.Generic;
    using BigMap;
    using Common;
    using CommonEx;
    using Config;
    using Engine;
    using FairyGUI;
    using EventDispatcher = EngineBase.EventDispatcher;

    public class HuntingShopView : UIViewBase
    {
        private UI_HuntingShopMain _huntingShopMain => this.main as UI_HuntingShopMain;
        
        private List<ConfigEventStoreUnit> _eventStoreUnitList = new List<ConfigEventStoreUnit>();//狩猎商城所有商品列表

        public HuntingShopView()
        {
            this.name = "HuntingShop";
            this.package = "BigMap";
            this.component = "HuntingShopMain";
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
            // _huntingShopMain.closeBtn.onClick.Add(this.Hide);
            _huntingShopMain.closeBtn.onClick.Add(this.HideWithSoundEffect);
            _eventStoreUnitList = ConfigUtils.GetEventStoreUnitList();
            _huntingShopMain.list.itemRenderer = EventShopListRender;
            
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DELEGATE_TASK_POINTS_UPDATE, UpdateShopInfo);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DELEGATE_TASK_POINTS_UPDATE, UpdateShopInfo);
        }

        protected override void OnShow()
        {
            base.OnShow();
            UpdateShopInfo();
        }
        
        private void UpdateShopInfo()
        {
            // _huntingShopMain.currency.icon = UIResource.GetItemUrl();//积分图标
            ((UI_NewCurrency)_huntingShopMain.currency).txtValue.text = DataManager.Instance.GetRoleData().npcTaskPoints.ToString();
            _huntingShopMain.list.numItems = _eventStoreUnitList.Count;
        }

        private void EventShopListRender(int index, GObject item)
        {
            var eventStoreUnit = _eventStoreUnitList[index];
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(eventStoreUnit.ItemId);
            ((UI_HuntingShopItem)item).name.text = ConfigUtils.GetTextById(itemTypeUnit.Name);
            ((UI_HuntingShopItem)item).itemCom.icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
            ((UI_ItemCom)((UI_HuntingShopItem)item).itemCom).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
            ((UI_ItemCom)((UI_HuntingShopItem)item).itemCom).hasCount.selectedIndex = 1;
            // ((UI_HuntingShopItem)item).icon = UIResource.GetItemUrl();//积分图标
            ((UI_HuntingShopItem)item).num.text = eventStoreUnit.Score.ToString();

            int exchangeTimes = TaskInfoManager.Instance.GetNpcTaskExchangeInfoByItemId(eventStoreUnit.Id)?.changeCounter ?? 0;//已兑换次数
            ((UI_HuntingShopItem)item).statuCtrl.selectedIndex = exchangeTimes < eventStoreUnit.Limit ? 0 : 1;//商品状态：是否售罄

            ((UI_HuntingShopItem)item).data = eventStoreUnit;
            ((UI_HuntingShopItem)item).onClick.Add(OnClickShopItem);
        }

        private void OnClickShopItem(EventContext context)
        {
            ConfigEventStoreUnit storeUnit = ((context.sender as UI_HuntingShopItem)?.data) as ConfigEventStoreUnit;
            if (storeUnit != null)
                UIManager.Instance.ShowUIPanel("HuntingShopBuyTips", storeUnit);
        }

    }
