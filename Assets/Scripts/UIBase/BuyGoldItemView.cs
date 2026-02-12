
using Common;
using CommonEx;
using Config;
using Engine;
using msg;

public class BuyGoldItemView : UIViewBase
{
    private UI_BuyGoldItem BuyGoldItem => this.main as UI_BuyGoldItem;
    
    private int _useCount = 1;
    private int _maxCnt = 0;
    public BuyGoldItemView()
    {
        this.name = "BuyGoldItem";
        this.package = "Common";
        this.component = "BuyGoldItem";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.BuyGoldItem.useBtn.onClick.Add(this.OnClickToBuy);
        this.BuyGoldItem.addBtn.onClick.Add(this.OnClickAddBtn);
        this.BuyGoldItem.reduceBtn.onClick.Add(this.OnClickReduceBtn);
        this.BuyGoldItem.cntSlider.onChanged.Add(this.OnCntSliderChange);
        this.BuyGoldItem.cntSlider.changeOnClick = false;
    }

    protected override void OnShow()
    {
        base.OnShow();
        _useCount = 1;
        _maxCnt = 999;
        this.BuyGoldItem.cntSlider.max = _maxCnt;
        this.BuyGoldItem.cntSlider.min = 1;
        this.BuyGoldItem.cntSlider.value = _useCount;
        UpdateCntTxt();
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(ConstDefine.Item_GoldId);
        // this.BuyGoldItem.purposeLb.text = itemTypeUnit.Name;
        // this.BuyGoldItem.desc.text = itemTypeUnit.Desc;
        ((UI_ItemCom) this.BuyGoldItem.item).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
        ((UI_ItemCom) this.BuyGoldItem.item).hasCount.selectedIndex = 1;
        ((UI_ItemCom) this.BuyGoldItem.item).txtLv.text = ItemInfoManager.Instance.GetItemCount(itemTypeUnit.Id).ToString();
        ((UI_ItemCom) this.BuyGoldItem.item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
        this.BuyGoldItem.itemName.text = itemTypeUnit.Name;
        ((UI_qualityLabel) this.BuyGoldItem.itemName).qualityCtrl.selectedIndex = itemTypeUnit.Quality - 1;
    }
    
    private void OnCntSliderChange()
    {
        int cur = (int)this.BuyGoldItem.cntSlider.value;
        _useCount = cur;
        UpdateCntTxt();
    }
    
    private void OnClickAddBtn()
    {
        if (_maxCnt > _useCount)
        {
            _useCount++;
            this.BuyGoldItem.cntSlider.value = _useCount;
            UpdateCntTxt();
        }
    }
    
    private void OnClickReduceBtn()
    {
        if (_useCount > 1)
        {
            _useCount--;
            this.BuyGoldItem.cntSlider.value = _useCount;
            UpdateCntTxt();
        }
    }
    
    private void UpdateCntTxt()
    {
        this.BuyGoldItem.cntLb.text = _useCount.ToString();
        this.BuyGoldItem.costLb.SetVar("value", (_useCount * 10).ToString()).FlushVars();
        this.BuyGoldItem.addBtn.enabled = _useCount <_maxCnt;
        this.BuyGoldItem.reduceBtn.enabled = _useCount > 1;

        this.BuyGoldItem.getLb.text = StringUtils.FormatCurrency(_useCount * 100000);
    }

    private void OnClickToBuy()
    {
        if (DataManager.Instance.GetRoleData().dia < _useCount * 10)
        {
            UIManager.Instance.ToastByKey(10155);
            return;
        }

        var builder = DiamondExchange4Gold_CS.CreateBuilder();
        builder.Diamonds = _useCount * 10;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_DiamondExchange4Gold_CS, builder.Build());
    }
}
