using System.Collections.Generic;
using Artifact;
using CommonEx;
using Config;
using Engine;
using FairyGUI;
using msg;

public class ArtifactAttrTipView : UIViewBase
{
    private UI_ArtifactAttrTip  artifactAttrTip => this.main as UI_ArtifactAttrTip;
    
    private ConfigArtifactEquipUnit _artifactEquipUnit;
    private List<ItemData> _attrList = new List<ItemData>();
    private EquipData equipData = new EquipData();

    public ArtifactAttrTipView()
    {
        this.name = "ArtifactAttrTip";
        this.package = "Artifact";
        this.component = "ArtifactAttrTip";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        ArtifactBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        if (values[0] != null)
        {
            _artifactEquipUnit = (ConfigArtifactEquipUnit)values[0];
        }
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.artifactAttrTip.attrList.itemRenderer = AttrListRender;
    }

    protected override void OnDispose()
    {
        base.OnDispose();
    }

    protected override void OnShow()
    {
        UpdateArtifactAttrTip();
    }

    private void UpdateArtifactAttrTip()
    {
        this.artifactAttrTip.equipBtn.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(_artifactEquipUnit.ItemId).Icon);
        ((UI_BtnEquip)this.artifactAttrTip.equipBtn).qualityIcon.visible = true;
        int x = ConfigUtils.GetConfigItemTypeUnitById(_artifactEquipUnit.ItemId).Quality;
        ((UI_BtnEquip)this.artifactAttrTip.equipBtn).ctrQuality.selectedIndex = ConfigUtils.GetConfigItemTypeUnitById(_artifactEquipUnit.ItemId).Quality - 1;
        // ((UI_BtnEquip)this.artifactAttrTip.equipBtn).hasCnt.selectedIndex = 1;
        // ((UI_BtnEquip)this.artifactAttrTip.equipBtn).lvLb.SetVar("value", _artifactEquipUnit.ArtifactLevel.ToString()).FlushVars();
        this.artifactAttrTip.equipLv.SetVar("value", _artifactEquipUnit.ArtifactLevel.ToString()).FlushVars();
        this.artifactAttrTip.name.text = ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(_artifactEquipUnit.ItemId).Name);
        
        //计算战力
        List<int> lstAttrsID = new List<int>();//属性id列表
        List<double> lstAttrsValue = new List<double>();//属性值列表
        lstAttrsID = AttrsIdListHandle(_artifactEquipUnit);
        lstAttrsValue = AttrsValueListHandle(_artifactEquipUnit);
        EquipData equipData = new EquipData();
        equipData.lstAttrsID = lstAttrsID;
        equipData.lstAttrsValue = lstAttrsValue;
        double fight = FightUtils.GetOneEquipFight(equipData, equipData, true);
        this.artifactAttrTip.value.text = StringUtils.FormatCurrency(fight);
        
        _attrList = AttrsHandle(_artifactEquipUnit);
        this.artifactAttrTip.attrList.numItems = _attrList.Count;
        this.artifactAttrTip.attrList.ResizeToFit();
    }

    private void AttrListRender(int index, GObject item)
    {
        ItemData itemData = _attrList[index];
        ((UI_ArtifactAttrItem3)item).attrName.text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(itemData.id).AttrName);

        string lastAttrValue = EquipManager.Instance.SetAttributeValue(itemData.id ,itemData.count, true);
        ((UI_ArtifactAttrItem3)item).attrValue.text = lastAttrValue;
    }
    
    private List<ItemData> AttrsHandle(ConfigArtifactEquipUnit unit)
    {
        List<ItemData> attrList = new List<ItemData>();
        string[] attrs = unit.Attr.Split("|");
        foreach (var attr in attrs)
        {
            string[] s = attr.Split(',');
            ItemData itemData = new ItemData();
            itemData.id = int.Parse(s[0]);
            itemData.count = double.Parse(s[1]);
            
            attrList.Add(itemData);
        }
        return attrList;
    }

    private List<int> AttrsIdListHandle(ConfigArtifactEquipUnit unit)
    {
        List<int> attrIdList = new List<int>();
        string[] attrs = unit.Attr.Split("|");
        foreach (var attr in attrs)
        {
            string[] s = attr.Split(',');
            attrIdList.Add(int.Parse(s[0]));
        }
        return attrIdList;
    }

    private List<double> AttrsValueListHandle(ConfigArtifactEquipUnit unit)
    {
        List<double> attrValueList = new List<double>();
        string[] attrs = unit.Attr.Split("|");
        foreach (var attr in attrs)
        {
            string[] s = attr.Split(',');
            int name = int.Parse(s[0]);
            double value = double.Parse(s[1]);

            eBattleAttr attrIdType = (eBattleAttr)name;
            if (attrIdType == eBattleAttr.eBattleAttr_PhysicAttack  // 物理伤害
                || attrIdType == eBattleAttr.eBattleAttr_MagicAttack  // 魔法伤害
                || attrIdType == eBattleAttr.eBattleAttr_SorceryAttack  // 道术伤害
                || attrIdType == eBattleAttr.eBattleAttr_HP // 生命
                || attrIdType == eBattleAttr.eBattleAttr_PhysicDefence  // 物理防御
                || attrIdType == eBattleAttr.eBattleAttr_MagicDefence  // 魔法防御
                || attrIdType == eBattleAttr.eBattleAttr_SorceryDefence  // 道术防御
                || attrIdType == eBattleAttr.eBattleAttr_HP_Recovery // 生命恢复
                || attrIdType == eBattleAttr.eBattleAttr_PetAtk  // //附加的宠物伤害（角色各功能加给宠物身上生效的伤害）
                || attrIdType == eBattleAttr.eBattleAttr_Parry_Value // 格挡值
                || attrIdType == eBattleAttr.eBattleAttr_Hurt_HPRecovery // 普攻回复的生命具体值
                || attrIdType == eBattleAttr.eBattleAttr_SkillAtkMultiple  // 技能伤害次数(倍数)
                || attrIdType == eBattleAttr.eBattleAttr_OnlineStageAwaardMultiple  // 关卡挂机奖励次数
                || attrIdType == eBattleAttr.eBattleAttr_HomeTownProduce_ExtraLimit  // 家园挂机时间上限
                || attrIdType == eBattleAttr.eBattleAttr_KilledRecovery  // 消灭对象后HP回复
                || attrIdType == eBattleAttr.eBattleAttr_FinalAttack // 攻击力
                || attrIdType == eBattleAttr.eBattleAttr_FinalDefence) // 防御
            {
                attrValueList.Add(value);
            }
            else
            {
                attrValueList.Add(value/10000);
            }
            
        }
        return attrValueList;
    }


}