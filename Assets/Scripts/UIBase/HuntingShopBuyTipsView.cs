
    using BigMap;
    using CommonEx;
    using Config;
    using Engine;
    using EngineBase;
    using msg;
    using UnityEngine;

    public class HuntingShopBuyTipsView : UIViewBase
    {
        private UI_HuntingShopBuyTips _huntingShopBuyTips => this.main as UI_HuntingShopBuyTips;
        
        private ConfigEventStoreUnit _storeUnit;

        private int _useCount = 1;
        private int _maxCount = 0;//每天购买上限数量
        private int _buyCount = 0;//今日已购买次数
        private int _canBuyCount = 0;//今日剩余购买次数

        public HuntingShopBuyTipsView()
        {
            this.name = "HuntingShopBuyTips";
            this.package = "BigMap";
            this.component = "HuntingShopBuyTips";
            this.removePackage = true;
            this.safeAreaInset = true;
        }

        public override void BindAll()
        {
            base.BindAll();
            BigMapBinder.BindAll();
        }

        protected override void OnUpdateParams(params object[] values)
        {
            base.OnUpdateParams(values);
            _storeUnit = (ConfigEventStoreUnit)values[0];
        }

        protected override void OnInit()
        {
            base.OnInit();
            // _huntingShopBuyTips.closeBtn.onClick.Add(this.Hide);
            _huntingShopBuyTips.closeBtn.onClick.Add(this.HideWithSoundEffect);
            _huntingShopBuyTips.reduceBtn.onClick.Add(this.OnClickReduce);
            _huntingShopBuyTips.addBtn.onClick.Add(this.OnClickAddBtn);
            _huntingShopBuyTips.num.onChanged.Add(this.OnClickNum);
            _huntingShopBuyTips.buyBtn.onClick.Add(this.OnClickBuyBtn);
            
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DELEGATE_TASK_POINTS_UPDATE, UpdateBuyBtnState);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DELEGATE_TASK_POINTS_UPDATE, UpdateBuyBtnState);
        }

        protected override void OnShow()
        {
            base.OnShow();
            _useCount = 1;
            _maxCount = _storeUnit.Limit;//每天购买上限数量
            UpdateBuyBtnState();
            UpdateBuyItemInfo();
            UpdateCntTxt();
        }

        private void UpdateBuyItemInfo()
        {
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(_storeUnit.ItemId);
            ((UI_ItemCom)_huntingShopBuyTips.itemCom).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
            ((UI_ItemCom)_huntingShopBuyTips.itemCom).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
            ((UI_ItemCom)_huntingShopBuyTips.itemCom).hasCount.selectedIndex = 1;
            _huntingShopBuyTips.name.text = ConfigUtils.GetTextById(itemTypeUnit.Name);
            _huntingShopBuyTips.desc.text = ConfigUtils.GetTextById(itemTypeUnit.Desc);
            
        }

        private void UpdateBuyBtnState()
        {
            _buyCount = TaskInfoManager.Instance.GetNpcTaskExchangeInfoByItemId(_storeUnit.Id)?.changeCounter ?? 0;//今日已购买次数
            _canBuyCount = _maxCount - _buyCount;//今日剩余购买次数
            if (_buyCount >= _storeUnit.Limit)
            {
                _huntingShopBuyTips.buyBtn.grayed = true;
            }
            else
            {
                _huntingShopBuyTips.buyBtn.grayed = false;
            }
        }

        /// <summary>
        /// 减少
        /// </summary>
        private void OnClickReduce()
        {
            if (_useCount > 1)
            {
                _useCount--;
                UpdateCntTxt();
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        private void OnClickAddBtn()
        {
            // 不能超过每日购买上限，不能超过可购买的积分数量
            if (_canBuyCount > _useCount)
            {
                _useCount++;
                UpdateCntTxt();
            }
            else
            {
                UIManager.Instance.Toast("今日购买次数已达上限！");
            }
        }

        private void UpdateCntTxt()
        {
            _huntingShopBuyTips.num.text = _useCount.ToString();
            ((UI_ShopBuyBtn)_huntingShopBuyTips.buyBtn).num.text = StringUtils.FormatCurrency(_useCount * _storeUnit.Score);
            _huntingShopBuyTips.addBtn.enabled = DataManager.Instance.GetRoleData().npcTaskPoints > _useCount * _storeUnit.Score;
            
            _huntingShopBuyTips.reduceBtn.enabled = _useCount > 1;
        }

        /// <summary>
        /// 数量
        /// </summary>
        private void OnClickNum()
        {
            string text = _huntingShopBuyTips.num.text;
            if (!string.IsNullOrEmpty(text) && int.Parse(text) > 1)
            {
                //最大输入数不超过货币可支付数量
                if (int.Parse(text) * _storeUnit.Score > DataManager.Instance.GetRoleData().npcTaskPoints)
                {
                    _useCount = DataManager.Instance.GetRoleData().npcTaskPoints / _storeUnit.Score;
                }
                else
                {
                    _useCount = Mathf.Min(_canBuyCount, int.Parse(text));
                }
                UpdateCntTxt();
            }
        }

        /// <summary>
        /// 购买
        /// </summary>
        private void OnClickBuyBtn()
        {
            //条件判断
            if (_useCount > _canBuyCount)
            {
                UIManager.Instance.Toast("今日购买次数已达上限！");
                return;
            }

            if (DataManager.Instance.GetRoleData().npcTaskPoints < _useCount * _storeUnit.Score)
            {
                //积分不足
                UIManager.Instance.Toast("积分不足！");
                return;
            }
            
            var builder = ExchangeItemByNPCPoints_CS.CreateBuilder();
            builder.StoreId = (uint)_storeUnit.Id;
            builder.Amount = (uint)_useCount;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ExchangeItemByNPCPoints_CS, builder.Build());
        }

    }
