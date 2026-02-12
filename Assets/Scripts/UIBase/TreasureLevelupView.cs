using Common;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using System.Collections.Generic;
using TreasureLevelup;
using EventDispatcher = EngineBase.EventDispatcher;

public class TreasureLevelupView : UIViewBase
{
    private UI_Main levelUpUI => this.main as UI_Main;
    private int type = 0;
    private bool enableLevelup = false;
    private float _itemWitdh = 0;
    private ConfigTreasureChestUnit treasureChestUnit;
    private SortedDictionary<int, string> propDict;
    private SortedDictionary<int, string> nextPropDict;
    private float _itemValue = 0;
    private bool _isGuiding;

    private ConfigCommonUnit _common213;

    private bool _isWatchAd = false;
    
    public TreasureLevelupView()
    {
        this.name = "TreasureLevelup";
        this.package = "TreasureLevelup";
        this.component = "Main";
        this.removePackage = true;
        this.safeAreaInset = false;
        this.GuideType = FuncType.Guide;
    }

    protected override void OnInit()
    {
        base.OnInit();

        _common213 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(213);
        // this.levelUpUI.panel.closeBtn.onClick.Add(this.Hide);
        this.levelUpUI.panel.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.levelUpUI.panel.closeBtn.onClick.Add(ChkGuide);
        this.levelUpUI.panel.btnUpgrade.onClick.Set(OnClickUpgrade);
        this.levelUpUI.panel.btnBuy.onClick.Set(OnClickBuy);
        this.levelUpUI.panel.btnSkip.onClick.Set(OnClickSkip);
        this.levelUpUI.panel.btnHasten.onClick.Set(OnClickHasten);
        this.levelUpUI.panel.listAttrs.itemRenderer = OnRenderListItem;
        // this.levelUpUI.panel.btnCurrency1.onClick.Set(OnClickGold);
        this.levelUpUI.panel.btnCurrency.onClick.Set(OnClickGold);
        // this.levelUpUI.panel.btnCurrency2.onClick.Set(OnClickSpeedItem);
        this.levelUpUI.panel.treasureNumPro.itemRenderer = ItemTreasureNumBarListItem;

        EventDispatcher.GameWorld.Regist<int, string>(EventDefine.EVENT_TREASURE_LEVELUP_RES, OnTreasureChesLevelUpRes);
        EventDispatcher.GameWorld.Regist<bool>(EventDefine.EVENT_TREASURE_PROGRESS_UPSUCC, OnTreasureProgressUpSucc);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ROLE_UPDATE, OnRoleUpdate);
        EventDispatcher.GameWorld.Regist<string>(EventDefine.EVENT_TREASURE_CD_UPDATE, OnTreasureCdUpdate);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, UpdateSpeedTicket);
    }

    public override void BindAll()
    {
        base.BindAll();

        TreasureLevelupBinder.BindAll();
    }

    protected override void OnDispose()
    {
        base.OnDispose();

        EventDispatcher.GameWorld.UnRegist<int, string>(EventDefine.EVENT_TREASURE_LEVELUP_RES, OnTreasureChesLevelUpRes);
        EventDispatcher.GameWorld.UnRegist<bool>(EventDefine.EVENT_TREASURE_PROGRESS_UPSUCC, OnTreasureProgressUpSucc);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ROLE_UPDATE, OnRoleUpdate);
        EventDispatcher.GameWorld.UnRegist<string>(EventDefine.EVENT_TREASURE_CD_UPDATE, OnTreasureCdUpdate);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ITEM_UPDATE, UpdateSpeedTicket);
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateTreasureProgress();
        UpdateUIInfo();
        UpdateSpeedTicket();
        ChkGuide();
    }
    /// <summary>
    /// 引导触发
    /// </summary>
    private void ChkGuide()
    {
        //引导-点击购买按钮1
        if(this.levelUpUI.panel.type.selectedIndex == 0)
        {
            if(TreasureChesManager.Instance.TreasureBoxCurNum == 0 && GuideManager.Instance.StarGuideByData(new GuideData()
            {
                bid = GuideID.NewAccount_ClickSecretCanon1,
                giding = GuideID.NewAccount_MPUpLevel,
                gid = GuideID.NewAccount_MPBuy1,
                tui = this.levelUpUI.panel.btnBuy,
                isForce = true,
                isSend = true,
                npcTxt = "Beginner_Doc_007",
                npcPosType = PosType.Down,
            })) {
                _isGuiding = true;
                return;
            }

            //引导-点击购买按钮2（需要检测是否已经购买过一次）
            if (TreasureChesManager.Instance.TreasureBoxCurNum == 1 && GuideManager.Instance.StarGuideByData(new GuideData()
            {
                bid = GuideID.NewAccount_MPBuy1,
                gid = GuideID.NewAccount_MPBuy2,
                tui = this.levelUpUI.panel.btnBuy,
                isForce = true,
                isSend = true
            })) { _isGuiding = true; return; }

            //引导-点击升级按钮
            if(DataManager.Instance.GetTreasureData().id == 1 && GuideManager.Instance.StarGuideByData(new GuideData()
            {
                bid = GuideID.NewAccount_MPBuy2,
                gid = GuideID.NewAccount_MPUpLevel1,
                tui = this.levelUpUI.panel.btnUpgrade,
                isForce = true,
                isSend = true
            })) { _isGuiding = true; return; }

        }
        //引导-点击加速按钮,1级才能引导
        else if (this.levelUpUI.panel.type.selectedIndex == 1 && DataManager.Instance.GetTreasureData().id == 1)
        {
            if(GuideManager.Instance.StarGuideByData(new GuideData()
            {
                bid = GuideID.NewAccount_MPUpLevel1,
                gid = GuideID.NewAccount_MPSpeed,
                tui = this.levelUpUI.panel.btnHasten,
                isForce = true,
                isSend = true
            })) { _isGuiding = true; return; }
        }
        //引导-点击关闭退出界面按钮
        else if (GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_MPSpeedDiam) && DataManager.Instance.GetTreasureData().id == 2)
        {
            //在lobby界面去强制关闭装备页
            var view = UIManager.Instance.FindByName("Lobby") as LobbyView;
            view?.ChkGuide();
        }
    }

    protected override void OnHide()
    {
        base.OnHide();
        if (_isGuiding)
        {
            GuideManager.Instance.HideGuide();
            _isGuiding = false;
        }
        Utils.HideUIPrefab(this.levelUpUI.panel.spine);

        GuideManager.Instance.HideGuide();
        //在lobby界面去强制关闭装备页
        var view = UIManager.Instance.FindByName("Lobby") as LobbyView;
        view?.ChkGuide();
    }

    private void PlaySpine()
    {
        Utils.ShowUIPrefab(this.levelUpUI.panel.spine,"UI_Player_LvUp", 100f, "Effect/");
    }

    private void UpdateSpeedTicket()
    {
        // ((UI_BtnCurrency)(this.levelUpUI.panel.btnCurrency2)).txtValue.text = ItemInfoManager.Instance.GetItemCount(ConstDefine.Item_SpeedCardId).ToString();
    }

    private void OnTreasureCdUpdate(string timeCdStr)
    {
        levelUpUI.panel.txtCountdown.text = timeCdStr;
    }

    private void OnRoleUpdate()
    {
        // ((UI_BtnCurrency)(this.levelUpUI.panel.btnCurrency1)).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().gold);
        ((UI_CurrencyAdd)(this.levelUpUI.panel.btnCurrency)).value.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().gold);
        
        if (TreasureChesManager.Instance.TreasureBoxOpenStatus != TreasureBoxOpenStatus.ProgressBarFull)
        {
            double curGold = double.Parse(ConfigUtils.GetTreasureChestUnitById(DataManager.Instance.GetTreasureData().id).GoldCoins);
            this.levelUpUI.panel.btnBuy.grayed = !(DataManager.Instance.GetRoleData().gold > curGold);
        }
    }

    private void UpdateTreasureProgress()
    {
        int totalSecond = (int)(DataManager.Instance.GetTreasureData().lastTargetTime - ServerTimeManager.Instance.CurServerTime);
        if (totalSecond > 0)
        {
            TreasureChesManager.Instance.TreasureBoxOpenStatus = TreasureBoxOpenStatus.LevelUp;
        }
        else
        {
            TreasureChesManager.Instance.TreasureBoxOpenStatus = TreasureBoxOpenStatus.None;
        }
        treasureChestUnit = ConfigUtils.GetTreasureChestUnitById(DataManager.Instance.GetTreasureData().id);
        if (treasureChestUnit != null)
        {
            this.levelUpUI.panel.btnUpgrade.grayed = true;
            this.levelUpUI.panel.btnUpgrade.enabled = true;
            // this.levelUpUI.panel.lvRedDot.visible = true;
            
            if (_isWatchAd)
            {
                this.levelUpUI.panel.btnUpgrade.enabled = false;
            }
            if(TreasureChesManager.Instance.TreasureBoxOpenStatus == TreasureBoxOpenStatus.LevelUp)
            {
                var data = TreasureChesManager.Instance.CheckTreasureChes();
                int code = data.Item1;

                this.type = code == 0 ? 1 : 0;
                this.levelUpUI.panel.type.SetSelectedIndexEx(this.type);
                _isWatchAd = false;
                ChkGuide();
            }else if (TreasureChesManager.Instance.TreasureBoxCurNum == treasureChestUnit.GoldCoinsNum && !_isWatchAd && TreasureChesManager.Instance.TreasureBoxOpenStatus == TreasureBoxOpenStatus.None)
            {
                TreasureChesManager.Instance.TreasureBoxOpenStatus = TreasureBoxOpenStatus.ProgressBarFull;
                this.levelUpUI.panel.treasureProType.selectedIndex = 1;
                this.levelUpUI.panel.type.SetSelectedIndexEx(0);
                enableLevelup = true;
                this.levelUpUI.panel.btnUpgrade.grayed = false;
                // this.levelUpUI.panel.lvRedDot.visible = true;
            }else if (TreasureChesManager.Instance.TreasureBoxOpenStatus == TreasureBoxOpenStatus.ProgressBarFull)
            {
                this.levelUpUI.panel.treasureProType.selectedIndex = 1;
                this.levelUpUI.panel.type.SetSelectedIndexEx(0);
                enableLevelup = true;
                this.levelUpUI.panel.btnUpgrade.grayed = false;
                // this.levelUpUI.panel.lvRedDot.visible = true;
            }
            else
            {
                this.levelUpUI.panel.treasureProType.selectedIndex = 0;
                this.levelUpUI.panel.type.SetSelectedIndexEx(0);
                this.levelUpUI.panel.btnUpgrade.grayed = true;
                // this.levelUpUI.panel.lvRedDot.visible = false;
                TreasureChesManager.Instance.TreasureBoxOpenStatus = TreasureBoxOpenStatus.None;
                _isWatchAd = false;
            }
        }

        _isGuiding = false;
    }

    private void OnClickClose()
    {
        this.SetVisible(false);
    }

    private void OnClickUpgrade()
    {
        if (!this.enableLevelup)
        {
            UIManager.Instance.ToastByKey(StringDefine.STRING_LEVEL_ERROR);
        }
        else
        {
            TreasureChesManager.Instance.TreasureChesLevelUp();
        }
    }

    private void OnClickBuy()
    {
        TreasureChesManager.Instance.BuyTreasureBoxNum();
    }
    

    private void OnClickSkip()
    {
        if (AdManager.Instance.GetAdFreeTimes((int) ePlayerAttrID.ePlayerAttrID_BoxAccFreeTimes) > 0)
        {
            AdManager.Instance.WatchAd(() =>
            {
                var builder = EquipBox_AccLevelupByAD_CS.CreateBuilder();
                _isWatchAd = true;
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_EquipBox_AccLevelupByAD_CS, builder.Build());
            });
        }
        else
        {
            UIManager.Instance.ToastByKey(10179);
        }

    }

    private void OnClickHasten()
    {
        int totalSecond = (int)(DataManager.Instance.GetTreasureData().lastTargetTime - ServerTimeManager.Instance.CurServerTime);
        if(totalSecond > 0)
            PlayerAttrUtils.UseSpeedItem(totalSecond, eAccelerationOp.eAccelerationOp_EquipBoxLevelUp, 0);
    }

    private void OnRenderListItem(int index, GObject obj)
    {
        GComponent item = (GComponent)obj;
        var ctrl = item.GetController("type");
        if (null != ctrl)
        {
            ctrl.selectedIndex = index;
        }
        var curLb = item.GetChild("curValue").asTextField;
        if (propDict.ContainsKey(index+1))
        {
            curLb.SetVar("value", propDict[index+1].ToString()).FlushVars();
        }
        else
        {
            curLb.SetVar("value", "0").FlushVars();
        }

        var nextLb =item.GetChild("nextValue").asTextField;
        if(nextPropDict.ContainsKey(index+1))
        {
            nextLb.SetVar("value", nextPropDict[index+1].ToString()).FlushVars();
        }
        else
        {
            nextLb.SetVar("value","0").FlushVars();
        }
    }

    private void OnTreasureChesLevelUpRes(int msgCode, string msgData)
    {
        if (IsShow() && IsOnStage())
        {
            JsonObject res = (JsonObject)SimpleJson.DeserializeObject(msgData);
            if (null == res)
                return;

            switch (msgCode)
            {
                case MsgCode.SUCCESS:
                {
                    TreasureChesManager.Instance.StartTreasureCd();
                    if (TreasureChesManager.Instance.TreasureBoxOpenStatus == TreasureBoxOpenStatus.ProgressBarFull)
                    {
                        TreasureChesManager.Instance.TreasureBoxOpenStatus = TreasureBoxOpenStatus.LevelUp;
                    }

                    OnRoleUpdate();
                    UpdateTreasureProgress();
                }
                    break;
                case MsgCode.FAIL:
                {
                }
                    break;
            }
        }
    }

    private void UpdateUIInfo()
    {
        if (!IsShow() || !IsOnStage()) { return; }

        //引导如果已经升级到2级，就认为加速过程跳过
        int currLv = DataManager.Instance.GetTreasureData().id;
        if(currLv == 2)
        {
            //强制设为完成
            int gid = (int)GuideID.NewAccount_MPSpeed;
            if (!GuideManager.Instance.GuideIsComplete(gid))
                GuideManager.Instance.SendToCompleteGuide(gid);

            gid = (int)GuideID.NewAccount_MPSpeedDiam;
            if (!GuideManager.Instance.GuideIsComplete(gid))
                GuideManager.Instance.SendToCompleteGuide(gid);

            //引导关闭界面
            GuideManager.Instance.StarGuideByData(new GuideData()
            {
                bid = GuideID.NewAccount_MPUpLevel1,
                gid = GuideID.NewAccount_MPClose,
                tui = this.levelUpUI.panel.closeBtn,
                isForce = true,
                isSend = true
            });
        }
        var cfgCurLv = ConfigUtils.GetTreasureChestUnitById(currLv);
        if (null != cfgCurLv)
        {
            this.levelUpUI.panel.txtCurrLv.SetVar("value", currLv.ToString()).FlushVars();
            this.levelUpUI.panel.txtNextLv.SetVar("value", cfgCurLv.NextID.ToString()).FlushVars();
        }

        int maxLv = TreasureChesManager.Instance.GetTreasureMaxLv();
        if (currLv >= maxLv)
        {
            this.type = 2;
            this.levelUpUI.panel.txtNextLv.visible = false;
            this.levelUpUI.panel.type.SetSelectedIndexEx(this.type);
        }

        // ((UI_BtnCurrency)(this.levelUpUI.panel.btnCurrency1)).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().gold);
        ((UI_CurrencyAdd)(this.levelUpUI.panel.btnCurrency)).value.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().gold);
        // ((UI_BtnCurrency)(this.levelUpUI.panel.btnCurrency2)).txtValue.text = ItemInfoManager.Instance.GetItemCount(ConstDefine.Item_SpeedCardId).ToString();
        
        treasureChestUnit = ConfigUtils.GetTreasureChestUnitById(DataManager.Instance.GetTreasureData().id);
        if (treasureChestUnit != null)
        {
            propDict = EquipManager.Instance.GetProbabilityDict(treasureChestUnit.QualityProShow);
            ConfigTreasureChestUnit next = ConfigUtils.GetTreasureChestUnitById(treasureChestUnit.Id + 1);
            if (next == null)//当已经达到最大了
                next = treasureChestUnit;
            nextPropDict = EquipManager.Instance.GetProbabilityDict(next.QualityProShow);
            this.levelUpUI.panel.listAttrs.numItems = 7;
            // this._itemWitdh = this.levelUpUI.panel.treasureNumPro.width / treasureChestUnit.GoldCoinsNum - 2f;
            this._itemWitdh = (this.levelUpUI.panel.treasureNumPro.width - (treasureChestUnit.GoldCoinsNum - 1)*5f) / treasureChestUnit.GoldCoinsNum;
            this.levelUpUI.panel.proBg.width = this.levelUpUI.panel.treasureNumPro.width + 10f;
            this.levelUpUI.panel.treasureNumPro.numItems = treasureChestUnit.GoldCoinsNum;
            
            this.levelUpUI.panel.btnBuy.money.text = StringUtils.FormatCurrency(double.Parse(treasureChestUnit.GoldCoins));

            // this.levelUpUI.panel.redDot.visible = DataManager.Instance.GetRoleData().gold >= double.Parse(treasureChestUnit.GoldCoins);
        }

        if (TreasureChesManager.Instance.TreasureBoxOpenStatus != TreasureBoxOpenStatus.ProgressBarFull)
        {
            double curGold = double.Parse(ConfigUtils.GetTreasureChestUnitById(DataManager.Instance.GetTreasureData().id).GoldCoins);
            this.levelUpUI.panel.btnBuy.grayed = !(DataManager.Instance.GetRoleData().gold > curGold);
        }

        var data = TreasureChesManager.Instance.CheckTreasureChes();
        if (!this.levelUpUI.panel.btnUpgrade.grayed)
        {
            this.levelUpUI.panel.btnUpgrade.grayed = data.Item1 != 0;
            // this.levelUpUI.panel.lvRedDot.visible = data.Item1 == 0;
        }
        
        int adTime = AdManager.Instance.GetAdFreeTimes((int) ePlayerAttrID.ePlayerAttrID_BoxAccFreeTimes);
        this.levelUpUI.panel.adLb.SetVar("cur",adTime.ToString()).SetVar("total", _common213.Param2).FlushVars();
    }

    private void ItemTreasureNumBarListItem(int index, GObject item)
    {
        item.width = this._itemWitdh;
        ((GProgressBar) item).value = (TreasureChesManager.Instance.TreasureBoxCurNum > index) ? 100 : 0;
    }

    private void OnTreasureProgressUpSucc(bool isLevelUp)
    {
        if(!IsShow() || !IsOnStage()) { return; }
        ChkGuide();
        if(isLevelUp)
            PlaySpine();
        this.levelUpUI.panel.treasureNumPro.numItems = treasureChestUnit.GoldCoinsNum;
        UpdateTreasureProgress();
        this.UpdateUIInfo();
    }

    private void OnClickGold()
    {
        Utils.OpenBuyGoldView();
    }

    private void OnClickSpeedItem()
    {
        Utils.OpenBuySpeedTicketView();
    }
}