using System.Collections.Generic;
using BigMap;
using CommonEx;
using Config;
using Engine;
using FairyGUI;

    public class FishingRewardView : UIViewBase
    {
        private UI_FishingReward _fishingReward => this.main as UI_FishingReward;
        
        private List<ItemData> _itemList = new List<ItemData>();
        private List<ItemData> _boxShowitemList = new List<ItemData>();

        public FishingRewardView()
        {
            this.name = "FishingReward";
            this.package = "BigMap";
            this.component = "FishingReward";
            this.removePackage = true;
            this.safeAreaInset = true;
            this.type = UIType.Tip;
        }
        
        public override void BindAll()
        {
            base.BindAll();
            BigMapBinder.BindAll();
        }

        protected override void OnInit()
        {
            base.OnInit();
            this._fishingReward.sortingOrder = 998;
            this._fishingReward.getRewardBtn.onClick.Add(this.Hide);
            this._fishingReward.againFishingBtn.onClick.Add(OnClickAgaFishingBtn);
            this._fishingReward.rewardList.itemRenderer = RewardItemRenderer;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
        
        protected override void OnUpdateParams(params object[] values)
        {
            base.OnUpdateParams(values);
            _itemList = values[0] as List<ItemData>;
            this._fishingReward.status.selectedIndex = 1;
            if (values.Length >= 2)
            {
                _boxShowitemList = values[1] as List<ItemData>;
                if (_boxShowitemList != null && _boxShowitemList.Count > 0)
                {
                    this._fishingReward.status.selectedIndex = 0;
                }
            }
            
        }

        protected override void OnShow()
        {
            base.OnShow();
            
            GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralGainRewardSE);
            
            UpdateRewardInfo();
        }

        private void UpdateRewardInfo()
        {
            this._fishingReward.rewardList.numItems = _itemList.Count;
        }

        private void RewardItemRenderer(int index, GObject item)
        {
            ItemData itemData = _itemList[index];
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemData.id);

            // 是否为传承装备
            if (itemData.ItemGuid != 0)
            {
                
                // 传承装备
                if (itemTypeUnit.Type == 14)
                {
                    EquipData equipData = EquipManager.Instance.GetNoWearLoreEquipByGuid(itemData.ItemGuid);
                    if (equipData != null)
                    {
                        ((UI_ItemCom)item).ctrlQuality.selectedIndex = equipData.quality - 1;
                    }
                }
                
                // 宠物
                if (itemTypeUnit.Type == 4)
                {
                    PetItemInfo petItemInfo = PetInfoManager.Instance.GetPet(itemData.ItemGuid);
                    if (petItemInfo != null)
                    {
                        ((UI_ItemCom)item).ctrlQuality.selectedIndex = petItemInfo.quality - 1;
                    }
                }
            }
            else
            {
                // 其他道具
                ((UI_ItemCom)item).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
            }
            
            ((UI_ItemCom)item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
            ((UI_ItemCom)item).itemSpineEff.visible = false;
            ((UI_ItemCom)item).hasCount.selectedIndex = 0;
            ((UI_ItemCom)item).txtLv.text = StringUtils.FormatCurrency((int)itemData.count);

            ((UI_ItemCom)item).data = itemData;
            ((UI_ItemCom)item).onClick.Set(OnItemTips);
        }
        
        private static void OnItemTips(EventContext context)
        {
            ItemData itemData = (ItemData)((UI_ItemCom)context.sender).data;
            if (itemData != null)
            {
                TipsManger.Instance.ShowPopupTip((UI_ItemCom)context.sender, Tipstype.None, itemData.id, itemData.ItemGuid);
            }
        }

        protected override void OnHide()
        {
            base.OnHide();
            if (_boxShowitemList.Count > 0)
            {
                UIManager.Instance.ShowUIPanel("FishingReward", _boxShowitemList);
            }
            _boxShowitemList.Clear();
            
            var view = UIManager.Instance.FindByName("ChapterMap") as  ChapterMapView;
            view?.StopFishingCoroutine();
            view?.CheckHeroNearFishingPos(true);
            view.curFishingPosData = null;  // 防止状态残留
        }

        /// <summary>
        /// 再次垂钓
        /// </summary>
        private void OnClickAgaFishingBtn()
        {
            UIManager.Instance.CloseUIPanel("FishingReward");
            
            //点击再次垂钓，再次抛竿
            var view = UIManager.Instance.FindByName("ChapterMap") as  ChapterMapView;
            view?.StartFishingAnimation(false);
        }
        
        
    }
