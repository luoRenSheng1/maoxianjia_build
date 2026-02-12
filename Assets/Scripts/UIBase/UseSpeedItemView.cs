using System.Collections.Generic;
using Common;
using CommonEx;
using Config;
using Engine;
using FairyGUI;
using msg;
using UnityEngine;
using Village;
using EventDispatcher = EngineBase.EventDispatcher;

public class UseSpeedItemView : UIViewBase
{
    private UI_UseSpeedItem UseItem => this.main as UI_UseSpeedItem;
    
    private int _useCount = 1;
    private int _maxCnt = 0;

    private eAccelerationOp _eAccelerationOp;
    private int _totalTime;
    private int _buildType;
    private bool _isGuiding;
    public UseSpeedItemView()
    {
        this.name = "UseSpeedItem";
        this.package = "Common";
        this.component = "UseSpeedItem";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.FuncType = FuncType.village;
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _eAccelerationOp = (eAccelerationOp) values[0];
        _totalTime = (int) values[1];
        _buildType = (int) values[2];
    }

    protected override void OnInit()
    {
        base.OnInit();
        
        this.UseItem.useBtn.onClick.Add(this.OnClickUseBtn);
        
        this.UseItem.addBtn.onClick.Add(this.OnClickAddBtn);
        this.UseItem.reduceBtn.onClick.Add(this.OnClickReduceBtn);
        this.UseItem.cntSlider.onChanged.Add(this.OnCntSliderChange);
        this.UseItem.cntSlider.changeOnClick = false;

        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_USE_ITEM_UPDATE, this.OnItemUseSuccess);
    }
    
    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_USE_ITEM_UPDATE, this.OnItemUseSuccess);
    }

    protected override void OnHide()
    {
        base.OnHide();
        if (_isGuiding)
        {
            GuideManager.Instance.HideGuide();
            _isGuiding = false;
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        _useCount = 1;
        _maxCnt = (int)ItemInfoManager.Instance.GetItemData(ConstDefine.Item_SpeedCardId).count;
        int maxUse = Mathf.CeilToInt(_totalTime / 300f);
        _maxCnt = Mathf.Min(maxUse, _maxCnt);
        this.UseItem.cntSlider.max = _maxCnt;
        this.UseItem.cntSlider.min = _maxCnt == 1 ? 0 : 1;
        _useCount = _maxCnt;
        this.UseItem.cntSlider.value = _useCount;
        UpdateCntTxt();
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(ConstDefine.Item_SpeedCardId);
        this.UseItem.purposeLb.text = itemTypeUnit.Name;
        this.UseItem.desc.text = itemTypeUnit.Desc;
        ((UI_ItemCom) this.UseItem.item).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
        ((UI_ItemCom) this.UseItem.item).hasCount.selectedIndex = 0;
        ((UI_ItemCom) this.UseItem.item).txtLv.text = ItemInfoManager.Instance.GetItemCount(itemTypeUnit.Id).ToString();
        ((UI_ItemCom) this.UseItem.item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
        this.UseItem.itemName.text = itemTypeUnit.Name;
        ((UI_qualityLabel) this.UseItem.itemName).qualityCtrl.selectedIndex = itemTypeUnit.Quality - 1;
        _isGuiding = false;
    }

    private void OnCntSliderChange()
    {
        int cur = (int)this.UseItem.cntSlider.value;
        _useCount = cur;
        UpdateCntTxt();
    }
    
    private void OnClickAddBtn()
    {
        if (_maxCnt > _useCount)
        {
            _useCount++;
            this.UseItem.cntSlider.value = _useCount;
            UpdateCntTxt();
        }
    }
    
    private void OnClickReduceBtn()
    {
        if (_useCount > 1)
        {
            _useCount--;
            this.UseItem.cntSlider.value = _useCount;
            UpdateCntTxt();
        }
    }

    private void UpdateCntTxt()
    {
        this.UseItem.cntLb.text = _useCount.ToString();
        this.UseItem.timeLb.SetVar("value", StringUtils.GetTimeString(_useCount * 300)).FlushVars();
        this.UseItem.addBtn.enabled = _useCount <_maxCnt;
        this.UseItem.reduceBtn.enabled = _useCount > 1;
    }

    private void OnItemUseSuccess()
    {
        if (IsShow() && IsOnStage())
        {
           SetVisible(false);
        }

    }

    private void OnClickUseBtn()
    {
        var builder = ItemUse4Acceleration_CS.CreateBuilder();
        builder.IsUseDiamond = false;
        builder.Operation = _eAccelerationOp;
        var itemBuilder = ItemInfo.CreateBuilder();
        itemBuilder.Id = ConstDefine.Item_SpeedCardId;
        itemBuilder.Num = _useCount;
        builder.UsedItems = itemBuilder.Build();
        if (_eAccelerationOp == eAccelerationOp.eAccelerationOp_BuildLevelUp)
            builder.ObjectId = (uint) _buildType;
        else if (_eAccelerationOp == eAccelerationOp.eAccelerationOp_CampExploring)
            builder.ObjectId = (uint) _buildType;
        else if (_eAccelerationOp == eAccelerationOp.eAccelerationOp_FinalFactoryProducing)
            builder.ObjectId = (uint) _buildType;
        ItemUse4Acceleration_CS use4AccelerationCs = builder.Build();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ItemUse4Acceleration_CS, use4AccelerationCs);
        
        SetVisible(false);
    }

}
