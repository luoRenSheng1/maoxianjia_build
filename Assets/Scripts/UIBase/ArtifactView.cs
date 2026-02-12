using System.Collections.Generic;
using Artifact;
using Common;
using CommonEx;
using Config;
using Engine;
using FairyGUI;
using msg;
using EventDispatcher = EngineBase.EventDispatcher;


public class ArtifactView : UIViewBase
{ 
    private UI_ArtifactMain Artifact => this.main as UI_ArtifactMain;
    
    private Dictionary<int,int> _artifactDict = new Dictionary<int, int>();//服务器下发
    private ConfigArtifactEquipUnit curUnit;
    private List<ItemData> curAttrList = new List<ItemData>();//当前等级属性
    private List<ItemData> nextAttrList = new List<ItemData>();//下一等级属性
    private List<ItemData> maxAttrList = new List<ItemData>();//最大等级属性
    
    private List<ConfigArtifactEquipUnit> _artifactEquipUnitList = new List<ConfigArtifactEquipUnit>();
    
    public ArtifactView()
    { 
        this.name = "Artifact";
        this.package = "Artifact";
        this.component = "ArtifactMain";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        ArtifactBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.Artifact.closeBtn.onClick.Add(this.Hide);
        this.Artifact.closeBtn.onClick.Add(this.HideUIAndSpine);
        this.Artifact.tabList.onClickItem.Add(OnClickArtifactTypeItem);
        this.Artifact.tabList.selectedIndex = 0;
        this.Artifact.upLvBtn.onClick.Add(this.OnClickUpLv);
        this.Artifact.attrList.itemRenderer = AttrListRender;
        this.Artifact.maxList.itemRenderer = MaxAttrListRender;
        
        this.Artifact.tabList.itemRenderer = TabListRender;
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ARTIFACT_UPDATE, this.UpdateArtifactInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ARTIFACT_UPDATE, this.RedPointHandler);//EVENT_ROLE_UPDATE
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ARTIFACT_UPDATE, this.UpdateArtifactInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ARTIFACT_UPDATE, this.RedPointHandler);
    }

    protected override void OnShow()
    {
        ChangeArtifactIndex(0);
        UpdateArtifactInfo();
        RedPointHandler();
    }

    private void HideUIAndSpine()
    {
        this.Hide();
        this.Artifact.itemBtn2.spineEff.visible = false;
        this.Artifact.itemBtn2.icon1.visible = true;
    }

    private void OnClickArtifactTypeItem(EventContext context)
    {
        GButton item = context.data as GButton;
        var index = this.Artifact.tabList.GetChildIndex(item);
        this.Artifact.tabList.selectedIndex = index;
        ChangeArtifactIndex(index);
        UpdateArtifactInfo();
        
        this.Artifact.itemBtn2.spineEff.visible = false;
        this.Artifact.itemBtn2.icon1.visible = true;
    }

    private void ChangeArtifactIndex(int index)
    {
        switch (index)
        {
            case 0://盾牌
                _artifactDict = EquipManager.Instance.GetArtifactDict();
                curUnit = ConfigUtils.GetArtifactEquipUnitByTypeAndLv((int)ArtifactType.SHIELD,_artifactDict[(int)ArtifactType.SHIELD]);
                break;
            case 1://勋章
                _artifactDict = EquipManager.Instance.GetArtifactDict();
                curUnit = ConfigUtils.GetArtifactEquipUnitByTypeAndLv((int)ArtifactType.MEDAL,_artifactDict[(int)ArtifactType.MEDAL]);
                break;
            case 2://符石
                _artifactDict = EquipManager.Instance.GetArtifactDict();
                curUnit = ConfigUtils.GetArtifactEquipUnitByTypeAndLv((int)ArtifactType.STONE,_artifactDict[(int)ArtifactType.STONE]);
                break;
            case 3://职印
                _artifactDict = EquipManager.Instance.GetArtifactDict();
                curUnit = ConfigUtils.GetArtifactEquipUnitByTypeAndLv((int)ArtifactType.JOB,_artifactDict[(int)ArtifactType.JOB]);
                break;
        }
    }

    private void UpdateArtifactInfo()
    {
        _artifactDict = EquipManager.Instance.GetArtifactDict();
        ChangeArtifactIndex(Artifact.tabList.selectedIndex);
        
        // this.Artifact.itemBtn.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(curUnit.ItemId)
        // ((UI_BtnEquip)this.Artifact.itemBtn).qualityIcon.visible = true;
        // ((UI_BtnEquip)this.Artifact.itemBtn).ctrQuality.selectedIndex = ConfigUtils.GetConfigItemTypeUnitById(curUnit.ItemId).Quality - 1;
        // ((UI_BtnEquip)this.Artifact.itemBtn).hasCnt.selectedIndex = 1;
        // ((UI_BtnEquip)this.Artifact.itemBtn).lvLb.SetVar("value",curUnit.ArtifactLevel.ToString()).FlushVars();
        
        this.Artifact.itemBtn2.icon1.url = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(curUnit.ItemId).Icon);
        ((UI_ArtifactItem)this.Artifact.itemBtn2).qualityIcon.visible = true;
        ((UI_ArtifactItem)this.Artifact.itemBtn2).qualityCtrl.selectedIndex = ConfigUtils.GetConfigItemTypeUnitById(curUnit.ItemId).Quality - 1;
        ((UI_ArtifactItem)this.Artifact.itemBtn2).lvLb.SetVar("value",curUnit.ArtifactLevel.ToString()).FlushVars();
        
        this.Artifact.name.text = ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(curUnit.ItemId).Name);
        this.Artifact.desc.text = ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(curUnit.ItemId).Desc);
        
        ConfigArtifactEquipUnit nextArtifactEquipUnit = ConfigUtils.GetNextArtifactEquipUnitByTypeAndLv(curUnit.Type, curUnit.ArtifactLevel);//下一等级
        int costDia;
        int costItem;
        string[] costItems;
        string costItemId;
        
        ((UI_TabCom)this.Artifact.tabCom1).type.selectedIndex = 1;
        ((UI_TabCom)this.Artifact.tabCom2).type.selectedIndex = 1;
        if (nextArtifactEquipUnit != null)//是否满级
        {
            costItems = nextArtifactEquipUnit.ItemCost.Split(',');
            costDia = nextArtifactEquipUnit.DiamondsCost;
            costItem = int.Parse(costItems[1]);
            costItemId = costItems[0];

            this.Artifact.isMax.selectedIndex = 0;
        }
        else
        {
            costDia = 0;
            costItem = 0;
            costItemId = curUnit.ItemCost.Split(',')[0];
            
            this.Artifact.isMax.selectedIndex = 1;
            this.Artifact.status.selectedIndex = 1;
        }
        ((UI_TabCom)this.Artifact.tabCom2).icon.url = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(int.Parse(costItemId)).Icon);

        ((UI_TabCom)this.Artifact.tabCom1).status.selectedIndex = (costDia <= DataManager.Instance.mRoleData.dia) ? 0 : 1;
        ((UI_TabCom)this.Artifact.tabCom2).status.selectedIndex = (costItem <= ItemInfoManager.Instance.GetItemCount(int.Parse(costItemId))) ? 0 : 1;
        this.Artifact.status.selectedIndex = ((costDia <= DataManager.Instance.mRoleData.dia) && (costItem <= ItemInfoManager.Instance.GetItemCount(int.Parse(costItemId)))) ? 0 : 2;
        
        
        if (curUnit.ArtifactLevel >= GetMaxHeroLv())//神器未满级时，升级上限取玩家所有角色中的等级最高的为上限等级
        {
            this.Artifact.status.selectedIndex = 2;
        }
        
        if (nextArtifactEquipUnit == null)
            this.Artifact.status.selectedIndex = 1;

        //消耗砖石StringUtils.FormatCurrency
        ((UI_TabCom)this.Artifact.tabCom1).num2.SetVar("value",StringUtils.FormatCurrency(DataManager.Instance.mRoleData.dia)).SetVar("cost",costDia.ToString()).FlushVars();//costDia.ToString()
        
        //消耗特殊材料
        ((UI_TabCom)this.Artifact.tabCom2).num2.SetVar("value",StringUtils.FormatCurrency(ItemInfoManager.Instance.GetItemCount(int.Parse(costItemId)))).SetVar("cost",costItem.ToString()).FlushVars();//costItem.ToString()
        
        // ((UI_BtnCurrency)this.Artifact.diaCurBtn).txtValue.text = DataManager.Instance.mRoleData.dia.ToString();
        ((UI_BtnCurrency)this.Artifact.diaCurBtn).txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().dia);
        ((UI_BtnCurrency)this.Artifact.itemCurBtn).icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(int.Parse(costItemId)).Icon);
        

        int x = ItemInfoManager.Instance.GetItemCount(int.Parse(costItemId));
        // ((UI_BtnCurrency)this.Artifact.itemCurBtn).txtValue.text = ItemInfoManager.Instance.GetItemCount(int.Parse(costItemId)).ToString();
        ((UI_BtnCurrency)this.Artifact.itemCurBtn).txtValue.text = StringUtils.FormatCurrency(ItemInfoManager.Instance.GetItemCount(int.Parse(costItemId)));
        
        curAttrList.Clear();//清理之前数据
        
        nextAttrList.Clear();//清理之前数据
        
        this.Artifact.curLv.SetVar("curLv",curUnit.ArtifactLevel.ToString()).FlushVars();
        if (nextArtifactEquipUnit != null)
        {
            curAttrList = AttrsHandle(curUnit);//当前等级属性处理
            this.Artifact.nextLv.SetVar("next",nextArtifactEquipUnit.ArtifactLevel.ToString()).FlushVars();
            nextAttrList = AttrsHandle(nextArtifactEquipUnit);//下一等级属性处理
        }
        
        this.Artifact.attrList.numItems = curAttrList.Count;
        this.Artifact.attrList.data = curAttrList;
        
        // 最高等级属性
        maxAttrList.Clear();
        ConfigArtifactEquipUnit maxUnit = ConfigUtils.GetMaxLvArtifactEquipUnitByType(curUnit.Type);
        maxAttrList = AttrsHandle(maxUnit);
        this.Artifact.maxLv.SetVar("max",maxUnit.ArtifactLevel.ToString()).FlushVars();
        this.Artifact.maxList.data = maxAttrList;
        this.Artifact.maxList.numItems = maxAttrList.Count;

        this.Artifact.upLvBtn.data = curUnit;
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

    private void AttrListRender(int index, GObject item)
    {
        ItemData curAttrsData = curAttrList[index];
        ItemData nextAttrData = nextAttrList[index];
        
        ((UI_ArtifactAttrItem)item).curAttrName.SetVar("name",
            ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(curAttrsData.id).AttrName)).FlushVars();

        string lastValue1 = EquipManager.Instance.SetAttributeValue(curAttrsData.id, curAttrsData.count, true);
        ((UI_ArtifactAttrItem)item).curAttr.text = lastValue1;
        
        string lastValue2 = EquipManager.Instance.SetAttributeValue(nextAttrData.id, nextAttrData.count, true);
        ((UI_ArtifactAttrItem)item).nextAttr.text = lastValue2;
    }

    private void MaxAttrListRender(int index, GObject item)
    {
        ItemData maxAttrsData = maxAttrList[index];
        ((UI_ArtifactAttrItem2)item).name.text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(maxAttrsData.id).AttrName);
        
        string lastValue = EquipManager.Instance.SetAttributeValue(maxAttrsData.id, maxAttrsData.count, true);
        ((UI_ArtifactAttrItem2)item).num.text = lastValue;
    }

    // 获取拥有的英雄中等级最高的英雄等级
    private int GetMaxHeroLv()
    {
        return HeroInfoManager.Instance.GetMaxHeroLv();
    }

    private void OnClickUpLv(EventContext context)
    {
        // 升级条件不符拦截
        ConfigArtifactEquipUnit unit = (context.sender as GButton).data as ConfigArtifactEquipUnit;
        ConfigArtifactEquipUnit nextUnit = ConfigUtils.GetNextArtifactEquipUnitByTypeAndLv(unit.Type, unit.ArtifactLevel);

        if (nextUnit != null)
        {
            if (DataManager.Instance.mRoleData.dia >= nextUnit.DiamondsCost)
            {
                if (ItemInfoManager.Instance.GetItemCount(int.Parse(nextUnit.ItemCost.Split(',')[0])) >= int.Parse(nextUnit.ItemCost.Split(',')[1]))
                {
                    if (unit.ArtifactLevel < GetMaxHeroLv())
                    {
                        var builder = ArtifactLevelUp_CS.CreateBuilder();
                        builder.AttrId = (ePlayerAttrID)unit.Type;
                        builder.LevelUpValues = 1;
                        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ArtifactLevelUp_CS, builder.Build());

                        if (unit.ItemId != nextUnit.ItemId)
                        {
                            // 升级成功播放spine
                            ((UI_ArtifactItem)this.Artifact.itemBtn2).icon1.visible = false;
                            this.Artifact.itemBtn2.spineEff.visible = true;
                            this.Artifact.itemBtn2.spineEff.SetScale(0.8f,0.8f);
                            Utils.PlaySpineAnim(this.Artifact.itemBtn2.spineEff, "Artifact_sqtx", false, () =>
                            {
                                this.Artifact.itemBtn2.spineEff.visible = false;
                                this.Artifact.itemBtn2.icon1.visible = true;
                            });
                            this.Artifact.itemBtn2.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(nextUnit.ItemId).Icon);
                        }
                        
                    }
                    else
                    {
                        UIManager.Instance.ToastByKey(8022);
                    }

                }
                else
                {
                    // UIManager.Instance.ToastByKey(5001);//材料不足提示
                    UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8038, ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(int.Parse(nextUnit.ItemCost.Split(',')[0])).Name)));
                }
            }
            else
            {
                UIManager.Instance.ToastByKey(5008);//砖石不足提示
            }
        }
        else
        {
            // UIManager.Instance.ToastByKey(5001);//满级提示
            UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8023, ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(unit.ItemId).Name)));//满级提示
        }
        
    }

    private void TabListRender(int index, GObject item)
    {
        var unit = _artifactEquipUnitList[index];
        var nextUnit = ConfigUtils.GetNextArtifactEquipUnitByTypeAndLv(unit.Type, unit.ArtifactLevel);

        if (nextUnit != null)//没有达到满级
        {
            string[] costItems2 = nextUnit.ItemCost.Split(',');
            int costDia2 = nextUnit.DiamondsCost;
            int costItem2 = int.Parse(costItems2[1]);
            int costItemId2 = int.Parse(costItems2[0]);
            
            //红点
            bool isConditionMet = DataManager.Instance.mRoleData.dia >= costDia2 
                                  && ItemInfoManager.Instance.GetItemCount(costItemId2) >= costItem2
                                  && unit.ArtifactLevel < GetMaxHeroLv();
            ((UI_ArtifactSelectBtn)item).redPoint.visible = isConditionMet;
        }
        else
        {
            ((UI_ArtifactSelectBtn)item).redPoint.visible = false;
        }
    }

    private void RedPointHandler()
    {
        _artifactDict = EquipManager.Instance.GetArtifactDict();
        _artifactEquipUnitList.Clear();
        foreach (var item in _artifactDict)
        {
            var curUnit = ConfigUtils.GetArtifactEquipUnitByTypeAndLv(item.Key,item.Value);
            _artifactEquipUnitList.Add(curUnit);
        }
        this.Artifact.tabList.numItems = 4;
    }


}
