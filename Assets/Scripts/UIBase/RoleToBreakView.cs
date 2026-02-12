using System.Collections.Generic;
using CommonEx;
using Config;
using Engine;
using FairyGUI;
using msg;
using RoleMain;
using Spine;
using UnityEngine;

public class RoleToBreakView : UIViewBase
{
    private UI_RoleToBreak roleUI => this.main as UI_RoleToBreak;
    
    private HeroInfo _heroInfo;
    List<ItemData> _itemDatas = new List<ItemData>();

    public RoleToBreakView()
    {
        this.name = "RoleToBreak";
        this.package = "RoleMain";
        this.component = "RoleToBreak";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        RoleMainBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _heroInfo = values[0] as HeroInfo;
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.roleUI.closeBtn.onClick.Add(this.Hide);
        this.roleUI.breakBtn.onClick.Add(this.OnClickBreakBtn);
        this.roleUI.itemList.itemRenderer = ItemListRender;
        this.roleUI.starList.itemRenderer = HeroStarListRender;
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateRoleInfo();
    }

    protected override void OnHide()
    {
        base.OnHide();
        Utils.ClearSpineModelOnFGUI(this.roleUI.spine);
    }
    
    private void UpdateRoleInfo()
    {
        string strResName = ConfigUtils.GetHeroModelPathByID(_heroInfo.HeroUnit.Id);
        Utils.SetSpineModelOnFGUI(this.roleUI.spine, strResName, 130);
        
        // this.roleUI.roleNameLb.text = _heroInfo.HeroUnit.Name;
        this.roleUI.roleNameLb.text = ConfigUtils.GetTextById(_heroInfo.HeroUnit.Name);
        ((UI_qualityLabel) this.roleUI.roleNameLb).qualityCtrl.selectedIndex = _heroInfo.HeroUnit.HeroQuality - 1;
        _itemDatas.Clear();
        ConfigHeroLevelUnit breakUnit = ConfigUtils.GetHeroLevel(_heroInfo.HeroUnit.Id, _heroInfo.Level);
        //判断是突破还是升级？ 
        if (breakUnit != null && breakUnit.Item != "0" && breakUnit.Level > _heroInfo.BreakLevel) //突破
        {
            bool isEnough = true;
            int itemId = 0;
            string[] itemStr = breakUnit.Item.Split('|');
            for (int i = 0; i < itemStr.Length; i++)
            {
                string[] itemArr = itemStr[i].Split(',');
                _itemDatas.Add(new ItemData()
                {
                    id = int.Parse(itemArr[0]),
                    count = double.Parse(itemArr[1])
                });
            }
        }

        this.roleUI.starList.numItems = _heroInfo.BreakLevelLayer > 0 ? 3 : 0;
        this.roleUI.itemList.numItems = _itemDatas.Count;

        List<ConfigHeroAttrUnit> heroAttrUnits = ConfigUtils.GetHeroBreakAttrsByHeroId(_heroInfo.HeroUnit.Id);
        ConfigHeroAttrUnit heroAttr1 = heroAttrUnits[_heroInfo.BreakLevelLayer];
        ConfigHeroAttrUnit heroAttr2 = heroAttrUnits[_heroInfo.BreakLevelLayer + 1];
        this.roleUI.curLb.SetVar("value", heroAttr1.Level.ToString()).FlushVars();
        this.roleUI.nextLb.SetVar("value", heroAttr2.Level.ToString()).FlushVars();
        this.roleUI.attrIcon.url = UIResource.GetAttrIconById(heroAttr1.AttrId.ToString());
        this.roleUI.attrName.text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(heroAttr1.AttrId).AttrName) + ":";
        // this.roleUI.attrValue.SetVar("value", StringUtils.ConvertToAttributeValue((int) EN_BUFF_ADD_TYPE.PetAtkADD, heroAttr1.Value)).FlushVars();
        string lastValue1 = EquipManager.Instance.SetAttributeValue(heroAttr1.AttrId, heroAttr1.Value, true);
        this.roleUI.attrValue.text = "+" + lastValue1;

        this.roleUI.occupationLb.url = UIResource.GetRoleOccupationImg(_heroInfo.HeroUnit.Vocation.ToString());//职业
        this.roleUI.attrLb.url = UIResource.GetRoleAttrImgImg(_heroInfo.HeroUnit.VocationAttr.ToString());//属系
    }

    private void HeroStarListRender(int index, GObject item)
    {
        var starParam = Utils.GetHeroStar(_heroInfo.BreakLevelLayer);
        ((UI_RoleStarItem) item).type.selectedIndex = starParam.Item1;
        ((UI_RoleStarItem) item).lockCtrl.selectedIndex = starParam.Item2 > index ? 0 : 1;
    }

    private void ItemListRender(int index, GObject item)
    {
        // ItemData itemData = _itemDatas[index];
        // ((UI_ItemCom) item).SetItemDataWithGuid(itemData, true);
        // string color = ItemInfoManager.Instance.GetItemCount(itemData.id) < itemData.count
        //     ?  FORNT_COLOR.New_Red
        //     : FORNT_COLOR.New_White;
        // ((UI_ItemCom) item).txtLv.text = string.Format("[color={0}]{1}[/color]", color, StringUtils.FormatCurrency(itemData.count));
        
        ItemData itemData = _itemDatas[index];
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemData.id);
        int num = ItemInfoManager.Instance.GetItemCount(itemData.id);
        ((UI_TabCom)item).type.selectedIndex = 1;
        ((UI_TabCom)item).icon.url = UIResource.GetItemUrl(itemTypeUnit.Icon);
        ((UI_TabCom)item).num2.SetVar("value",StringUtils.FormatCurrency(num)).SetVar("cost",itemData.count.ToString()).FlushVars();
        if (ItemInfoManager.Instance.GetItemCount(itemData.id) < itemData.count)
        {
            ((UI_TabCom)item).status.selectedIndex = 1;
        }
        else
        {
            ((UI_TabCom)item).status.selectedIndex = 0;
        }

        ((UI_TabCom)item).data = itemData;
        ((UI_TabCom)item).onClick.Add(OnItemTips);
    }
    
    private static void OnItemTips(EventContext context)
    {
        ItemData itemData = (ItemData)((UI_TabCom)context.sender).data;
        if (itemData != null)
        {
            TipsManger.Instance.ShowPopupTip((UI_TabCom)context.sender, Tipstype.None, itemData.id, itemData.ItemGuid);
        }
    }

    private void OnClickBreakBtn()
    {
        ConfigHeroLevelUnit breakUnit = ConfigUtils.GetHeroLevel(_heroInfo.HeroUnit.Id, _heroInfo.Level);
        //判断是突破还是升级？ 
        if (breakUnit != null && breakUnit.Item != "0" && breakUnit.Level > _heroInfo.BreakLevel) //突破
        {
            bool isEnough = true;
            int itemId = 0;
            string[] itemStr = breakUnit.Item.Split('|');
            for (int i = 0; i < itemStr.Length; i++)
            {
                string[] itemArr = itemStr[i].Split(',');
                if (ItemInfoManager.Instance.GetItemCount(int.Parse(itemArr[0])) < int.Parse(itemArr[1]))
                {
                    isEnough = false;
                    itemId = int.Parse(itemArr[0]);
                    break;
                }
            }

            if (!isEnough)
            {
                //UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(10180, ConfigUtils.GetConfigItemTypeUnitById(itemId).Name));
                SetVisible(false);
                // UIManager.Instance.ShowUIPanel("BuryGiftPack", Gift_Bury.Gift_yxtp);

                if (ConfigUtils.GetConfigItemTypeUnitById(itemId).Type == 7)//角色碎片
                {
                    UIManager.Instance.ShowUIPanel("SummonHero");
                    return;
                }
                
                
                LimitPackVo limitPackVo = ShopInfoManager.Instance.GetPackNum(5004);
                if (limitPackVo.BuyCounter >= ConfigUtils.GetGiftUnitsById(5004).BuyNumber)
                {
                    // UIManager.Instance.ToastByKey(5120);
                    UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(10180, ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(itemId).Name)));
                }
                else
                {
                    // UIManager.Instance.ShowUIPanel("BuryGiftPack", Gift_Bury.Gift_yxtp);// 关闭商业化
                }
                
                return;
            }

            var builder = HeroBreak_CS.CreateBuilder();
            builder.HeroId = (uint) _heroInfo.HeroUnit.Id;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_HeroBreak_CS, builder.Build());
            SetVisible(false);
        }
    }

}
