
using CommonEx;
using Config;
using Engine;
using EngineBase;
using msg;
using PVPMap;
using UnityEngine;

public class BuyFightTimesView : UIViewBase
{
    private UI_BuyFightTimes BuyFightTimes => this.main as UI_BuyFightTimes;

    private ConfigCommonUnit _common15;
    private int _useCount = 1;
    private int _maxCnt = 0;
    private int _oneDiamond;
    public BuyFightTimesView()
    {
        this.name = "BuyFightTimes";
        this.package = "PVPMap";
        this.component = "BuyFightTimes";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        PVPMapBinder.BindAll();
    }
    
    protected override void OnInit()
    {
        base.OnInit();
        // this.BuyFightTimes.closeBtn.onClick.Add(this.Hide);
        this.BuyFightTimes.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.BuyFightTimes.addBtn.onClick.Add(this.OnClickAddBtn);
        this.BuyFightTimes.reduceBtn.onClick.Add(this.OnClickReduceBtn);
        this.BuyFightTimes.moneyBtn.onClick.Add(this.OnClickToBuy);
        this.BuyFightTimes.cntLb.onChanged.Add(this.OnClickCndLbChange);
        _common15 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(15);
        string[] itemArr = _common15.Param2.Split(',');
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(itemArr[0]));
        ((UI_EmptyDimandBtn) this.BuyFightTimes.moneyBtn).itemIcon.url = UIResource.GetItemUrl(itemTypeUnit.Icon);
        _oneDiamond = int.Parse(itemArr[1]);
    }

    protected override void OnShow()
    {
        base.OnShow();
        _useCount = 1;
        _maxCnt = int.Parse(_common15.Param3) - PvpRankDataManager.Instance.FightBuyCnt;
        UpdateCntTxt();
    }
    
    private void OnClickAddBtn()
    {
        if (_maxCnt > _useCount)
        {
            _useCount++;
            UpdateCntTxt();
        }
    }
    
    private void OnClickReduceBtn()
    {
        if (_useCount > 1)
        {
            _useCount--;
            UpdateCntTxt();
        }
    }
    
    private void UpdateCntTxt()
    {
        this.BuyFightTimes.cntLb.text = _useCount.ToString();
        ((UI_EmptyDimandBtn) this.BuyFightTimes.moneyBtn).diamondLb.text = StringUtils.FormatCurrency(_useCount * _oneDiamond);
        this.BuyFightTimes.addBtn.enabled = _useCount <_maxCnt;
        this.BuyFightTimes.reduceBtn.enabled = _useCount > 1;
        this.BuyFightTimes.itemNum.SetVar("value", _useCount.ToString()).FlushVars();
    }

    private void OnClickToBuy()
    {
        if (DataManager.Instance.GetRoleData().dia < _useCount * _oneDiamond)
        {
            UIManager.Instance.ToastByKey(10155);
            return;
        }

        var builder = GlobalBuyChallengeCounter_CS.CreateBuilder();
        builder.PurchaseCounter = _useCount;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_GlobalBuyChallengeCounter_CS, builder.Build());
    }

    private void OnClickCndLbChange()
    {
        string text = this.BuyFightTimes.cntLb.text;
        if (!string.IsNullOrEmpty(text) && int.Parse(text) > 1)
        {
            _useCount = Mathf.Min(_maxCnt, int.Parse(text));
            UpdateCntTxt();
        }
    }
}
