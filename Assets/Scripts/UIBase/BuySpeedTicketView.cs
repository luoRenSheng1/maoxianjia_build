using Common;
using CommonEx;
using Config;
using Engine;
using msg;

public class BuySpeedTicketView : UIViewBase
{
    private UI_BuySpeedTicket SpeedItem => this.main as UI_BuySpeedTicket;
    
    private int _useCount = 1;
    private int _maxCnt = 0;
    public BuySpeedTicketView()
    {
        this.name = "BuySpeedTicket";
        this.package = "Common";
        this.component = "BuySpeedTicket";
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
        this.SpeedItem.useBtn.onClick.Add(this.OnClickToBuy);
        this.SpeedItem.addBtn.onClick.Add(this.OnClickAddBtn);
        this.SpeedItem.reduceBtn.onClick.Add(this.OnClickReduceBtn);
        this.SpeedItem.cntSlider.onChanged.Add(this.OnCntSliderChange);
        this.SpeedItem.cntSlider.changeOnClick = false;
    }

    protected override void OnShow()
    {
        base.OnShow();
        _useCount = 1;
        _maxCnt = 999;
        this.SpeedItem.cntSlider.max = _maxCnt;
        this.SpeedItem.cntSlider.min = 1;
        this.SpeedItem.cntSlider.value = _useCount;
        UpdateCntTxt();
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(ConstDefine.Item_SpeedCardId);
        this.SpeedItem.purposeLb.text = itemTypeUnit.Name;
        this.SpeedItem.desc.text = itemTypeUnit.Desc;
        ((UI_ItemCom) this.SpeedItem.item).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
        ((UI_ItemCom) this.SpeedItem.item).hasCount.selectedIndex = 1;
        ((UI_ItemCom) this.SpeedItem.item).txtLv.text = ItemInfoManager.Instance.GetItemCount(itemTypeUnit.Id).ToString();
        ((UI_ItemCom) this.SpeedItem.item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
        this.SpeedItem.itemName.text = itemTypeUnit.Name;
        ((UI_qualityLabel) this.SpeedItem.itemName).qualityCtrl.selectedIndex = itemTypeUnit.Quality - 1;
    }
    
    private void OnCntSliderChange()
    {
        int cur = (int)this.SpeedItem.cntSlider.value;
        _useCount = cur;
        UpdateCntTxt();
    }
    
    private void OnClickAddBtn()
    {
        if (_maxCnt > _useCount)
        {
            _useCount++;
            this.SpeedItem.cntSlider.value = _useCount;
            UpdateCntTxt();
        }
    }
    
    private void OnClickReduceBtn()
    {
        if (_useCount > 1)
        {
            _useCount--;
            this.SpeedItem.cntSlider.value = _useCount;
            UpdateCntTxt();
        }
    }
    
    private void UpdateCntTxt()
    {
        this.SpeedItem.cntLb.text = _useCount.ToString();
        this.SpeedItem.costLb.SetVar("value", (_useCount * 10).ToString()).FlushVars();
        this.SpeedItem.addBtn.enabled = _useCount <_maxCnt;
        this.SpeedItem.reduceBtn.enabled = _useCount > 1;
    }

    private void OnClickToBuy()
    {
        if (DataManager.Instance.GetRoleData().dia < _useCount * 10)
        {
            UIManager.Instance.ToastByKey(10155);
            return;
        }

        var builder = DiamondExchange4AccItem_CS.CreateBuilder();
        builder.ItemCounter = _useCount;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_DiamondExchange4AccItem_CS, builder.Build());
    }
}
