using Common;
using CommonEx;
using Config;
using EngineBase;
using Equip;
using FairyGUI;
using Lobby;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine
{
    // 1=基础道具
    // 2=装备道具
    // 3=宠物
    public enum Tipstype
    {
        None=0,//填这个代表是动态去item表里获取
        Item = 1,
        HeroLevelAttr = 2,
        Pet = 3,
        Skill = 5,
        Stone = 6,//符石 
        Monster = 7, //地图关卡怪物属性提示
        Achievement = 8,//成就属性提示
        ClassicInfo = 9,//秘典满级信息
        Classic = 10,//秘典属性
        CopyBossTx = 11,//副本BOSS特性
    }
    public class TipsManger : Singleton<TipsManger>
    {
        private GComponent _tipParent;
        private GObjectPool _objectPool;
        private static int tipCount = 5;

        private GComponent _popTips = null; 
        private GComponent _PopTarget = null;

        private StringBuilder _stringBuilder = new StringBuilder();
        private void Init()
        {
            _tipParent = new GComponent();
            _tipParent.gameObjectName = "TipsManger";
            GameObject.DontDestroyOnLoad(_tipParent.displayObject.gameObject);
            _tipParent.touchable = true;
            GRoot.inst.AddChild(_tipParent);
            _tipParent.sortingOrder = 9999999;
            tipCount = 0;
            _objectPool = new GObjectPool(_tipParent.container.cachedTransform);
            
            Stage.inst.onTouchBegin.AddCapture(__stageTouchEnd);
        }

        public void ShowTip(string content)
        {
            if (_tipParent == null) Init();
            UI_ToastWindow tWindow = _objectPool.GetObject("ui://Common/ToastWindow") as UI_ToastWindow;
            tWindow.visible = false;
            _tipParent.AddChild(tWindow);
            tWindow.touchable = false;
            tWindow.ctrlMode.selectedIndex = 0;
            GameManager.Instance.TimerManager.SetTimer(tipCount * 0.3f, (toast) =>
            {
                UI_ToastWindow toastWindow = toast as UI_ToastWindow; 
                toastWindow.visible = true;
               if (!string.IsNullOrEmpty(content))
               {
                   toastWindow.t0.Play();
                   toastWindow.tips.txt_title.text = content;
                   GameManager.Instance.TimerManager.SetTimer(1.5f, OnHideUI, toastWindow);
               }
               else
               {
                   LogUtils.LogWarning("value does not exist");
               }
            }, tWindow);
            tipCount++;
        }
        
        public void ShowTip(string iconUr, string content)
        {
            if (_tipParent == null) Init();
            UI_ToastWindow tWindow = _objectPool.GetObject("ui://Common/ToastWindow") as UI_ToastWindow;
            tWindow.visible = false;
            _tipParent.AddChild(tWindow);
            tWindow.touchable = false;
            tWindow.ctrlMode.selectedIndex = 1;
            tipCount++;
            GameManager.Instance.TimerManager.SetTimer(tipCount * 0.3f, (toast) =>
            {
                UI_ToastWindow toastWindow = toast as UI_ToastWindow; 
                toastWindow.visible = true;
 
                if (!string.IsNullOrEmpty(iconUr) && !string.IsNullOrEmpty(content))
                {
                    toastWindow.iconAni.Play();
                    toastWindow.iconTips.txt_Icon_title.text = content;
                    toastWindow.iconTips.loader_Icon.url = iconUr;
                    GameManager.Instance.TimerManager.SetTimer(1.5f, OnHideUI, toastWindow);
                }else if (!string.IsNullOrEmpty(content))
                {
                    toastWindow.t0.Play();
                    toastWindow.tips.txt_title.text = content;
                    GameManager.Instance.TimerManager.SetTimer(1.5f, OnHideUI, toastWindow);
                }
                else
                {
                    LogUtils.LogWarning("value does not exist");
                }
            }, tWindow);
       
        }

        private void OnHideUI(object obj)
        {
            tipCount--;
            UI_ToastWindow toastWindow  = obj as UI_ToastWindow;
            toastWindow.visible = false;
            _objectPool.ReturnObject(toastWindow);
        }

        public void ShowPopupTip(GObject target, Tipstype tipstype, params object[] values)
        {
            if (_tipParent == null) Init();
            if (_popTips != null)
            {
                ClosePopupTip();
                return;
            }

            _PopTarget = target as GComponent;
            if (tipstype != Tipstype.Skill && tipstype != Tipstype.HeroLevelAttr && tipstype != Tipstype.Monster && tipstype != Tipstype.Achievement && tipstype != Tipstype.ClassicInfo && tipstype != Tipstype.Classic && tipstype != Tipstype.CopyBossTx)
            {
                // 客户端显示道具tips调用的样式类型
                // 1=基础道具
                // 2=装备道具
                // 3=宠物
                // 5=技能
                // 6=符石
                int itemId = (int) values[0];
                ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemId);
                if (itemTypeUnit == null)
                {
                    LogUtils.LogErrorFormat("道具表找不到该道具id={0}的物品", itemId);
                    return;
                }
                tipstype = (Tipstype) itemTypeUnit.Tipstype;
            }

            // if (tipstype == Tipstype.Pet)
            // {
            //     ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById((int) values[0]);
            //     int petId = itemTypeUnit.Param;
            //     PetItemInfo petItemInfo = PetInfoManager.Instance.GetPetByPetId(petId);
            //     if (petItemInfo == null)//如果当前没有拥有该宠物的话，去配置表读取默认的
            //         petItemInfo = PetInfoManager.Instance.GetPetInAll(petId);
            //     UIManager.Instance.ShowUIPanel("PetDetail", petItemInfo, true);
            // }
            if (tipstype == Tipstype.Skill)
            {
                int skillId = (int) values[0];
                SkillInfo skillInfo = SkillInfoManager.Instance.GetSkill(skillId);
                if (skillInfo == null) // 如果当前没有拥有该技能的话，去配置表读取默认的
                    skillInfo = SkillInfoManager.Instance.GetSkillInAll(skillId);
                UIManager.Instance.ShowUIPanel("SkillDetail", skillInfo, true);
            }
            else if (tipstype == Tipstype.Stone)
            {
                int stoneId = (int) values[0];
                ulong stoneGuid = (ulong) values[1];
                RuneInfo runeInfo = RuneInfoManager.Instance.GetRune((ulong)stoneGuid);
                if (runeInfo == null)
                    runeInfo = RuneInfoManager.Instance.GetRuneInAll((ulong)stoneId);
                UIManager.Instance.ShowUIPanel("RuneDetail", runeInfo, true);

            }
            else if (tipstype == Tipstype.HeroLevelAttr)
            {
                UI_CommonHeroLevelAttrTips commonItem = _objectPool.GetObject("ui://CommonEx/CommonHeroLevelAttrTips") as UI_CommonHeroLevelAttrTips;
                if (commonItem == null) return;
                //value[0]  描述   value[1] 是否解锁  value[2] 解锁等级
                int arrtId = (int) values[0];
                string name = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(arrtId).AttrName);
                commonItem.pContent.SetVar("name", ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(arrtId).AttrName)).SetVar("value", values[1].ToString()).FlushVars();
                commonItem.lockCtrl.selectedIndex = (bool) values[2] ? 0 : 1;
                commonItem.lockLb.SetVar("lv", values[3].ToString()).FlushVars();
                // commonItem.type.selectedIndex = (int) values[3] - 1;
                _tipParent.AddChild(commonItem);
                Vector2 pos = GRoot.inst.GetPoupPosition(commonItem, target, PopupDirection.Up);
                // commonItem.xy = pos;
                commonItem.x = pos.x;
                commonItem.y = pos.y - 10;
                float targetCenterX = target.x + target.width / 2;
                float flagImgLocalX = targetCenterX - commonItem.x;
                commonItem.flagImg.x = flagImgLocalX + 50;
                commonItem?.onRemovedFromStage.Add(OnPopupClosed);
                _popTips = commonItem;
            }else if (tipstype == Tipstype.Monster)
            {
                UI_CommonAttrTips commonItem = _objectPool.GetObject("ui://CommonEx/CommonAttrTips") as UI_CommonAttrTips;
                if (commonItem == null) return;
                //value[0]  名称   value[1] 描述    value[2] 图标
                commonItem.pContentName.text = values[0].ToString();
                commonItem.pContentDesc.text = values[1].ToString();
                commonItem.bossAttr.icon = UIResource.GetMonsterEntryIcon((int)values[2]);
                
                _tipParent.AddChild(commonItem);
                Vector2 globalPos = target.LocalToGlobal(Vector2.zero);
                Vector2 localPos = GRoot.inst.GlobalToLocal(globalPos);
                commonItem.xy = new Vector2(localPos.x - 20,localPos.y + target.height/2);
                commonItem?.onRemovedFromStage.Add(OnPopupClosed);
                _popTips = commonItem;
            }else if (tipstype == Tipstype.CopyBossTx)
            {
                UI_CommonCopyBossTxTips commonItem = _objectPool.GetObject("ui://CommonEx/CommonCopyBossTxTips") as UI_CommonCopyBossTxTips;
                if (commonItem == null) return;
                //value[0]  名称   value[1] 描述    value[2] 图标
                commonItem.pContentName.text = values[0].ToString();
                commonItem.pContentDesc.text = values[1].ToString();
                commonItem.bossAttr.icon = UIResource.GetMonsterEntryIcon((int)values[2]);
                
                _tipParent.AddChild(commonItem);
                commonItem.xy = GRoot.inst.GetPoupPosition(commonItem, target, PopupDirection.Auto);
                commonItem?.onRemovedFromStage.Add(OnPopupClosed);
                _popTips = commonItem;
                
            }else if (tipstype == Tipstype.Achievement)
            {
                UI_Tips commonItem = _objectPool.GetObject("ui://CommonEx/Tips") as UI_Tips;
                if (commonItem == null) return;
                
                commonItem.name.text = values[0].ToString();//属性id
                commonItem.num.text = values[1].ToString();//属性值
                int xPos = (int)values[2];//x偏移量
                
                _tipParent.AddChild(commonItem);
                Vector2 pos = GRoot.inst.GetPoupPosition(commonItem, target, PopupDirection.Up);
                commonItem.xy = new Vector2(pos.x+xPos, pos.y);//new Vector2(pos.x-120, pos.y)
                commonItem?.onRemovedFromStage.Add(OnPopupClosed);
                _popTips = commonItem;
            }else if (tipstype == Tipstype.ClassicInfo)
            {
                //秘典满级属性tips
                UI_ClassicInfo classicInfo = _objectPool.GetObject("ui://Lobby/ClassicInfo") as UI_ClassicInfo;
                if (classicInfo == null) return;

                ClassicInfo info = values[0] as ClassicInfo;
                bool isMax = (bool)values[1];
                classicInfo.maxCtrl.selectedIndex = isMax ? 1 : 0;
                
                ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeByParam(info.classicId);
                classicInfo.classicName.text = ConfigUtils.GetTextById(itemTypeUnit.Name);
                classicInfo.classItem.icon = UIResource.GetItemUrl(itemTypeUnit.Icon.ToString());//获取秘典图标
                classicInfo.maxAttrList.itemRenderer = AttrItem2Render;
                List<ClassicAttr> maxAttrsList = info.maxAttrs;
                classicInfo.maxAttrList.data = info;
                classicInfo.maxAttrList.numItems = maxAttrsList.Count;
                classicInfo.maxAttrList.ResizeToFit();
                
                _tipParent.AddChild(classicInfo);
                Vector2 globalPos = target.LocalToGlobal(Vector2.zero);
                Vector2 localPos = GRoot.inst.GlobalToLocal(globalPos);
                classicInfo.xy = new Vector2(localPos.x - 20, localPos.y - classicInfo.height - (39 * (maxAttrsList.Count - 1)));
                classicInfo?.onRemovedFromStage.Add(OnPopupClosed);
                _popTips = classicInfo;
            }else if (tipstype == Tipstype.Classic)
            {
                // 秘典其他属性  旧逻辑
                // UI_AttrTips attrTips = _objectPool.GetObject("ui://CommonEx/AttrTips") as UI_AttrTips;
                // if (attrTips == null) return;
                //
                // attrTips.attrItem.attrName.text = values[0].ToString();//属性id
                // attrTips.attrItem.attValue.text = values[1].ToString();//属性值
                // int xPos = (int)values[2];//x偏移量
                //
                // _tipParent.AddChild(attrTips);
                // Vector2 globalPos = target.LocalToGlobal(Vector2.zero);
                // Vector2 localPos = GRoot.inst.GlobalToLocal(globalPos);
                // attrTips.xy = new Vector2(localPos.x + xPos, localPos.y - attrTips.height);
                // // Vector2 pos = GRoot.inst.GetPoupPosition(attrTips, target, PopupDirection.Up);
                // // attrTips.xy = new Vector2(pos.x+xPos, pos.y);//new Vector2(pos.x-120, pos.y)
                // attrTips?.onRemovedFromStage.Add(OnPopupClosed);
                // _popTips = attrTips;
                
                
                UI_AttrTips attrTips = _objectPool.GetObject("ui://CommonEx/AttrTips") as UI_AttrTips;
                if (attrTips == null) return;
                
                Vector2 globalPos = target.LocalToGlobal(Vector2.zero);
                Vector2 localPos = GRoot.inst.GlobalToLocal(globalPos);

                List<ClassicAttr> attrData = values[0] as List<ClassicAttr>;
                int xPos = (int)values[1];//x偏移量
                if (values.Length > 2)
                {
                    int dirIconXPos = (int)values[2];//箭头x偏移量
                    attrTips.dirIcon.x = dirIconXPos;
                }
                else
                {
                    attrTips.dirIcon.x = (attrTips.width / 2) - 30;
                }
                
                attrTips.attrList.itemRenderer = ClassicAttrRender;
                attrTips.attrList.data = attrData;
                attrTips.attrList.numItems = attrData.Count;
                attrTips.attrList.ResizeToFit();
                
                _tipParent.AddChild(attrTips);
                // Vector2 globalPos = target.LocalToGlobal(Vector2.zero);
                // Vector2 localPos = GRoot.inst.GlobalToLocal(globalPos);
                attrTips.xy = new Vector2(localPos.x + xPos, localPos.y - attrTips.height - (39 * (attrData.Count - 1)));
                attrTips?.onRemovedFromStage.Add(OnPopupClosed);
                _popTips = attrTips;

            }
            else
            {
                UI_CommonItemTips commonItem = _objectPool.GetObject("ui://CommonEx/CommonItemTips") as UI_CommonItemTips;
                if (commonItem == null) return;
                int itemId = (int) values[0];
                ulong itemGuid = 0;
                if (values.Length > 1)
                {
                    itemGuid = (ulong)values[1];
                }
                
                int quality;
                ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemId);
                quality = itemTypeUnit.Quality;
                
                // 是否为传承装备  类型14为传承装备
                if (itemGuid > 0 && itemTypeUnit.Type == 14)
                {
                    // 传承装备
                    EquipData equipData = EquipManager.Instance.GetNoWearLoreEquipByGuid(itemGuid);
                    if (equipData != null)
                    {
                        quality = equipData.quality;
                    }
                }
                
                commonItem.pName.text = ConfigUtils.GetTextById(itemTypeUnit.Name);
                commonItem.pName.qualityCtrl.selectedIndex = quality - 1;
                // commonItem.pContent.text = ConfigUtils.GetTextById(itemTypeUnit.Desc,itemTypeUnit.DescParam);
                if (itemTypeUnit.Type == 16)//圣物
                {
                    string value = (double.Parse(itemTypeUnit.DescParam)/100).ToString();
                    commonItem.pContent.text = ConfigUtils.GetTextById(itemTypeUnit.Desc,value);
                }
                else
                {
                    commonItem.pContent.text = ConfigUtils.GetTextById(itemTypeUnit.Desc,itemTypeUnit.DescParam);
                }
                
                commonItem.pLv.visible = false;
                ((UI_ItemCom) commonItem.item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
                ((UI_ItemCom) commonItem.item).ctrlQuality.selectedIndex = quality - 1;
                _tipParent.AddChild(commonItem);
                commonItem.xy = GRoot.inst.GetPoupPosition(commonItem, target, PopupDirection.Auto);
                commonItem?.onRemovedFromStage.Add(OnPopupClosed);
                _popTips = commonItem;
            }
        }

        private void ClassicAttrRender(int index, GObject item)
        {
            List<ClassicAttr> infoAttrs = item.parent.data as List<ClassicAttr>;
            string attrName = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(infoAttrs[index].AttrId).AttrName);
            string value = EquipManager.Instance.SetAttributeValue(infoAttrs[index].AttrId, infoAttrs[index].AttrVal, true);
            ((UI_AttrItem) item).attrName.text = attrName;
            ((UI_AttrItem) item).attValue.text = value;

        }

        private void AttrItem2Render(int index, GObject item)
        {
            ClassicInfo info = item.parent.data as ClassicInfo;
            string lastAttrValue = EquipManager.Instance.SetAttributeValue(info.maxAttrs[index].AttrId, info.maxAttrs[index].AttrVal,true);
            ((UI_ClassicMaxAttrItem) item).maxAttrLb.SetVar("value", lastAttrValue).SetVar("name",ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(info.maxAttrs[index].AttrId).AttrName)).FlushVars();
        }

        public void ClosePopupTip()
        {
            if (_tipParent == null) Init();
            _tipParent.RemoveChild(_popTips);
            _popTips = null;
        }

        private void OnPopupClosed(EventContext context)
        {
            GComponent tips = context.sender as GComponent;
            tips?.onRemovedFromStage.Remove(OnPopupClosed);
            _objectPool.ReturnObject(tips);
        }

        private void __stageTouchEnd(EventContext context)
        {
            DisplayObject mc = Stage.inst.touchTarget as DisplayObject;
            bool handled = false;
            while (mc != Stage.inst && mc != null)
            {
                if (mc.gOwner == _popTips)
                {
                    handled = true;
                    break;
                }

                if (mc.gOwner == _PopTarget)
                {
                    handled = true;
                    break;
                }
                
                mc = mc.parent;
            }
            
            if(!handled)
                ClosePopupTip();
        }


        public void ShowMessagePopup(string message,string txtLfetBtn = "",string txtRightBtn = "", Action<bool> callback = null)
        {
            UIManager.Instance.ShowUIPanel("MessageTips", message, txtLfetBtn, txtRightBtn, callback);
        }
    }
}