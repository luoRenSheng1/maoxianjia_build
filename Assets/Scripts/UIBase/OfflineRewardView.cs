using CommonEx;
using Config;
using Engine;
using FairyGUI;
using msg;
using Offline;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

public class OfflineRewardView : UIViewBase
{
    private UI_OfflineReward OfflineReward => this.main as UI_OfflineReward;

    private OfflineRewardData _offlineRewardData;

    private int _type;
    private float _timer = 0;

    public OfflineRewardView()
    {
        this.type = UIType.Top;
        this.name = "OfflineReward";
        this.package = "Offline";
        this.component = "OfflineReward";
    }

    public override void BindAll()
    {
        base.BindAll();
        OfflineBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _type = (int) values[0];
        _offlineRewardData = values[1] as OfflineRewardData;
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.OfflineReward.getRw.onClick.Add(this.OnClickGetRwBtn);
        this.OfflineReward.adGetRw.onClick.Add(this.OnClickAdGetRwBtn);

        this.OfflineReward.itemList.itemRenderer = ItemRewardListRender;
        
        EventDispatcher.GameWorld.Regist<int>(EventDefine.EVENT_UPDATE_ONLINEAWARD_lobby, this.UpdateOnlineReward);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist<int>(EventDefine.EVENT_UPDATE_ONLINEAWARD_lobby, this.UpdateOnlineReward);
    }

    protected override void OnShow()
    {
        base.OnShow();
        if (GuideManager.Instance.IsShowGuiding)
        {
            UIManager.Instance.CloseUIPanel("OfflineReward");
            return;
        }

        this.OfflineReward.ctrl.selectedIndex = _type;
        _offlineRewardData.ItemDatas.Sort((a, b) =>
        {
            ConfigItemTypeUnit itemTypeUnit1 = ConfigUtils.GetConfigItemTypeUnitById(a.id);
            ConfigItemTypeUnit itemTypeUnit2 = ConfigUtils.GetConfigItemTypeUnitById(a.id);
            int result = itemTypeUnit1.Quality > itemTypeUnit2.Quality ? -1 : (itemTypeUnit1.Quality == itemTypeUnit2.Quality ? 0 : 1);
            if (result == 0)
                result = a.id > b.id ? -1 : 1;
            return result;
        });
        if (_type == 0)
        {
            this.OfflineReward.goldLb.text = StringUtils.FormatCurrency(_offlineRewardData.gold);
            this.OfflineReward.keyLb.text = _offlineRewardData.boxKeyNum.ToString();
            this.OfflineReward.itemList.numItems = _offlineRewardData.ItemDatas.Count;
            this.OfflineReward.timeLb.text = StringUtils.GetTimeString((int) DataManager.Instance.GetRoleData().OfflineTotalSecond);
        }
        else if(_type == 1)
        {
            this.OfflineReward.goldLb.text =StringUtils.FormatCurrency(_offlineRewardData.gold);
            this.OfflineReward.keyLb.text = _offlineRewardData.boxKeyNum.ToString();
            this.OfflineReward.itemList.numItems = _offlineRewardData.ItemDatas.Count;
            this.OfflineReward.timeLb.text = StringUtils.GetTimeString((int)DataManager.Instance.GetRoleData().OnlineAwardCdTime);
            
            this.OfflineReward.redPoint1.visible = _offlineRewardData.gold > 0;
            int onlineFreeTime = AdManager.Instance.GetAdFreeTimes((int)ePlayerAttrID.ePlayerAttrID_OnlineAwardFreeTimes);
            this.OfflineReward.redPoint2.visible = onlineFreeTime > 0;
        }

        _timer = 0;

        if (JumpManager.Instance.isWatchAd)
        {
            var globalPos = this.OfflineReward.adGetRw.LocalToGlobal(Vector2.zero);
            JumpManager.Instance.ShowFinger(JumpTypeEnum.WatchAd, this.OfflineReward.adGetRw);
        }
    }

    private void ItemRewardListRender(int index, GObject item)
    {
        ItemData itemData = _offlineRewardData.ItemDatas[index];
        ((UI_ItemCom) item).SetItemDataWithGuid(itemData, true);
    }

    private void OnClickGetRwBtn()
    {
        if (_type == 0)
        {
            Hide();
            // UIManager.Instance.ShowUIPanel("LotteryTask", false, 0);
        }
        else
        {
            GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralOnlineGainGoldSE);
            SetVisible(false);
            if (_offlineRewardData.gold == 0)
            {
                return;
            }
            var builder = ClaimOnlineAward_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimOnlineAward_CS, builder.Build());
        }
    }

    private void OnClickAdGetRwBtn()
    {
        //UIManager.Instance.ToastByKey(206);
        if (_type == 0)//离线
        {
            int offFreeTime = AdManager.Instance.GetAdFreeTimes((int)ePlayerAttrID.ePlayerAttrID_OfflineAwardFreeTimes);
            if (offFreeTime > 0)
            {
                UIManager.Instance.WatchAd(() =>
                {
                    var builder = ClaimStageAwardByAD_CS.CreateBuilder();
                    builder.IsOnline = false;
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimStageAwardByAD_CS, builder.Build());
                });

            }
            else
            {
                UIManager.Instance.ToastByKey(10179);
            }
        }
        else if (_type == 1) //在线
        {
            int onlineFreeTime = AdManager.Instance.GetAdFreeTimes((int)ePlayerAttrID.ePlayerAttrID_OnlineAwardFreeTimes);
            if (onlineFreeTime > 0)
            {
                UIManager.Instance.WatchAd(() =>
                {
                    var builder = ClaimStageAwardByAD_CS.CreateBuilder();
                    builder.IsOnline = true;
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimStageAwardByAD_CS,
                        builder.Build());
                });

            }
            else
            {
                this.OfflineReward.redPoint2.visible = false;
                UIManager.Instance.ToastByKey(10179);
            }
        }
    }
    
    private void UpdateOnlineReward(int onlineTime)
    {
        if (IsOnStage() && IsShow())
        {
            if(_type !=1) return;
            this.OfflineReward.timeLb.text = StringUtils.GetTimeString((int)DataManager.Instance.GetRoleData().OnlineAwardCdTime);
            this.OfflineReward.redPoint1.visible = _offlineRewardData.gold > 0;
            int onlineFreeTime = AdManager.Instance.GetAdFreeTimes((int)ePlayerAttrID.ePlayerAttrID_OnlineAwardFreeTimes);
            this.OfflineReward.redPoint2.visible = onlineFreeTime > 0;
        }

    }
}
