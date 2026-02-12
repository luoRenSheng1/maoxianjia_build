
using System;
using System.Collections.Generic;
using System.Linq;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Passport;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

public class PassportView : UIViewBase
{
    private UI_Passport Passport => this.main as UI_Passport;
    private PassPortInfo _passPortInfo;
    private ConfigCommonUnit _commonUnit700;
    private List<ConfigPassTaskUnit> _passTaskUnits;
    
    public PassportView()
    {
        this.name = "Passport";
        this.package = "Passport";
        this.component = "Passport";
    }

    public override void BindAll()
    {
        base.BindAll();
        PassportBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.Passport.closeBtn.onClick.Add(this.Hide);
        this.Passport.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.Passport.advanceBtn.onClick.Add(this.OnClickAdvanceBtn);
        this.Passport.Passport.passportList.itemRenderer = PassportListItemRender;
        this.Passport.Passport.passportList.itemProvider = ItemProvider;
        this.Passport.Passport.passportList.SetVirtual();
        
        this.Passport.tabCtrl.onChanged.Add(this.OnTabCtrlChange);

        _commonUnit700 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(700);

        this.Passport.PassTask.passtaskList.itemRenderer = PassTaskListItemRender;
        _passTaskUnits = ConfigDataGroup.GetInstance<ConfigPassTask>().Data.Values.ToList();
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_PASSPORT, this.OnUpdatePassport);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_PASSPORT_UNLOCK_ADVANCE, this.OnUpdateUnlockAdvancePassport);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PASSPORT_GetSubTask, this.UpdatePassTaskInfo);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_PASSPORT, this.OnUpdatePassport);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_PASSPORT_UNLOCK_ADVANCE, this.OnUpdateUnlockAdvancePassport);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PASSPORT_GetSubTask, this.UpdatePassTaskInfo);
    }

    protected override void OnShow()
    {
        base.OnShow();
        OnUpdatePassport();
    }

    private void OnTabCtrlChange()
    {
        int index = this.Passport.tabCtrl.selectedIndex;
        switch (index)
        {
            case 0:
                OnUpdatePassport();
                break;
            case 1:
                UpdatePassTaskInfo();
                break;
        }
    }

    private void UpdatePassTaskInfo()
    {
        _passTaskUnits.Sort((a, b) =>
        {
            //可领取>已完成未解锁>未完成>未完成未解锁>已领取
            int unlockTier = ActivityManager.Instance.GetPassPortInfo().UnlockTier;
            int result = 0;
            PassPortSubTask subTaskA = ActivityManager.Instance.GetPassSubTask(a.Id, a.CD);
            PassPortSubTask subTaskB = ActivityManager.Instance.GetPassSubTask(b.Id, b.CD);
            int aR = subTaskA == null ? 3 : subTaskA.HasGetReward ? 4 : (subTaskA.Progress >= ulong.Parse(a.Param) ? (unlockTier>=a.PayType ? 1 : 2) : 3);
            int bR = subTaskB == null ? 3 : subTaskB.HasGetReward ? 4 : (subTaskB.Progress >= ulong.Parse(b.Param) ? (unlockTier>=b.PayType ? 1 : 2) : 3);
            result = aR > bR ? 1 : (aR == bR ? 0 : -1);
            if(result == 0)
                result = a.Id > b.Id ? 1 : -1;
            return result;
        });
        this.Passport.PassTask.passtaskList.numItems = _passTaskUnits.Count;
        this.Passport.PassTask.passtaskList.ScrollToView(0);
    }
    
    
    private void OnUpdatePassport()
    {
        _passPortInfo = ActivityManager.Instance.GetPassPortInfo();
        this.Passport.Passport.passportList.numItems = _passPortInfo.PassGiftUnits.Count + 1;

        string[] advanceArr = _passPortInfo.PassGiftUnits[1].FancyGift.Split(',');
        // string[] advanceArr = _passPortInfo.PassGiftUnits[0].FancyGift.Split(',');
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(advanceArr[0]));
        if (itemTypeUnit.Type == 3)
        {
           string strResName = ConfigUtils.GetHeroModelPathByID(itemTypeUnit.Param);
           Utils.SetSpineModelOnFGUI(this.Passport.spine, strResName, 140f, "idle");
        }

        // this.Passport.titleLb.text = itemTypeUnit.Name;
        this.Passport.titleLb.text = ConfigUtils.GetTextById(itemTypeUnit.Name);
        int totalSecond = (int)(_passPortInfo.EndTime - ServerTimeManager.Instance.CurServerTime);
        this.Passport.timeLb.text = StringUtils.GetTimeString2(totalSecond);

        this.Passport.expBar.min = 0;
        this.Passport.expBar.max = ActivityManager.Instance.GetPassportBoxMaxExp();
        this.Passport.expBar.value = _passPortInfo.PassPortLv == ActivityManager.Instance.GetPassportMaxLv() ? ActivityManager.Instance.GetPassportBoxMaxExp() : _passPortInfo.PassPortExp;
        this.Passport.passLvLb.text = _passPortInfo.PassPortLv.ToString();

        if (this._passPortInfo.UnlockTier == 0)
        {
            this.Passport.advanceBtn.visible = true;
            this.Passport.Passport.suo.visible = true;
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(int.Parse(_commonUnit700.Param3));
            if (payListUnit != null)
            {
                ((UI_AdvancePassportBtn) this.Passport.advanceBtn).moneyType.selectedIndex = payListUnit.MoneyType-1;
                double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
                ((UI_AdvancePassportBtn) this.Passport.advanceBtn).moneyLb1.SetVar("value", price.ToString("f2")).FlushVars();
                ((UI_AdvancePassportBtn) this.Passport.advanceBtn).moneyLb2.SetVar("value", price.ToString("f2")).FlushVars();
            }
            
        }
        else
        {
            this.Passport.advanceBtn.visible = false;
            this.Passport.Passport.suo.visible = false;
        }

        this.Passport.leftBtn.reddot.visible = ActivityManager.Instance.HasPassportRewardGet();
        this.Passport.rightBtn.reddot.visible = ActivityManager.Instance.HasPassportTaskRewardGet();

        int scrollNormalIndex = _passPortInfo.PassGiftUnits.Count;
        bool hasReward = false;
        for (int i = 0; i < _passPortInfo.PassGiftUnits.Count; i++)
        {
            bool isNormalGet = _passPortInfo.PassPortLv >= _passPortInfo.PassGiftUnits[i].Level;
            if (isNormalGet)
                isNormalGet =
                    !ActivityManager.Instance.IsGetPassportReward(_passPortInfo.PassGiftUnits[i].Level, false);
            if (isNormalGet)
            {
                scrollNormalIndex = i;
                hasReward = true;
                break;
            }
        }
        for (int i = 0; i < _passPortInfo.PassGiftUnits.Count; i++)
        {
            bool isSuperGet = _passPortInfo.UnlockTier >= 1 && _passPortInfo.PassPortLv >= _passPortInfo.PassGiftUnits[i].Level;
            if(isSuperGet)
                isSuperGet = !ActivityManager.Instance.IsGetPassportReward(_passPortInfo.PassGiftUnits[i].Level, true);
            if (isSuperGet)
            {
                if (scrollNormalIndex > i)
                    scrollNormalIndex = i;
                hasReward = true;
                break;
            }
        }

        if (!hasReward)
        {
            int scrollIndex = _passPortInfo.PassPortLv;
            this.Passport.Passport.passportList.ScrollToView(scrollIndex, true, true);
        }
        else
        {
            this.Passport.Passport.passportList.ScrollToView(Mathf.Max(0 ,scrollNormalIndex), true, true);
        }
        

    }

    private void OnUpdateUnlockAdvancePassport()
    {
        OnUpdatePassport();
        UpdatePassTaskInfo();
    }
    
    private string ItemProvider(int index)
    {
        if (_passPortInfo.PassGiftUnits.Count == index)
        {
            return "ui://Passport/BoxItem";
        }
        
        return "ui://Passport/PassportItem";
    }

    private void PassportListItemRender(int index, GObject item)
    {
        if (index >= _passPortInfo.PassGiftUnits.Count)
        {
            ((UI_BoxItem) item).expBar.max = ActivityManager.Instance.GetPassportBoxMaxExp();
            ((UI_BoxItem) item).expBar.min = 0;
            ((UI_BoxItem) item).expBar.value = _passPortInfo.PassPortLv>= ActivityManager.Instance.GetPassportMaxLv() ? _passPortInfo.PassPortExp : 0;
            ((UI_BoxItem) item).tempExp.max = ActivityManager.Instance.GetPassportBoxMaxExp();
            ((UI_BoxItem) item).tempExp.min = 0;
            ((UI_BoxItem) item).tempExp.value = _passPortInfo.PassPortLv>= ActivityManager.Instance.GetPassportMaxLv() ? ActivityManager.Instance.GetPassportBoxMaxExp() : 0;
            ((UI_BoxItem) item).boxDesc.SetVar("lv",ActivityManager.Instance.GetPassportMaxLv().ToString()).SetVar("exp", ActivityManager.Instance.GetPassportBoxMaxExp().ToString()).SetVar("total", ActivityManager.Instance.GetPassPortBoxMaxCnt().ToString()).FlushVars();
            ((UI_BoxItem) item).boxDetail.onClick.Add(this.OnClickBoxTips);
            ((UI_BoxItem) item).boxGetBtn.onClick.Add(this.OnClickGetBoxRewardBtn);
            ((UI_BoxItem) item).boxGetLb.SetVar("cur", (ActivityManager.Instance.GetPassPortBoxMaxCnt()-_passPortInfo.ClaimedCounterAfterTopLevel).ToString()).SetVar("total", ActivityManager.Instance.GetPassPortBoxMaxCnt().ToString()).FlushVars();
            ((UI_BoxItem) item).rewardCtrl.selectedIndex = _passPortInfo.CounterAfterTopLevel > 0 ? 1 : (ActivityManager.Instance.GetPassPortBoxMaxCnt() - _passPortInfo.ClaimedCounterAfterTopLevel == 0 ? 2 : 0);
            ((UI_RedDotComponent) ((UI_BoxItem) item).redDot).countType.selectedIndex =
                _passPortInfo.CounterAfterTopLevel > 1 ? 1 : 0;
            ((UI_RedDotComponent) ((UI_BoxItem) item).redDot).countText.text =
                _passPortInfo.CounterAfterTopLevel.ToString();
        }
        else
        {
            if (index == 0)
            {
                //适配UI效果
                ((UI_PassportItem) item).expBar.max = 1;
                ((UI_PassportItem) item).expBar.min = 0;
                ((UI_PassportItem) item).expBar.value = 1;
                ((UI_PassportItem) item).normalItem.visible = false;
                ((UI_PassportItem) item).superItem.visible = false;
                // ((UI_PassportItem) item).passIcon.visible = false;
                ((UI_PassportItem) item).passLv.visible = false;
            }
            else
            {
                ((UI_PassportItem) item).normalItem.visible = true;
                ((UI_PassportItem) item).superItem.visible = true;
                ((UI_PassportItem) item).passIcon.visible = true;
                ((UI_PassportItem) item).passLv.visible = true;
                
                ((UI_PassportItem) item).expBar.max = _passPortInfo.PassGiftUnits[index].Score;
                ((UI_PassportItem) item).expBar.min = 0;
                if (_passPortInfo.PassPortLv >= _passPortInfo.PassGiftUnits[index].Level)
                {
                    if (_passPortInfo.PassGiftUnits[index].Level == ActivityManager.Instance.GetPassportMaxLv())
                    {
                        ((UI_PassportItem) item).expBar.value = _passPortInfo.PassGiftUnits[index-1].Score;
                    }
                    else
                    {
                        ((UI_PassportItem) item).expBar.value = _passPortInfo.PassGiftUnits[index].Score;
                    }
                }
                else if((_passPortInfo.PassPortLv+1) == _passPortInfo.PassGiftUnits[index].Level)
                {
                    ((UI_PassportItem) item).expBar.value = _passPortInfo.PassPortExp;
                }else
                {
                    ((UI_PassportItem) item).expBar.value = 0;
                }

                ((UI_PassportItem) item).passLv.text = _passPortInfo.PassGiftUnits[index].Level.ToString();
                string[] normalArr = _passPortInfo.PassGiftUnits[index].Gift.Split(',');
                ((UI_ItemCom)((UI_PassportItem)item).normalItem.item).SetItemDataWithGuid(new ItemData(int.Parse(normalArr[0]), int.Parse(normalArr[1])), true);
                bool isNormalGet = _passPortInfo.PassPortLv >= _passPortInfo.PassGiftUnits[index].Level;
                ((UI_PassportItem) item).normalItem.rewardCtrl.selectedIndex = !isNormalGet ? 0 : ActivityManager.Instance.IsGetPassportReward(_passPortInfo.PassGiftUnits[index].Level, false) ? 2 : 1;
            
                string[] advanceArr = _passPortInfo.PassGiftUnits[index].FancyGift.Split(',');
                if (index == 0)
                {
                    ((UI_ItemCom)((UI_PassportItem)item).superItem.item).SetItemDataWithGuid(new ItemData(int.Parse(advanceArr[0]), int.Parse(advanceArr[1])), true);
                    ((UI_ItemCom)((UI_PassportItem)item).superItem.item).onClick.Clear();
                    ((UI_ItemCom) ((UI_PassportItem) item).superItem.item).data = advanceArr[0];
                    ((UI_ItemCom)((UI_PassportItem)item).superItem.item).onClick.Add(this.OnClickOpenHeroDetail);
                }
                else
                {
                    ((UI_ItemCom)((UI_PassportItem)item).superItem.item).SetItemDataWithGuid(new ItemData(int.Parse(advanceArr[0]), int.Parse(advanceArr[1])), true);
                }

                ((UI_PassportItem) item).superItem.lockCtrl.selectedIndex = _passPortInfo.UnlockTier >= 1 ? 0 : 1;
                bool isSuperGet = _passPortInfo.PassPortLv >= _passPortInfo.PassGiftUnits[index].Level;
                ((UI_PassportItem) item).superItem.rewardCtrl.selectedIndex = !isSuperGet ? 0 : ActivityManager.Instance.IsGetPassportReward(_passPortInfo.PassGiftUnits[index].Level, true) ? 2 : 1;
                ((UI_PassportItem) item).superItem.lockCtrl.selectedIndex = _passPortInfo.UnlockTier >= 1 ? 0 : 1;
                ((GButton) ((UI_PassportItem) item).normalItem.getBtn).data = _passPortInfo.PassGiftUnits[index].Level;
                ((GButton)((UI_PassportItem)item).normalItem.getBtn).onClick.Add(this.OnClickGetNormalReward);
                ((GButton) ((UI_PassportItem) item).superItem.getBtn).data = _passPortInfo.PassGiftUnits[index].Level;
                ((GButton)((UI_PassportItem)item).superItem.getBtn).onClick.Add(this.OnClickGetAdvanceReward);
            }
            
            // ((UI_PassportItem) item).expBar.max = _passPortInfo.PassGiftUnits[index].Score;
            // ((UI_PassportItem) item).expBar.min = 0;
            // if (_passPortInfo.PassPortLv >= _passPortInfo.PassGiftUnits[index].Level)
            // {
            //     if (_passPortInfo.PassGiftUnits[index].Level == ActivityManager.Instance.GetPassportMaxLv())
            //     {
            //         ((UI_PassportItem) item).expBar.value = _passPortInfo.PassGiftUnits[index-1].Score;
            //     }
            //     else
            //     {
            //         ((UI_PassportItem) item).expBar.value = _passPortInfo.PassGiftUnits[index].Score;
            //     }
            // }
            // else if((_passPortInfo.PassPortLv+1) == _passPortInfo.PassGiftUnits[index].Level)
            // {
            //     ((UI_PassportItem) item).expBar.value = _passPortInfo.PassPortExp;
            // }else
            // {
            //     ((UI_PassportItem) item).expBar.value = 0;
            // }
            //
            // ((UI_PassportItem) item).passLv.text = _passPortInfo.PassGiftUnits[index].Level.ToString();
            // string[] normalArr = _passPortInfo.PassGiftUnits[index].Gift.Split(',');
            // ((UI_ItemCom)((UI_PassportItem)item).normalItem.item).SetItemDataWithGuid(new ItemData(int.Parse(normalArr[0]), int.Parse(normalArr[1])), true);
            // bool isNormalGet = _passPortInfo.PassPortLv >= _passPortInfo.PassGiftUnits[index].Level;
            // ((UI_PassportItem) item).normalItem.rewardCtrl.selectedIndex = !isNormalGet ? 0 : ActivityManager.Instance.IsGetPassportReward(_passPortInfo.PassGiftUnits[index].Level, false) ? 2 : 1;
            //
            // string[] advanceArr = _passPortInfo.PassGiftUnits[index].FancyGift.Split(',');
            // if (index == 0)
            // {
            //     ((UI_ItemCom)((UI_PassportItem)item).superItem.item).SetItemDataWithGuid(new ItemData(int.Parse(advanceArr[0]), int.Parse(advanceArr[1])), true);
            //     ((UI_ItemCom)((UI_PassportItem)item).superItem.item).onClick.Clear();
            //     ((UI_ItemCom) ((UI_PassportItem) item).superItem.item).data = advanceArr[0];
            //     ((UI_ItemCom)((UI_PassportItem)item).superItem.item).onClick.Add(this.OnClickOpenHeroDetail);
            // }
            // else
            // {
            //     ((UI_ItemCom)((UI_PassportItem)item).superItem.item).SetItemDataWithGuid(new ItemData(int.Parse(advanceArr[0]), int.Parse(advanceArr[1])), true);
            // }
            //
            // ((UI_PassportItem) item).superItem.lockCtrl.selectedIndex = _passPortInfo.UnlockTier >= 1 ? 0 : 1;
            // bool isSuperGet = _passPortInfo.PassPortLv >= _passPortInfo.PassGiftUnits[index].Level;
            // ((UI_PassportItem) item).superItem.rewardCtrl.selectedIndex = !isSuperGet ? 0 : ActivityManager.Instance.IsGetPassportReward(_passPortInfo.PassGiftUnits[index].Level, true) ? 2 : 1;
            // ((UI_PassportItem) item).superItem.lockCtrl.selectedIndex = _passPortInfo.UnlockTier >= 1 ? 0 : 1;
            // ((GButton) ((UI_PassportItem) item).normalItem.getBtn).data = _passPortInfo.PassGiftUnits[index].Level;
            // ((GButton)((UI_PassportItem)item).normalItem.getBtn).onClick.Add(this.OnClickGetNormalReward);
            // ((GButton) ((UI_PassportItem) item).superItem.getBtn).data = _passPortInfo.PassGiftUnits[index].Level;
            // ((GButton)((UI_PassportItem)item).superItem.getBtn).onClick.Add(this.OnClickGetAdvanceReward);
        }
    }

    private void OnClickBoxTips(EventContext context)
    {

        int itemId = 0;
        if (_passPortInfo.UnlockTier >= 1)
        {
            itemId = int.Parse(_commonUnit700.Param5.Split(',')[0]);
        }
        else
        {
            itemId = int.Parse(_commonUnit700.Param4.Split(',')[0]);
        }
        if (itemId != 0)
        {
            TipsManger.Instance.ShowPopupTip((UIGLoader)context.sender, Tipstype.None, itemId);
        }
    }

    private void OnClickGetBoxRewardBtn()
    {
        var builder = ClaimPassTaskAward_CS.CreateBuilder();
        builder.IsAll = false;
        builder.IsOverToplevel = true;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimPassTaskAward_CS, builder.Build());
    }

    private void OnClickOpenHeroDetail(EventContext context)
    {
        string advanceItemId = (string) (context.sender as UI_ItemCom).data;
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(advanceItemId));
        if (itemTypeUnit.Type == 3)
        {
            UIManager.Instance.ShowUIPanel("RolePassportDetail", itemTypeUnit.Param);
        }
    }

    private void OnClickGetNormalReward(EventContext context)
    {
        int level = (int) (context.sender as GButton).data;
        var builder = ClaimPassTaskAward_CS.CreateBuilder();
        builder.IsAll = false;
        builder.IsOverToplevel = false;
        builder.Level = level;
        builder.UnlockTier = 0;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimPassTaskAward_CS, builder.Build());
    }
    
    private void OnClickGetAdvanceReward(EventContext context)
    {
        int level = (int) (context.sender as GButton).data;
        var builder = ClaimPassTaskAward_CS.CreateBuilder();
        builder.IsAll = false;
        builder.IsOverToplevel = false;
        builder.Level = level;
        builder.UnlockTier = 1;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimPassTaskAward_CS, builder.Build());
    }

    private void PassTaskListItemRender(int index, GObject item)
    {
        ConfigPassTaskUnit taskUnit = _passTaskUnits[index];
        ((UI_ItemCom) ((UI_PassportTaskItem) item).item).txtLv.text = taskUnit.ActivityLevel.ToString();
        ((UI_PassportTaskItem) item).advanceCtrl.selectedIndex = _passPortInfo.UnlockTier >= 1 || (taskUnit.PayType == 0) ? 0 : 1;
        // ((UI_PassportTaskItem) item).taskNameLb.text = taskUnit.Name;
        ((UI_PassportTaskItem) item).taskNameLb.text = ConfigUtils.GetTextById(taskUnit.Name);
        ((UI_PassportTaskItem) item).taskType.selectedIndex = taskUnit.CD - 1;
        ((UI_PassportTaskItem) item).getRwBtn.data = taskUnit.Id;
        ((UI_PassportTaskItem) item).getRwBtn.onClick.Add(this.OnClickGetPassSubTask);

        if (taskUnit.CD == 1)
        {
            int totalSecond = ServerTimeManager.Instance.GetToZeroLeftTime();
            ((UI_PassportTaskItem) item).timeLb.text = StringUtils.GetTimeString2(totalSecond);
        }else if (taskUnit.CD == 2)
        {
            int totalSecond = (int)(Utils.GetSundayEndTime() - (long) ServerTimeManager.Instance.CurServerTime);
            ((UI_PassportTaskItem) item).timeLb.text = StringUtils.GetTimeString2(totalSecond);
        }

        ((UI_PassportTaskItem) item).expBar.min = 0;
        ((UI_PassportTaskItem) item).expBar.max = int.Parse(taskUnit.Param);
        PassPortSubTask subTask = ActivityManager.Instance.GetPassSubTask(taskUnit.Id, taskUnit.CD);
        if (subTask != null)
        {
            ulong progress = subTask.Progress;
            if (taskUnit.Type == 34) //在线时长
            {
                ulong timeDelta = ServerTimeManager.Instance.CurServerTime - ServerTimeManager.Instance.LoginTimeStamp;
                progress += timeDelta;
            }

            progress = Math.Min(progress, ulong.Parse(taskUnit.Param));
            ((UI_PassportTaskItem) item).expBar.value = progress;
            ((UI_PassportTaskItem) item).rewardCtrl.selectedIndex =
                subTask.HasGetReward ? 2 : (progress >= ulong.Parse(taskUnit.Param) ? 1 : 0);

            if (progress >= ulong.Parse(taskUnit.Param))
            {
                ((UI_PassportTaskItem) item).expBar.maxCtrl.selectedIndex = 1;
            }
            else
            {
                ((UI_PassportTaskItem) item).expBar.maxCtrl.selectedIndex = 0;
            }
        }
        else
        {
            ((UI_PassportTaskItem) item).expBar.value = 0;
            ((UI_PassportTaskItem) item).rewardCtrl.selectedIndex = false ? 2 : 0 >= ulong.Parse(taskUnit.Param) ? 1 : 0;
        }

        // ((UI_PassportTaskItem) item).ywcImg.url = UIResource.GetImageUrlWithLang("ywc", "Common");

    }

    private void OnClickGetPassSubTask(EventContext context)
    {
        int subTaskId = (int) (context.sender as GButton).data;
        var builder = ClaimPassSubTaskExp_CS.CreateBuilder();
        builder.SubTaskId = subTaskId;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimPassSubTaskExp_CS, builder.Build());

        Vector2 pos = ((context.sender as GButton).parent as UI_PassportTaskItem).item.LocalToGlobal(Vector2.zero);
        UIItemsGain itemsGain2 = UIGainBasePool.CreateUIGainBase();//new UIItemsGain();
        itemsGain2.ApplyItemSourceToDestinationEx(this.Passport.txzIcon.asCom, "ui://Passport/txzjinagyan");
        itemsGain2.StartItemFly(pos, 5);
    }

    private void OnClickAdvanceBtn()
    {
        UIManager.Instance.ShowUIPanel("PassportDetail");
    }
    
}
