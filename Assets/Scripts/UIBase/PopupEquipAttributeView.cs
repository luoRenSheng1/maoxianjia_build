using CommonEx;
using Engine;
using Equip;
using FairyGUI;

public class PopupEquipAttributeView : UIViewBase
{
    private UI_PopupEquipAttribute panel => this.main as UI_PopupEquipAttribute;
    private EquipData partEquipData = new EquipData();

    public PopupEquipAttributeView()
    {
        this.name = "PopupEquipAttribute";
        this.package = "Equip";
        this.component = "PopupEquipAttribute";
        this.removePackage = true;
        this.safeAreaInset = false;
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.panel.mainAttrList.itemRenderer = OnEquippedEquipAttrs;
        this.panel.attrList.itemRenderer = OnEquippedEntryAttrs;
    }
    
    public override void BindAll()
    {
        base.BindAll();

        UIObjectFactory.SetPackageItemExtension(UI_PopupEquipAttribute.URL, typeof(UI_PopupEquipAttribute));
    }

    protected override void OnDispose()
    {
        base.OnDispose();
    }

    protected override void OnShow()
    {
        base.OnShow();

        UpdateEquipAttrs(partEquipData);
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);

        if (values.Length >= 1)
        {
            partEquipData = (EquipData)values[0];
        }
    }

    private void UpdateEquipAttrs(EquipData _partEquipData)
    {
        partEquipData = _partEquipData;
        if (null != partEquipData && !partEquipData.IsNull())
        {
            var config = ConfigUtils.GetConfigItemTypeUnitById(partEquipData.GetSourceId());
            if (null != config)
            {
                UI_ComEquipItem comEquipItem = (UI_ComEquipItem)panel.comEquipItem2;
                comEquipItem.icon.icon = UIResource.GetItemUrl(config.Icon);//UIResource.GetPartURL(config.Parts); //config.Icon;
                // comEquipItem.txtLv.SetVar("value", partEquipDatapartEquipData = EquipData .lv.ToString()).FlushVars();
                comEquipItem.ctrlQuality.selectedIndex = partEquipData.quality - 1;
                panel.equipLv.SetVar("value", partEquipData.lv.ToString()).FlushVars();
                // panel.txtName.text = ConfigUtils.GetTextById(config.Name);
                panel.name.text = ConfigUtils.GetTextById(config.Name);
                // ((UI_qualityLabel) panel.txtName).qualityCtrl.selectedIndex = partEquipData.quality - 1;
                // panel.lisAttrs.numItems = partEquipData.GetAttrCount();
                // panel.lisEntrys.numItems = partEquipData.GetEntryCount();
                panel.mainAttrList.numItems = partEquipData.GetAttrCount();
                panel.attrList.numItems = partEquipData.GetEntryCount();
                panel.attrList.ResizeToFit();
                // panel.lisEntrys.ResizeToFit();
                this.panel.fightLb2.text = StringUtils.FormatCurrency(FightUtils.GetOneEquipFight(partEquipData, partEquipData, true));
            }
        }
    }


    private void OnEquippedEquipAttrs(int index, GObject obj)
    {
        GComponent item = (GComponent)obj;
        if (null == item)
            return;

        if (partEquipData.IsNull())
            return;

        if (index >= partEquipData.lstAttrsID.Count)
            return;

        int id = partEquipData.lstAttrsID[index];
        double value = partEquipData.lstAttrsValue[index];
        SetItemValue(item, id, value);
        // item.GetChild("txtAttrName").text = StringUtils.ConvertToAttributeName(id);
        // item.GetChild("txtAttrName").text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(id).AttrName);
        // item.GetChild("txtAttrValue").text = StringUtils.FormatCurrency(value);
        // item.GetController("status").selectedIndex = 0;
    }
    
    private void OnEquippedEntryAttrs(int index, GObject obj)
    {
        GComponent item = (GComponent)obj;
        if (null == item)
            return;

        if (partEquipData.IsNull())
            return;

        if (index >= partEquipData.lstEntrysID.Count)
            return;

        int id = partEquipData.lstEntrysID[index];
        // double value = partEquipData.lstEntrysValue[index] * 100;
        double value = partEquipData.lstEntrysValue[index];
        SetItemValue(item, id, value);
        // item.GetChild("txtAttrName").text = StringUtils.ConvertToAttributeName(id);
        // item.GetChild("txtAttrName").text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(id).AttrName);
        // item.GetChild("txtAttrValue").text = $"{StringUtils.FormatCurrency(value)}%";
        // int entryId = partEquipData.lstEntryCfgsID[index];
        // item.GetChild("desc").text = ConfigUtils.GetConfigEntryUnit(entryId).Desc;
    } 
    
    private void SetItemValue(GComponent item, int id, double value)
    {
        if (id < 1000)
        {
            // item.GetChild("txtAttrName").text = StringUtils.ConvertToAttributeName(id);
            item.GetChild("txtAttrName").text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(id).AttrName);//读取属性配置表
            item.GetChild("txtAttrValue").text = EquipManager.Instance.SetAttributeValue(id, value);//$"{StringUtils.FormatCurrency(value)}%";
            // int entryId = partEquipData.lstEntryCfgsID[index];
            // item.GetChild("desc").text = ConfigUtils.GetConfigEntryUnit(entryId).Desc; 
        }
        else  //是技能
        {
            item.GetChild("txtAttrName").text = ConfigUtils.GetTextById(ConfigUtils.GetSkillById(id).Name);//读取属性配置表
            item.GetChild("txtAttrValue").text = ConfigUtils.GetStringByKey(8046) + "+" + value;
        }
    }
}