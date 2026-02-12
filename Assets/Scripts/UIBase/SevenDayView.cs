
using System.Collections;
using System.Collections.Generic;
using CommonEx;
using Config;
using Engine;
using FairyGUI;
using msg;
using SevenDay;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

public class SevenDayView : UIViewBase
{
    private UI_SevenDay SevenDay => this.main as UI_SevenDay;
    
    private List<SevenDayInfo> _sevenDayInfos = new List<SevenDayInfo>();
    List<ItemData> _rewardItemDatas = new List<ItemData>();
    private Coroutine _coTimeFlow;
    public SevenDayView()
    {
        this.name = "SevenDay";
        this.package = "SevenDay";
        this.component = "SevenDay";
        this.removePackage = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        SevenDayBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.SevenDay.closeBtn.onClick.Add(this.Hide);
        this.SevenDay.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.SevenDay.dayList.itemRenderer = SevenDayItemRender;
        this.SevenDay.dayList.itemProvider = ItemProvider;
        this.SevenDay.dayList.onClickItem.Add(this.OnClickToGetRw);
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_SEVENDAY_UPDATE, this.OnUpdateSevenDayInfo);
    }

    private void SevenDayItemRender(int index, GObject item)
    {
        SevenDayInfo sevenDay = _sevenDayInfos[index];

        if (sevenDay.sevenUnit.Count == 7)
        {
            ((UI_SevenBigItem) item).tag.selectedIndex = sevenDay.Tag;
            string[] rewardArr = sevenDay.sevenUnit.Reward.Split("|");
            _rewardItemDatas = new List<ItemData>();
            for (int i = 0; i < rewardArr.Length; i++)
            {
                string[] oneRewardArr = rewardArr[i].Split(",");
                ItemData itemData = new ItemData();
                itemData.id = int.Parse(oneRewardArr[0]);
                itemData.count = double.Parse(oneRewardArr[1]);
                _rewardItemDatas.Add(itemData);
            }
            ((UI_SevenBigItem) item).rwList.itemRenderer = RewardItemListRender;
            ((UI_SevenBigItem) item).rwList.numItems = _rewardItemDatas.Count;
        }
        else
        {
            ((UI_SevenItem) item).tag.selectedIndex = sevenDay.Tag;
            ((UI_SevenItem) item).day.selectedIndex = sevenDay.sevenUnit.Count - 1;
            string[] rwArr = sevenDay.sevenUnit.Reward.Split(",");
            ItemInfo((UI_ItemCom) ((UI_SevenItem) item).item, int.Parse(rwArr[0]), int.Parse(rwArr[1]));
        }
    }

    private void ItemInfo(GObject item, int id,double count)
    {
        ((UI_ItemCom) item).SetItemDataWithGuid(new ItemData(id, count), true);
        ((UI_ItemCom) item).onClick.Clear();
        ((UI_ItemCom) item).data = id;
        ((UI_ItemCom)item).onClick.Set(OnItemTips);
    }
    
    private void RewardItemListRender(int index, GObject item)
    {
        ItemInfo(item, _rewardItemDatas[index].id, _rewardItemDatas[index].count);
    }
    
    private void OnItemTips(EventContext context)
    {
        context.StopPropagation();
        int itemId = (int)((UI_ItemCom)context.sender).data;
        if (itemId != 0)
        {
            TipsManger.Instance.ShowPopupTip((UI_ItemCom)context.sender, Tipstype.None, itemId);
        }
    }

    private string ItemProvider(int index)
    {
        if (_sevenDayInfos[index].sevenUnit.Count == 7)
        {
            return "ui://1tqiwa7amg7g8";
        }

        return "ui://1tqiwa7amg7g7";
    }
    protected override void OnShow()
    {
        base.OnShow();
        // this.SevenDay.titleImg.url = UIResource.GetImageUrlWithLang("n3","SevenDay");
        StartTimeLeftCd();
        OnUpdateSevenDayInfo();
    }

    public void StartTimeLeftCd()
    {
        if (null != _coTimeFlow)
            GameManager.Instance.StopCoroutine(_coTimeFlow);

        int totalSecond = ServerTimeManager.Instance.GetToZeroLeftTime();
        if (totalSecond > 0)
        {
            this.SevenDay.timeLeft.SetVar("value", StringUtils.GetTimeString((int)totalSecond)).FlushVars();
            _coTimeFlow = GameManager.Instance.StartCoroutine(TimeFlow());
        }
        
            
    }
    
    protected override void OnHide()
    {
        base.OnHide();
        if (null != _coTimeFlow)
            GameManager.Instance.StopCoroutine(_coTimeFlow);
        _coTimeFlow = null;
    }

    IEnumerator TimeFlow()
    {
        while (true)
        {
            int totalSecond = ServerTimeManager.Instance.GetToZeroLeftTime();
            if (totalSecond > 0)
            {
                this.SevenDay.timeLeft.SetVar("value", StringUtils.GetTimeString((int)totalSecond)).FlushVars();
            }

            yield return GameManager.Instance.waitSec1;
        }
    }
    
    private void OnUpdateSevenDayInfo()
    {
        _sevenDayInfos = ActivityManager.Instance.GetSevenDayInfos();
        this.SevenDay.dayList.numItems = 7;
        bool allSign = ActivityManager.Instance.HasSignAllSevenDay();
        if (allSign)
        {
            if (null != _coTimeFlow)
                GameManager.Instance.StopCoroutine(_coTimeFlow);
            this.SevenDay.timeLeft.text = ConfigUtils.GetStringByKey(10166);
        }
    }

    private void OnClickToGetRw(EventContext context)
    {
        var index = this.SevenDay.dayList.GetChildIndex((GObject) context.data);
        SevenDayInfo sevenDayInfo = _sevenDayInfos[index];
        if (sevenDayInfo.Tag == 1)
        {
            var builder = NewPlayerSignInClaimAward_CS.CreateBuilder();
            builder.Id = (uint) sevenDayInfo.sevenUnit.DailyCheckId;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_NewPlayerSignInClaimAward_CS, builder.Build());
        }else if (sevenDayInfo.Tag == 0)
        {
            UIManager.Instance.ToastByKey(10167);
        }else if (sevenDayInfo.Tag == 2)
        {
            UIManager.Instance.ToastByKey(5060);
        }

    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_SEVENDAY_UPDATE, this.OnUpdateSevenDayInfo);
    }
}
