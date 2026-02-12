using System.Collections.Generic;
using System.Text;
using Common;
using CommonEx;
using Config;
using Engine;
using Equip;
using FairyGUI;
using msg;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

public class EquipView : UIViewBase
{
    private UI_Main equipUI => this.main as UI_Main;
    private ulong currEquipGuid = 0;
    private bool isNew = false;
    private int goldFlyType = 0;//普通装备分解飞金币效果：1为大地图，2为装备界面
    private StringBuilder _stringBuilder = new StringBuilder();

    private bool _isGuiding;
    private bool _isReceive = true;
    private bool _flag = false;
    public EquipView()
    {
        this.name = "Equip";
        this.package = "Equip";
        this.component = "Main";
        this.removePackage = true;
        // this.type = UIType.Normal;
        this.type = UIType.Tip;
        this.GuideType = FuncType.Guide;
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.equipUI.frame.onClick.Set(OnClickClose);
        this.equipUI.uiType.onChanged.Set(OnChangedUIType);
        
        this.equipUI.uiEquipRecommand.btnEquip2.onClick.Set(OnClickNewEquip);

        // 修改
        this.equipUI.uiEquipRecommand.mainAttrList2.itemRenderer = OnRenderEquipAttrs;//主属性
        this.equipUI.uiEquipRecommand.attrsList2.itemRenderer = OnRenderEntryAttrs;//词条属性
        
        this.equipUI.uiEquipObtain.resolveBtn4.onClick.Set(OnClickDecomEquip);//分解
        this.equipUI.uiEquipObtain.repalceBtn4.onClick.Set(OnClickReplaceEquip);//替换
        
        this.equipUI.uiEquipObtain.mainAttrList3.itemRenderer = OnRenderEquippedEquipAttrs;//旧装备主属性
        this.equipUI.uiEquipObtain.attrList3.itemRenderer = OnRenderEquippedEntryAttrs;//旧装备词条属性
        this.equipUI.uiEquipObtain.mainAttrList4.itemRenderer = OnRenderNewEquipAttrs;//新装备主属性
        this.equipUI.uiEquipObtain.attrList4.itemRenderer = OnRenderNewEntryAttrs;//新装备词条属性
        
        this.equipUI.uiEquipRecommand.recycleBtn2.onClick.Set(OnClickRecycleButton);  // 新装备回收
        this.equipUI.uiEquipRecommand.upLoadBtn2.onClick.Set(OnClickEquipButton); // 新装备 装备
        this.equipUI.uiEquipRecommand.downBtn2.onClick.Set(OnClickRemoveButton);   // 新装备 卸下
        
        this.equipUI.uiEquipObtain.recycleBtn4.onClick.Set(OnClickRecycleButton);  // 旧装备 回收
        this.equipUI.uiEquipObtain.replaceBtn5.onClick.Set(OnClickReplaceButton); // 旧装备 替换

        EventDispatcher.GameWorld.Regist<ulong>(EventDefine.EVENT_EQUIP_CHANGE, OnEquipChangeInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_SIGLE_EQUIP_WEAR, OnEquipSingleWear);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PART_DECOMPOSE_RES, this.OnPartDecomposeSucc);
        EventDispatcher.GameWorld.Regist<ulong>(EventDefine.EVENT_LORE_EQUIP_EQUIP, OnLoreEquipChangeInfo);
    }

    public override void BindAll()
    {
        base.BindAll();

        EquipBinder.BindAll();
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist<ulong>(EventDefine.EVENT_EQUIP_CHANGE, OnEquipChangeInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_SIGLE_EQUIP_WEAR, OnEquipSingleWear);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PART_DECOMPOSE_RES, this.OnPartDecomposeSucc);
        EventDispatcher.GameWorld.UnRegist<ulong>(EventDefine.EVENT_LORE_EQUIP_EQUIP, OnLoreEquipChangeInfo);
    }

    protected override void OnShow()
    {
        base.OnShow();
        _isReceive = true;
        this.equipUI.uiEquipObtain.isNew.selectedIndex = 0;
        int uiType = isNew ? 1 : 0;
        this.equipUI.uiType.SetSelectedIndexEx(uiType);
        this.equipUI.uiEquipObtain.isGuide.selectedIndex = 0;

        this.equipUI.uiEquipObtain.btnCtrl.selectedIndex = 0;
        this.equipUI.uiEquipRecommand.typeCtrl.selectedIndex = 0;
        this.equipUI.uiEquipRecommand.newIcon.visible = true;

        this.equipUI.uiEquipObtain.autoSell2.selected = true;
        
        InitInheritUI(); //传承装备UI

        if(this.equipUI.uiType.selectedIndex == 1)
        {
            //引导-穿戴装备
            if (this.equipUI.uiEquipRecommand.typeCtrl.selectedIndex == 0)
            {
                GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    bid = GuideID.NewAccount_ClickBox,
                    gid = GuideID.NewAccount_ClickEquip,
                    tui = this.equipUI.uiEquipRecommand.btnEquip2,
                    isForce = true,
                    isSend = true,
                    npcTxt = "Beginner_Doc_002",
                    npcPosType = PosType.Down,
                    mType = 0
                });
                _isGuiding = true;
            }
            //引导-传承装备进行穿戴
            if (this.equipUI.uiEquipRecommand.typeCtrl.selectedIndex == 1)
            {
                if(GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    giding = GuideID.Click_UIFirstEquip,
                    gid = GuideID.Click_UIClothingEquip,
                    tui = this.equipUI.uiEquipRecommand.upLoadBtn2,
                    isForce = true,
                    isSend = true,
                    npcTxt = "Beginner_Doc_015",
                    npcPosType = PosType.Down,
                }))
                {
                    _isGuiding = true;
                }
            }
        }
        //if (!GuideManager.Instance.NotShowThisGuide((int) GuideID.Click_Equiped))
        //{
        //    _isGuiding = true;
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.equipUI.uiEquipRecommand.btnEquip2, GuideID.Click_Equiped, PosType.Down, true, true);
        //}
    }

    protected override void OnHide()
    {
        base.OnHide();
        if(EquipManager.Instance.curNoEquipGuid == 0)
            EquipManager.Instance.HasNewEquipToStopAutopack = false;
        if (_isGuiding)
        {
            GuideManager.Instance.HideGuide();
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TASK_UPDATE);
        }
        _isGuiding = false;
        EquipManager.Instance.isInheritEquip = false;

        var view = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
        view?.ChkGuideToTouch();
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);

        if (values.Length >= 2)
        {
            currEquipGuid = (ulong)values[0];
            isNew = (bool)values[1];
            
            if (values.Length >= 3)
            {
                goldFlyType = (int)values[2];
            }
        }
        else
        {
            LogUtils.LogWarning("No values provided.");
        }
    }

    #region 传承装备
    private void InitInheritUI()
    {
        if (!EquipManager.Instance.isInheritEquip) return;
        _flag = false;//修改状态
        if (isNew)
        {
            this.equipUI.uiType.selectedIndex = 1;
            this.equipUI.uiEquipRecommand.typeCtrl.selectedIndex = 1;
            this.equipUI.uiEquipRecommand.status.selectedIndex = 1;
            this.equipUI.uiEquipRecommand.newIcon.visible = false;
        }
        else
        {
            //判断是已经装备上的
            EquipData equipData = EquipManager.Instance.GetEquipById(currEquipGuid);
            if (RoleManager.Instance.GetLoreEquipSlotInfoBySlotType(equipData.partType) == null)
            {
                // 部位没有装备
                this.equipUI.uiType.selectedIndex = 1;
                this.equipUI.uiEquipRecommand.typeCtrl.selectedIndex = 1;
                this.equipUI.uiEquipRecommand.status.selectedIndex = 0;
                this.equipUI.uiEquipRecommand.newIcon.visible = true;
            }
            else
            {
                this.equipUI.uiType.selectedIndex = 0;
                this.equipUI.uiEquipObtain.btnCtrl.selectedIndex = 1;
            }
        }
    }
    
    //传承 新装备和旧装备的 回收按钮
    private void OnClickRecycleButton()
    {
        this.Hide();
        Debug.Log("==SellLoreEquip_CS=currEquipGuid="+currEquipGuid);
        var builder = SellLoreEquip_CS.CreateBuilder();
        builder.EquipGuid = currEquipGuid;
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_SellLoreEquip_CS, builder.Build());
    }
    
    //新装备的卸下按钮
    private void OnClickRemoveButton()
    {
        
        this.Hide();
        var builder = TakeOffLoreEquip_CS.CreateBuilder();
        builder.EquipGuid = currEquipGuid;
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_TakeOffLoreEquip_CS, builder.Build());
    }
    
    //旧装备替换按钮
    private void OnClickReplaceButton()
    {
        var builder = ReplaceLoreEquip_CS.CreateBuilder();
        builder.EquipGuid = currEquipGuid;
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ReplaceLoreEquip_CS, builder.Build());
    }
    
    //新装备的装备按钮
    private void OnClickEquipButton()
    {
        this.Hide();
        // var builder = LoreEquipOnPlayer_CS.CreateBuilder();
        // builder.EquipGuid = currEquipGuid;
        // GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_LoreEquipOnPlayer_CS, builder.Build());
        
        var builder = ReplaceLoreEquip_CS.CreateBuilder();
        builder.EquipGuid = currEquipGuid;
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ReplaceLoreEquip_CS, builder.Build());

        //var lobby = UIManager.Instance.FindByName("Lobby") as LobbyView;
        //lobby?.ChkGuide();
    }
    #endregion
    
    private void OnClickClose()
    {
        this.SetVisible(false);
    }

    private void OnEquipSingleWear()
    {
        _isReceive = true;
        // UIManager.Instance.ToastByKey(StringDefine.STRING_USE_EQUIP_SUCC);
        if(EquipManager.Instance.curNoEquipGuid > 0)
            EquipManager.Instance.GetEquipById(EquipManager.Instance.curNoEquipGuid).isNew = false;
        EquipManager.Instance.curNoEquipGuid = 0;
        EquipManager.Instance.isNewEquip = false;
        SetVisible(false);
    }

    /// <summary>
    /// 开出新装备，点击触发使用事件。之前该部位上没有装备
    /// </summary>
    private void OnClickNewEquip()
    {
        if(!_isReceive) return;
        _isReceive = false;
        FightUtils.IsWearEquip = true;
        var builder = ReplaceEquip_CS.CreateBuilder();
        builder.EquipGuid = currEquipGuid;
        ReplaceEquip_CS replaceEquipCs = builder.Build();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ReplaceEquip_CS, replaceEquipCs);
    }

    /// <summary>
    /// 分解装备
    /// </summary>
    private void OnClickDecomEquip()
    {
        if(!_isReceive) return;
        EquipData equipData = EquipManager.Instance.GetEquipById(currEquipGuid);
        if (equipData != null)
        {
            int sourceId = equipData.id;
            ConfigItemTypeUnit config = ConfigUtils.GetConfigItemTypeUnitById(sourceId);
            if (null != config)
            {
                //旧部位id
                partEquipData = DataManager.Instance.FindEquip((EN_EQUIP_PARTS) config.Parts);
                partEquipData.isNew = false;
            }
            
            GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GoldPickupSE);
            if (goldFlyType == 1)//大地图
            {
                var view = UIManager.Instance.FindByName("ChapterMap") as  ChapterMapView;
                if (view != null)
                {
                    UI_ComUserInfo userInfo = view?.GetBigMapUIComUserInfo() as UI_ComUserInfo;
                    if (userInfo != null)
                    {
                        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(2000);
                        UI_BtnCurrency btnGold = ((UI_ComCurrency)userInfo.comCurrency).btnGold;
                        UIItemsGain itemsGain = UIGainBasePool.CreateUIGainBase();
                        itemsGain.ApplyItemSourceToDestination(btnGold, itemTypeUnit.Icon);
                        Vector2 pos = (this.equipUI.uiEquipObtain.resolveBtn4).LocalToGlobal(Vector2.zero);
                        itemsGain.StartItemFly(pos, 5);
                    }
                }
                

            }

            if (goldFlyType == 2)//装备界面
            {
                var view = UIManager.Instance.FindByName("EquipSystem") as  EquipSystemView;
                if (view != null)
                {
                    ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(2000);
                    UI_EquipCurrency equipCurrency = view?.GetEquipSystemGoldCur() as UI_EquipCurrency;
                    UIItemsGain itemsGain = UIGainBasePool.CreateUIGainBase();
                    itemsGain.ApplyItemSourceToDestination(equipCurrency, itemTypeUnit.Icon);
                    Vector2 pos = (this.equipUI.uiEquipObtain.resolveBtn4).LocalToGlobal(Vector2.zero);
                    itemsGain.StartItemFly(pos, 5);
                }
            }
            
            SetVisible(false);
            _isReceive = false;
            _flag = false;//修改状态
            EquipManager.Instance.DecomposeEquip(currEquipGuid);
        }

    }

    /// <summary>
    /// 替换装备
    /// </summary>
    private void OnClickReplaceEquip()
    {
        if(!_isReceive) return;
        _isReceive = false;
        FightUtils.IsWearEquip = true;
        if (this.equipUI.uiEquipObtain.autoSell2.selected)
        {
            var builder = ReplaceEquipAutoSell_CS.CreateBuilder();
            builder.EquipGuid = currEquipGuid;
            ReplaceEquipAutoSell_CS replaceEquipCs = builder.Build();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ReplaceEquipAutoSell_CS, replaceEquipCs);
            
            GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GoldPickupSE);
            if (goldFlyType == 1)//大地图
            {
                var view = UIManager.Instance.FindByName("ChapterMap") as  ChapterMapView;
                if (view != null)
                {
                    UI_ComUserInfo userInfo = view?.GetBigMapUIComUserInfo() as UI_ComUserInfo;
                    if (userInfo != null)
                    {
                        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(2000);
                        UI_BtnCurrency btnGold = ((UI_ComCurrency)userInfo.comCurrency).btnGold;
                        UIItemsGain itemsGain = UIGainBasePool.CreateUIGainBase();
                        itemsGain.ApplyItemSourceToDestination(btnGold, itemTypeUnit.Icon);
                        Vector2 pos = (this.equipUI.uiEquipObtain.repalceBtn4).LocalToGlobal(Vector2.zero);
                        itemsGain.StartItemFly(pos, 5);
                    }
                }
                

            }

            if (goldFlyType == 2)//装备界面
            {
                var view = UIManager.Instance.FindByName("EquipSystem") as  EquipSystemView;
                if (view != null)
                {
                    ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(2000);
                    UI_EquipCurrency equipCurrency = view?.GetEquipSystemGoldCur() as UI_EquipCurrency;
                    UIItemsGain itemsGain = UIGainBasePool.CreateUIGainBase();
                    itemsGain.ApplyItemSourceToDestination(equipCurrency, itemTypeUnit.Icon);
                    Vector2 pos = (this.equipUI.uiEquipObtain.repalceBtn4).LocalToGlobal(Vector2.zero);
                    itemsGain.StartItemFly(pos, 5);
                }
            }
        }
        else
        {
            var builder = ReplaceEquip_CS.CreateBuilder();
            builder.EquipGuid = currEquipGuid;
            ReplaceEquip_CS replaceEquipCs = builder.Build();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ReplaceEquip_CS, replaceEquipCs);
        }

    }

    private void OnChangedUIType(EventContext context)
    {
        var tab = this.equipUI.uiType.selectedIndex;
        switch (tab)
        {
            case 1:
                UpdateNewEquipInfo();
                break;
            case 0:
                UpdateEquippedEquipInfo();
                break;
        }
    }

    private void SetItemValue(GComponent item, int id, double value, bool flag = false)//flag是否为主属性
    {
        if (id < 1000)
        {
            // item.GetChild("txtAttrName").text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(id).AttrName);//读取属性配置表
            // item.GetChild("txtAttrValue").text = EquipManager.Instance.SetAttributeValue(id, value);//$"{StringUtils.FormatCurrency(value)}%";
            // int entryId = partEquipData.lstEntryCfgsID[index];
            // item.GetChild("desc").text = ConfigUtils.GetConfigEntryUnit(entryId).Desc; 
            
            GTextField txtAttrName = item.GetChild("txtAttrName") as GTextField;
            GTextField txtAttrValue = item.GetChild("txtAttrValue") as GTextField;
            
            txtAttrName.text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(id).AttrName);//读取属性配置表
            txtAttrValue.text = EquipManager.Instance.SetAttributeValue(id, value);//$"{StringUtils.FormatCurrency(value)}%";


            if (flag)
            {//为主属性
                txtAttrName.color = GetColorFromHex("#FCF78E");
                txtAttrValue.color = GetColorFromHex("#FCF78E");
            }
            else
            {
                txtAttrName.color = GetColorFromHex("#4C2911");
                txtAttrValue.color = GetColorFromHex("#4C2911");
            }
            
        }
        else  //是技能
        {
            // item.GetChild("txtAttrName").text = ConfigUtils.GetTextById(ConfigUtils.GetSkillById(id).Name);//读取属性配置表
            // item.GetChild("txtAttrValue").text = ConfigUtils.GetStringByKey(8046) + "+" + value;

            GTextField txtAttrName = item.GetChild("txtAttrName") as GTextField;
            GTextField txtAttrValue = item.GetChild("txtAttrValue") as GTextField;
            
            txtAttrName.text = ConfigUtils.GetTextById(ConfigUtils.GetSkillById(id).Name);
            txtAttrValue.text = ConfigUtils.GetStringByKey(8046) + "+" + value;
            
            txtAttrName.color = GetColorFromHex("#F05769");
            txtAttrValue.color = GetColorFromHex("#F05769");
        }
    }
    
    private Color GetColorFromHex(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        // 如果解析失败，返回默认颜色（白色）
        return Color.white;
    }
    
    //新获得装备主属性，之前未拥有该部位装备
    private void OnRenderEquipAttrs(int index, GObject obj)
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
        SetItemValue(item, id, value, true);
        // // item.GetChild("txtAttrName").text = StringUtils.ConvertToAttributeName(id);
        // item.GetChild("txtAttrName").text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(id).AttrName);//读取属性配置表
        // item.GetChild("txtAttrValue").text = SetAttributeValue(id, value);//$"{StringUtils.FormatCurrency(value)}%";
        // // int entryId = partEquipData.lstEntryCfgsID[index];
        // // item.GetChild("desc").text = ConfigUtils.GetConfigEntryUnit(entryId).Desc; 
    }
    
    // 新获得装备词条属性，之前未拥有该部位装备
    private void OnRenderEntryAttrs(int index, GObject obj)
    {
        GComponent item = (GComponent)obj;
        if (null == item)
            return;

        if (partEquipData.IsNull())
            return;

        if (index >= partEquipData.lstEntrysID.Count)
            return;

        int id = partEquipData.lstEntrysID[index];
        double value = partEquipData.lstEntrysValue[index];
        SetItemValue(item, id, value);
        // // item.GetChild("txtAttrName").text = StringUtils.ConvertToAttributeName(id);
        // item.GetChild("txtAttrName").text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(id).AttrName);//读取属性配置表
        // item.GetChild("txtAttrValue").text = SetAttributeValue(id, value);//$"{StringUtils.FormatCurrency(value)}%";
        // // int entryId = partEquipData.lstEntryCfgsID[index];
        // // item.GetChild("desc").text = ConfigUtils.GetConfigEntryUnit(entryId).Desc; 
    }

    //使用中的
    private void OnRenderEquippedEquipAttrs(int index, GObject obj)
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
        SetItemValue(item, id, value, true);
        // // item.GetChild("txtAttrName").text = StringUtils.ConvertToAttributeName(id);
        // item.GetChild("txtAttrName").text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(id).AttrName);//属性名称读取配置表
        // item.GetChild("txtAttrValue").text = SetAttributeValue(id, value);//StringUtils.FormatCurrency(value);
        // item.GetController("status").selectedIndex = 0;
    }

    //新获得装备
    private void OnRenderNewEquipAttrs(int index, GObject obj)
    {
        GComponent item = (GComponent)obj;
        if (null == item)
            return;

        if (obtainEquipData.IsNull())
            return;

        if (index >= obtainEquipData.lstAttrsID.Count)
            return;

        int id = obtainEquipData.lstAttrsID[index];
        double valueObtatin = obtainEquipData.lstAttrsValue[index];
        SetItemValue(item, id, valueObtatin, true);
        // // item.GetChild("txtAttrName").text = StringUtils.ConvertToAttributeName(id);
        // item.GetChild("txtAttrName").text =  ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(id).AttrName);//属性名称读取配置表
        // item.GetChild("txtAttrValue").text = SetAttributeValue(id, valueObtatin);//StringUtils.FormatCurrency(valueObtatin);

        // 新的不需要，先注释
        // if (!partEquipData.IsNull() && index < partEquipData.lstAttrsValue.Count)
        // {
        //     double valueOld = partEquipData.lstAttrsValue[index];
        //     if (valueObtatin > valueOld)
        //     {
        //         item.GetController("status").selectedIndex = 2;
        //     }
        //     else if (valueObtatin < valueOld)
        //     {
        //         item.GetController("status").selectedIndex = 1;
        //     }
        //     else
        //     {
        //         item.GetController("status").selectedIndex = 0;
        //     }
        // }
        // else 
        // {
        //     item.GetController("status").selectedIndex = 0;
        // }
    }
    
    //使用中的词条
    private void OnRenderEquippedEntryAttrs(int index, GObject obj)
    {
        GComponent item = (GComponent)obj;
        if (null == item)
            return;

        if (partEquipData.IsNull())
            return;

        if (index >= partEquipData.lstEntrysID.Count)
            return;

        int id = partEquipData.lstEntrysID[index];
        double value = partEquipData.lstEntrysValue[index];
        SetItemValue(item, id, value);
        // // item.GetChild("txtAttrName").text = StringUtils.ConvertToAttributeName(id);
        // item.GetChild("txtAttrName").text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(id).AttrName);//属性名称读取配置表
        // item.GetChild("txtAttrValue").text = SetAttributeValue(id, value);//$"{StringUtils.FormatCurrency(value)}%";
        // int entryId = partEquipData.lstEntryCfgsID[index];
        // item.GetChild("desc").text = ConfigUtils.GetConfigEntryUnit(entryId).Desc;
    }

    //新获得装备词条 【词条是不比较大小的】
    private void OnRenderNewEntryAttrs(int index, GObject obj)
    {
        GComponent item = (GComponent)obj;
        if (null == item)
            return;

        if (obtainEquipData.IsNull())
            return;

        if (index >= obtainEquipData.lstEntrysID.Count)
            return;

        int id = obtainEquipData.lstEntrysID[index];
        double valueObtatin = obtainEquipData.lstEntrysValue[index];
        SetItemValue(item, id, valueObtatin);
        // // item.GetChild("txtAttrName").text = StringUtils.ConvertToAttributeName(id);
        // item.GetChild("txtAttrName").text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(id).AttrName);//属性名称读取配置表
        // item.GetChild("txtAttrValue").text = SetAttributeValue(id, valueObtatin);//$"{StringUtils.FormatCurrency(valueObtatin)}%";
        // int entryId = obtainEquipData.lstEntryCfgsID[index];
        // item.GetChild("desc").text = ConfigUtils.GetConfigEntryUnit(entryId).Desc;
    }
    
    private bool isLoreWear = false;
    private List<EquipData> loreEquipList;
    // 修改后的新获得装备更新
    private void UpdateNewEquipInfo()
    {
        int sourceId = EquipManager.Instance.GetEquipById(currEquipGuid).id;
        ConfigItemTypeUnit config = ConfigUtils.GetConfigItemTypeUnitById(sourceId);
        if (null != config)
        {
            partEquipData = EquipManager.Instance.GetEquipById(currEquipGuid);
            if (null != partEquipData)
            {
                this.equipUI.uiEquipRecommand.txtEquipItem.text = ConfigUtils.GetTextById(config.Name);
                this.equipUI.uiEquipRecommand.mainAttrList2.numItems = partEquipData.GetAttrCount();//主属性
                this.equipUI.uiEquipRecommand.attrsList2.numItems = partEquipData.GetEntryCount();//其他属性
                this.equipUI.uiEquipRecommand.attrsList2.ResizeToFit();
                
                UI_ComEquipItem comEquipItem2 = (UI_ComEquipItem)equipUI.uiEquipRecommand.comEquipItem2;
                comEquipItem2.icon.url = UIResource.GetItemUrl(config.Icon);
                // comEquipItem.txtLv.SetVar("value", partEquipData.lv.ToString()).FlushVars();
                comEquipItem2.ctrlQuality.selectedIndex = partEquipData.quality - 1;
                this.equipUI.uiEquipRecommand.equipLv.SetVar("value", partEquipData.lv.ToString()).FlushVars();

                isLoreWear = false;
                loreEquipList = EquipManager.Instance.GetInstallLoreEquipsList();
                foreach (var equipData in loreEquipList)
                {
                    if (equipData.partType == partEquipData.partType)
                    {
                        isLoreWear = true;
                        break;
                    }
                }

                double fight = 0f;
                if (isLoreWear)
                    fight = FightUtils.GetOneEquipFight(partEquipData, partEquipData, true);
                else
                    fight = FightUtils.GetOneEquipFight(partEquipData, null, false);
                
                this.equipUI.uiEquipRecommand.fightLb.text = StringUtils.FormatCurrency(fight);
                
                this.equipUI.uiEquipRecommand.hasEntry.selectedIndex = partEquipData.GetEntryCount() == 0 ? 1 : 0;
            }
        }
    }

    private EquipData partEquipData = new EquipData();
    private EquipData obtainEquipData = new EquipData();
    
    private void UpdateEquippedEquipInfo()
    {
        int sourceId = EquipManager.Instance.GetEquipById(currEquipGuid).id;//开出的新装备的配置表中的id
        ConfigItemTypeUnit config = ConfigUtils.GetConfigItemTypeUnitById(sourceId);
        if (null != config)
        {
            //旧部位id
            partEquipData = DataManager.Instance.FindEquip((EN_EQUIP_PARTS)config.Parts);//根据部位参数找到旧装备
            if (null != partEquipData && !partEquipData.IsNull())
            {
                var newConfig = ConfigUtils.GetConfigItemTypeUnitById(partEquipData.GetSourceId());//旧装备配置表信息
                if (null != newConfig)
                {
                    this.equipUI.uiEquipObtain.name3.text = ConfigUtils.GetTextById(newConfig.Name);
                    
                    this.equipUI.uiEquipObtain.mainAttrList3.numItems = partEquipData.GetAttrCount();
                    this.equipUI.uiEquipObtain.attrList3.numItems = partEquipData.GetEntryCount();
                    this.equipUI.uiEquipObtain.attrList3.ResizeToFit();
                    
                    // this.equipUI.uiEquipObtain.upNew.visible = partEquipData.isNew;
                    UI_ComEquipItem comEquipItem3 = (UI_ComEquipItem)equipUI.uiEquipObtain.equip3;
                    comEquipItem3.icon.url = UIResource.GetItemUrl(newConfig.Icon);
                    // comEquipItem.txtLv.SetVar("value", partEquipData.lv.ToString()).FlushVars();
                    this.equipUI.uiEquipObtain.equipLv3.SetVar("value", partEquipData.lv.ToString()).FlushVars();
                    comEquipItem3.ctrlQuality.selectedIndex = partEquipData.quality - 1;
                    
                    this.equipUI.uiEquipObtain.hasEntryOfOld.selectedIndex = partEquipData.GetEntryCount() == 0 ? 1 : 0;
                }

                //新获得
                obtainEquipData = EquipManager.Instance.GetObtainEquip(currEquipGuid);
                if (null != obtainEquipData)
                {
                    // this.equipUI.uiEquipObtain.downNew.visible = obtainEquipData.isNew;
                    UI_ComEquipItem comNewEquipItem4 = (UI_ComEquipItem)equipUI.uiEquipObtain.equip4;
                    // comNewEquipItem4.txtLv.SetVar("value", obtainEquipData.lv.ToString()).FlushVars();
                    this.equipUI.uiEquipObtain.equipLv4.SetVar("value", obtainEquipData.lv.ToString()).FlushVars();
                    comNewEquipItem4.ctrlQuality.selectedIndex = obtainEquipData.quality - 1;
                    this.equipUI.uiEquipObtain.mainAttrList4.numItems = obtainEquipData.GetAttrCount();
                    this.equipUI.uiEquipObtain.attrList4.numItems = obtainEquipData.GetEntryCount();
                    this.equipUI.uiEquipObtain.attrList4.ResizeToFit();
                    this.equipUI.uiEquipObtain.name4.text = ConfigUtils.GetTextById(config.Name);
                    comNewEquipItem4.icon.url = UIResource.GetItemUrl(config.Icon);
                    
                    this.equipUI.uiEquipObtain.hasEntryOfNew.selectedIndex = obtainEquipData.GetEntryCount() == 0 ? 1 : 0;
                }
                
                double upFight = FightUtils.GetOneEquipFight(partEquipData, partEquipData, true);//旧
                double downFight = FightUtils.GetOneEquipFight(obtainEquipData, partEquipData, false);//新
                if (upFight > downFight)
                {
                    this.equipUI.uiEquipObtain.fightCtrl.selectedIndex = 0;
                }
                else if(upFight<downFight)
                {
                    this.equipUI.uiEquipObtain.fightCtrl.selectedIndex = 1;
                }
                else
                {
                    this.equipUI.uiEquipObtain.fightCtrl.selectedIndex = 2;
                }
                
                this.equipUI.uiEquipObtain.fightLb3.text = StringUtils.FormatCurrency(upFight);//旧装备
                this.equipUI.uiEquipObtain.fightLb4.text = StringUtils.FormatCurrency(downFight);//新装备
            }
        }
    }
    
    private void OnLoreEquipChangeInfo(ulong oldGuid)
    {
        _isReceive = true;
        if (!IsShow() || !IsOnStage()) return;
        currEquipGuid = oldGuid;
        // EquipManager.Instance.curNoEquipGuid = currEquipGuid;
        UpdateEquippedEquipInfo();
        this.equipUI.uiEquipObtain.isNew.selectedIndex = _flag ? 0 : 1;
        _flag = !_flag;
    
        int sourceId = EquipManager.Instance.GetEquipById(currEquipGuid).id;
        ConfigItemTypeUnit config = ConfigUtils.GetConfigItemTypeUnitById(sourceId);
        if (null != config)
        {
            //旧部位id
            partEquipData = DataManager.Instance.FindEquip((EN_EQUIP_PARTS) config.Parts);
        
            obtainEquipData = EquipManager.Instance.GetObtainEquip(currEquipGuid);
            _stringBuilder.Clear();
            for (int i = 0; i < partEquipData.lstAttrsID.Count; i++)
            {
                for (int j = 0; j < obtainEquipData.lstAttrsID.Count; j++)
                {
                    if (partEquipData.lstAttrsID[i] == obtainEquipData.lstAttrsID[j])
                    {
                        double addValue = partEquipData.lstAttrsValue[i] - obtainEquipData.lstAttrsValue[j];
                        _stringBuilder.Append(addValue>0 ? "+" : "");
                        _stringBuilder.Append(addValue.ToString());
                        // _stringBuilder.Append(StringUtils.ConvertToAttributeName(partEquipData.lstAttrsID[i]));
                        _stringBuilder.Append(ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(partEquipData.lstAttrsID[i]).AttrName));
                        _stringBuilder.Append(" ");
                    }
                }
            }
            // if(_stringBuilder.Length > 0)
            // TipsManger.Instance.ShowTip(_stringBuilder.ToString());
        }

    }
    
    private void OnEquipChangeInfo(ulong oldGuid)
    {
        _isReceive = true;
        if (!IsShow() || !IsOnStage()) return;
        currEquipGuid = oldGuid;
        EquipManager.Instance.curNoEquipGuid = currEquipGuid;
        UpdateEquippedEquipInfo();
        this.equipUI.uiEquipObtain.isNew.selectedIndex = _flag ? 0 : 1;
        _flag = !_flag;
    
        int sourceId = EquipManager.Instance.GetEquipById(currEquipGuid).id;
        ConfigItemTypeUnit config = ConfigUtils.GetConfigItemTypeUnitById(sourceId);
        if (null != config)
        {
            //旧部位id
            partEquipData = DataManager.Instance.FindEquip((EN_EQUIP_PARTS) config.Parts);
        
            obtainEquipData = EquipManager.Instance.GetObtainEquip(currEquipGuid);
            _stringBuilder.Clear();
            for (int i = 0; i < partEquipData.lstAttrsID.Count; i++)
            {
                for (int j = 0; j < obtainEquipData.lstAttrsID.Count; j++)
                {
                    if (partEquipData.lstAttrsID[i] == obtainEquipData.lstAttrsID[j])
                    {
                        double addValue = partEquipData.lstAttrsValue[i] - obtainEquipData.lstAttrsValue[j];
                        _stringBuilder.Append(addValue>0 ? "+" : "");
                        _stringBuilder.Append(addValue.ToString());
                        // _stringBuilder.Append(StringUtils.ConvertToAttributeName(partEquipData.lstAttrsID[i]));
                        _stringBuilder.Append(ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(partEquipData.lstAttrsID[i]).AttrName));
                        _stringBuilder.Append(" ");
                    }
                }
            }
            // if(_stringBuilder.Length > 0)
            // TipsManger.Instance.ShowTip(_stringBuilder.ToString());
        }

    }

    private void OnPartDecomposeSucc()
    {
        _isReceive = true;
        if(_isGuiding)
            GuideManager.Instance.HideGuide();
        SetVisible(false);
    }
}