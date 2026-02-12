using System;
using System.Collections.Generic;
using System.Linq;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using Equip;
using FairyGUI;
using msg;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

public class InheritRecycleView : UIViewBase
{
    private UI_InheritRecycle inheritRecycle => this.main as UI_InheritRecycle;

    private List<ConfigLoreEntryQualityUnit> qualityConfigList;
    private List<EquipData> _noWearLoreEquips = new List<EquipData>();
    private List<eLoreEquipQuality> _qualityList = new List<eLoreEquipQuality>();
    private Dictionary<int,double> _reGoldDict = new Dictionary<int, double>();
    private ConfigCommonUnit _common200017;
    private List<ItemData> _itemDatas = new List<ItemData>();
    private int goldNum = 0;
    
    public InheritRecycleView()
    {
        this.package = "Equip";
        this.name = "InheritRecycle";
        this.component = "InheritRecycle";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        EquipBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        _common200017 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(200017);
        RecycleGoldHandler();
        _noWearLoreEquips = EquipManager.Instance.GetNoWearLoreEquip();
        // this.inheritRecycle.closeBtn.onClick.Add(()=>{this.Hide();});
        this.inheritRecycle.closeBtn.onClick.Add(()=>{this.HideWithSoundEffect();});
        this.inheritRecycle.recycleBtn.onClick.Add(this.OnClickRecycleButton);
        this.inheritRecycle.list.itemRenderer = QualityListRender;
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_LOREEQUIP_UPDATE,UpdateNoWearLoreEquips);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_LOREEQUIP_UPDATE,UpdateNoWearLoreEquips);
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
    }
    
    protected override void OnShow()
    {
        base.OnShow();
        qualityConfigList = ConfigUtils.GetLoreEntryQualityUnitsList();

        RecycleGoldHandler();
        _reGoldDict.Clear();
        _qualityList.Clear();
        ((UI_TabCom) this.inheritRecycle.tabCom).icon.url = UIResource.GetItemUrl(2000.ToString());
        ((UI_TabCom)this.inheritRecycle.tabCom).num1.text = "0";
        
        UpdateNoWearLoreEquips();
        // this.inheritRecycle.list.numItems = qualityConfigList.Count;
        // UpdateNoWearLoreEquips();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    private void UpdateNoWearLoreEquips()
    {
        _noWearLoreEquips = EquipManager.Instance.GetNoWearLoreEquip();
        this.inheritRecycle.status.selectedIndex = _noWearLoreEquips.Count == 0 ? 1 : 0;
        this.inheritRecycle.recycleBtn.data = _qualityList;
        this.inheritRecycle.list.numItems = qualityConfigList.Count;
    }

    private void RecycleGoldHandler()
    {
        string[] reGolds = _common200017.Param1.Split('|');
        foreach (var reGold in reGolds)
        {
            string[] s = reGold.Split(',');
            ItemData item = new ItemData();
            item.id = int.Parse(s[0]);
            item.count = double.Parse(s[1]);
            _itemDatas.Add(item);
        }
    }
    
    private void QualityListRender(int index, GObject cell)
    {
        UI_InheritReItem item = (UI_InheritReItem)cell;
        item.quality.selectedIndex = qualityConfigList[index].Quality-1;
        item.num.visible = false;
        item.isCheck.selectedIndex = 0;
        item.btnCheck.data = index;
        item.btnCheck.onClick.Add(OnClickCheckBox);
        
        int qualityType = qualityConfigList[index].Quality;
        
        if (_qualityList.Count > 0)
        {
            bool isContained = _qualityList.Contains((eLoreEquipQuality)qualityType);
            if (isContained && item.isCheck.selectedIndex == 0)
            {
                item.isCheck.selectedIndex = 1;
                item.num.visible = true;
            }

            if (!isContained && item.isCheck.selectedIndex == 1)
            {
                item.isCheck.selectedIndex = 0;
                item.num.visible = false;
            }
        }
        
        // 初始化当前品质的金币值为0
        if (!_reGoldDict.ContainsKey(qualityType))
        {
            _reGoldDict[qualityType] = 0;
        }

        int equipNum = 0;//不同品质的装备的数量
        double gold = 0;
        foreach (var equipData in _noWearLoreEquips)
        {
            if (equipData.quality == qualityType)
            {
                equipNum++;
                
                if (equipData.quality <= 3)
                {
                    gold += equipData.recallGold * _itemDatas[0].count * ConstDefine.CONFIG_PLACE_EX;
                    _reGoldDict[equipData.quality]  = gold;
                }
                else
                {
                    gold += equipData.recallGold * _itemDatas[equipData.quality -3].count * ConstDefine.CONFIG_PLACE_EX;
                    _reGoldDict[equipData.quality]  = gold;
                }
            }
        }
        item.num.SetVar("value", equipNum.ToString()).FlushVars();
        SetGoldNum();
    }

    private void OnClickCheckBox(EventContext context)
    {
        int index = (int)((GButton)context.sender).data;
        UI_InheritReItem item = (UI_InheritReItem)this.inheritRecycle.list.GetChildAt(index);
        if (item.isCheck.selectedIndex == 0)
        {
            item.isCheck.selectedIndex = 1;
            item.num.visible = true;
            // _qualityList.Add((eLoreEquipQuality)index + 1);
            _qualityList.Add((eLoreEquipQuality)index + 2);
        }
        else
        {
            item.isCheck.selectedIndex = 0;
            item.num.visible = false;
            // _qualityList.Remove((eLoreEquipQuality)index + 1);
            _qualityList.Remove((eLoreEquipQuality)index + 2);
        }
        SetGoldNum();
        // _qualityList.Add((eLoreEquipQuality)index + 1);
    }
    
    private void SetGoldNum()
    {
        double selectNum = 0;
        GObject[] objList = this.inheritRecycle.list.GetChildren();
        for (int i = 0; i < objList.Length; i++)
        {
            UI_InheritReItem obj = objList[i] as UI_InheritReItem;
            if (obj.isCheck.selectedIndex == 1)
            {
                // selectNum += _reGoldDict[i + 1];
                int qualityType = qualityConfigList[i].Quality;
                _reGoldDict.TryGetValue(qualityType, out double goldValue);
                selectNum += goldValue;
                // selectNum++;
            }
        }
        
        goldNum = (int)Math.Ceiling(selectNum);//向上取整
        ((UI_TabCom)this.inheritRecycle.tabCom).num1.text = StringUtils.FormatCurrency(goldNum);
    }
    
    private void OnClickRecycleButton()
    {
        Debug.Log("==点击回收按钮==");
        if(goldNum == 0)
            return;
        
        this.Hide();
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GoldPickupSE);
        var builder = BatchSellLoreEquip_CS.CreateBuilder();
        foreach (var quality in _qualityList)
        {
            builder.AddEquipQuality(quality);
        }
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_BatchSellLoreEquip_CS, builder.Build());
        // UIManager.Instance.ToastByKey(8045);
        UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8045, goldNum));
        
        // 清空缓存数据和选择状态
        _reGoldDict.Clear();
        _qualityList.Clear();
        goldNum = 0;
        // 刷新未被装备的装备列表
        UpdateNoWearLoreEquips();
        // 重置所有品质项的复选框状态
        GObject[] children = this.inheritRecycle.list.GetChildren();
        foreach (UI_InheritReItem item in children)
        {
            if (item != null)
            {
                item.isCheck.selectedIndex = 0;
                item.num.visible = false;
            }
        }
        // 重新设置列表数据并刷新显示
        this.inheritRecycle.list.numItems = qualityConfigList.Count;
        ((UI_TabCom)this.inheritRecycle.tabCom).num1.text = StringUtils.FormatCurrency(0);

    }
}
