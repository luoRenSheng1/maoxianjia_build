
using System.Collections.Generic;
using BestHTTP.Extensions;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using Lobby;
using msg;
using PVPMap;
using EventDispatcher = EngineBase.EventDispatcher;

public class RankingListMainView : UIViewBase
{
    private UI_RankingListMain RankingListMain => this.main as UI_RankingListMain;

    private List<ConfigFirstRewardUnit> _PvpRewardUnits;
    private List<ItemData> _itemDatas = new List<ItemData>();
    private ConfigCommonUnit _common15;
    public RankingListMainView()
    {
        this.name = "RankingListMain";
        this.package = "PVPMap";
        this.component = "RankingListMain";
        this.removePackage = true;
        this.safeAreaInset = true;
    }
    
    public override void BindAll()
    {
        base.BindAll();
        PVPMapBinder.BindAll();
    }
    
    protected override void OnInit()
    {
        base.OnInit();
        _PvpRewardUnits = ConfigUtils.GetPvpRewardUnits();
        this.RankingListMain.rankingList.itemRenderer = PVPListItemRender;
        this.RankingListMain.rankingList.SetVirtual();
        // this.RankingListMain.closeBtn.onClick.Add(this.Hide);
        this.RankingListMain.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.RankingListMain.reportBtn.onClick.Add(this.OnClickReportBtn);
        this.RankingListMain.addNumBtn.onClick.Add(this.OnClickAddNumBtn);
        _common15 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(15);
        this.RankingListMain.helpBtn.onClick.Add(this.OnClickHelpBtn);
        this.RankingListMain.myselfItem.onClick.Add(this.OnClickMySelfItem);
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PVP_UPDATE, this.OnUpdatePvpRank);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PVP_FIGHT_COUNT_UPDATE, this.UpdateFightLeftCnt);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PVP_GET_REWARD_UPDATE, this.UpdateMyRankItem);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PVP_UPDATE, this.OnUpdatePvpRank);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PVP_FIGHT_COUNT_UPDATE, this.UpdateFightLeftCnt);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PVP_GET_REWARD_UPDATE, this.UpdateMyRankItem);

        PlayLobbyBGM();
    }

    protected override void OnShow()
    {
        base.OnShow();
        GameManager.Instance.SoundManager.PlayMusic((int)SoundType.PVPBGM);
        OnUpdatePvpRank();
        UpdateFightLeftCnt();
        this.RankingListMain.img.url = UIResource.GetImageUrlWithLang("txdyds", "PVPMap");
    }

    protected override void OnHide()
    {
        base.OnHide();
        Utils.ClearSpineModelOnFGUI(this.RankingListMain.spine1);
        Utils.ClearSpineModelOnFGUI(this.RankingListMain.spine2);
        Utils.ClearSpineModelOnFGUI(this.RankingListMain.spine3);
        PlayLobbyBGM();
    }

    private void OnUpdatePvpRank()
    {
        this.RankingListMain.rankingList.numItems = _PvpRewardUnits.Count;
        UpdateRankModel();
        this.RankingListMain.rankingList.ScrollToView(0);
        UpdateMyRankItem();
    }

    private void UpdateRankModel()
    {
        PvpRankVo rankVo1 = PvpRankDataManager.Instance.ThisRankHasPlayer(1);
        if (rankVo1 != null)
        {
            string model1 = ConfigUtils.GetHeroModelPathByID(rankVo1.HeroId);
            Utils.SetSpineModelOnFGUI(this.RankingListMain.spine1, model1, 110f);
            this.RankingListMain.num1Lb.text = rankVo1.PlayerName;
        }
        else
        {
            this.RankingListMain.num1Lb.text = ConfigUtils.GetStringByKey(10189);
            Utils.ClearSpineModelOnFGUI(this.RankingListMain.spine1);
        }

        PvpRankVo rankVo2 = PvpRankDataManager.Instance.ThisRankHasPlayer(2);
        if (rankVo2 != null)
        {
            string model2 = ConfigUtils.GetHeroModelPathByID(rankVo2.HeroId);
            Utils.SetSpineModelOnFGUI(this.RankingListMain.spine2, model2, 90f);
            this.RankingListMain.num2Lb.text = rankVo2.PlayerName;
        }
        else
        {
            this.RankingListMain.num2Lb.text = ConfigUtils.GetStringByKey(10189);
            Utils.ClearSpineModelOnFGUI(this.RankingListMain.spine2);
        }

        PvpRankVo rankVo3 = PvpRankDataManager.Instance.ThisRankHasPlayer(3);
        if(rankVo3 != null)
        {
            string model3 = ConfigUtils.GetHeroModelPathByID(rankVo3.HeroId);
            Utils.SetSpineModelOnFGUI(this.RankingListMain.spine3, model3, 90f);
            this.RankingListMain.num3Lb.text = rankVo3.PlayerName;
        }
        else
        {
            this.RankingListMain.num3Lb.text = ConfigUtils.GetStringByKey(10189);
            Utils.ClearSpineModelOnFGUI(this.RankingListMain.spine3);
        }

    }

    private void UpdateFightLeftCnt()
    {
        this.RankingListMain.cntLb.SetVar("cur", PvpRankDataManager.Instance.FightLeftCount.ToString()).SetVar("total", _common15.Param1).FlushVars();
    }

    private void UpdateMyRankItem()
    {
        if (!PvpRankDataManager.Instance.YestdayMyRankAwardGet &&
            PvpRankDataManager.Instance.YestdayMyRank <= _PvpRewardUnits.Count && PvpRankDataManager.Instance.YestdayMyRank>=1 && PvpRankDataManager.Instance.YesterdayGFPlayCounter > 0)//有奖励但是没有领取
        {
            this.RankingListMain.myselfItem.rankCtrl.selectedIndex = 0;
            this.RankingListMain.myselfItem.status.selectedIndex = 0;
            if (PvpRankDataManager.Instance.YestdayMyRank <= 3)
            {
                this.RankingListMain.myselfItem.numCtrl.selectedIndex = PvpRankDataManager.Instance.YestdayMyRank - 1;
            }
            else
            {
                this.RankingListMain.myselfItem.numCtrl.selectedIndex = 3;
            }
            ConfigFirstRewardUnit pvpReward = _PvpRewardUnits[PvpRankDataManager.Instance.YestdayMyRank-1];
            string[] rewards = pvpReward.Reward.Split('|');
            _itemDatas.Clear();
            foreach (var rewardStr in rewards)
            {
                string[] rewardItem = rewardStr.Split(',');
                _itemDatas.Add(
                    new ItemData()
                    {
                        id = int.Parse(rewardItem[0]),
                        count = double.Parse(rewardItem[1])
                    }
                );
            }
            this.RankingListMain.myselfItem.rewardList.itemRenderer = RewardItemRender;
            this.RankingListMain.myselfItem.rewardList.data = _itemDatas;
            this.RankingListMain.myselfItem.rewardList.numItems = _itemDatas.Count;
            this.RankingListMain.myselfItem.num.text = PvpRankDataManager.Instance.YestdayMyRank.ToString();
        }
        else
        {
            PvpRankVo rankVo = PvpRankDataManager.Instance.IsMyInRank();
            this.RankingListMain.myselfItem.rankCtrl.selectedIndex = rankVo != null ? 0 : (PvpRankDataManager.Instance.TodayGFPlayCounter > 0 ? 1 : 2);
            if(!PvpRankDataManager.Instance.YestdayMyRankAwardGet && PvpRankDataManager.Instance.YesterdayGFPlayCounter > 0)
                this.RankingListMain.myselfItem.status.selectedIndex = 1;
            else
            {
                this.RankingListMain.myselfItem.status.selectedIndex = 1;
            }
            if (rankVo != null)
            {
                if (rankVo.Rank <= 3)
                {
                    this.RankingListMain.myselfItem.numCtrl.selectedIndex = rankVo.Rank-1;
                }
                else
                {
                    this.RankingListMain.myselfItem.numCtrl.selectedIndex = 3;
                }
            
                ConfigFirstRewardUnit pvpReward = _PvpRewardUnits[rankVo.Rank-1];
                string[] rewards = pvpReward.Reward.Split('|');
                _itemDatas.Clear();
                foreach (var rewardStr in rewards)
                {
                    string[] rewardItem = rewardStr.Split(',');
                    _itemDatas.Add(
                        new ItemData()
                        {
                            id = int.Parse(rewardItem[0]),
                            count = double.Parse(rewardItem[1])
                        }
                    );
                }
                this.RankingListMain.myselfItem.rewardList.itemRenderer = RewardItemRender;
                this.RankingListMain.myselfItem.rewardList.data = _itemDatas;
                this.RankingListMain.myselfItem.rewardList.numItems = _itemDatas.Count;
                this.RankingListMain.myselfItem.num.text = rankVo.Rank.ToString();
            }
            else
            {
                ConfigCommonUnit commonUnit11 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(11);
                string[] rewards = commonUnit11.Param1.Split('|');
                _itemDatas.Clear();
                foreach (var rewardStr in rewards)
                {
                    string[] rewardItem = rewardStr.Split(',');
                    _itemDatas.Add(
                        new ItemData()
                        {
                            id = int.Parse(rewardItem[0]),
                            count = double.Parse(rewardItem[1])
                        }
                    );
                }
                this.RankingListMain.myselfItem.rewardList.itemRenderer = RewardItemRender;
                this.RankingListMain.myselfItem.rewardList.data = _itemDatas;
                this.RankingListMain.myselfItem.rewardList.numItems = _itemDatas.Count;
            }
        }
        this.RankingListMain.myselfItem.getBtn.onClick.Add(this.OnClickGetMyRewardBtn);
        this.RankingListMain.myselfItem.fightingCapacity.text = StringUtils.FormatCurrency(RoleManager.Instance.TotalFight);
        this.RankingListMain.myselfItem.pName.text = DataManager.Instance.GetRoleData().userName;
        this.RankingListMain.myselfItem.headIcon.icon = UIResource.GetItemUrl(DataManager.Instance.GetRoleData().GetAvatarUrl());
    }

    private void OnClickGetMyRewardBtn()
    {
        if (!PvpRankDataManager.Instance.YestdayMyRankAwardGet &&
            PvpRankDataManager.Instance.YestdayMyRank <= _PvpRewardUnits.Count &&
            PvpRankDataManager.Instance.YestdayMyRank >= 1) //有奖励但是没有领取
        {
            var builder = GlobalFirstClaimAward_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_GlobalFirstClaimAward_CS, builder.Build());
        }
        else
        {
            UIManager.Instance.ToastByKey(10198);
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        int totalSecond = ServerTimeManager.Instance.GetToZeroLeftTime();
        if (totalSecond > 0)
        {
            this.RankingListMain.timeLeft.SetVar("value", StringUtils.GetTimeString((int)totalSecond)).FlushVars();
        }
    }

    private void PVPListItemRender(int index, GObject item)
    {
        if (index <= 2)
        {
            ((UI_RankingItem) item).numCtrl.selectedIndex = index;
        }
        else
        {
            ((UI_RankingItem) item).numCtrl.selectedIndex = 3;
        }

        PvpRankVo rankVo = PvpRankDataManager.Instance.ThisRankHasPlayer(index + 1);
        if (rankVo != null)
        {
            if (rankVo.PlayerId == DataManager.Instance.GetRoleData().userID)
            {
                ((UI_RankingItem) item).status.selectedIndex = 2;
            }
            else
            {
                if (PvpRankDataManager.Instance.IsMyInRank() == null)
                {
                    ((UI_RankingItem) item).status.selectedIndex = 0;
                }
                else
                {
                    if(PvpRankDataManager.Instance.IsMyInRank().Rank > index+1)
                        ((UI_RankingItem) item).status.selectedIndex = 0;
                    else
                    {
                        ((UI_RankingItem) item).status.selectedIndex = 2;
                    }
                }
            }
            ((UI_RankingItem) item).pName.text = rankVo.PlayerName;
            // string s = rankVo.PlayerHeadIcon;
            ((UI_RankingItem) item).heroIcon.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(rankVo.PlayerHeadIcon.ToInt32()).Icon);
            ((UI_RankingItem) item).fightingCapacity.text = StringUtils.FormatCurrency(rankVo.Fight);
        }
        else
        {
            ((UI_RankingItem) item).status.selectedIndex = 1;
            PvpRankVo myRank = PvpRankDataManager.Instance.IsMyInRank();
            if (myRank!=null && myRank.Rank < index + 1)
            {
                ((UI_RankingItem) item).standCollarBtn.enabled = false;
            }
            else
            {
                ((UI_RankingItem) item).standCollarBtn.enabled = true;
            }
     
        }

        ((UI_RankingItem) item).num.text = (index + 1).ToString();
        ConfigFirstRewardUnit pvpReward = _PvpRewardUnits[index];
        string[] rewards = pvpReward.Reward.Split('|');
        _itemDatas.Clear();
        foreach (var rewardStr in rewards)
        {
            string[] rewardItem = rewardStr.Split(',');
            _itemDatas.Add(
                new ItemData()
                {
                    id = int.Parse(rewardItem[0]),
                    count = double.Parse(rewardItem[1])
                }
                );
        }
        ((UI_RankingItem) item).rewardList.itemRenderer = RewardItemRender;
        ((UI_RankingItem) item).rewardList.data = _itemDatas;
        ((UI_RankingItem) item).rewardList.numItems = _itemDatas.Count;
        ((UI_RankingItem) item).challengeBtn.data = rankVo;
        ((UI_RankingItem) item).challengeBtn.onClick.Add(this.OnClickPVPBtn);
        ((UI_RankingItem) item).standCollarBtn.data = index+1;
        ((UI_RankingItem) item).standCollarBtn.onClick.Add(this.OnClickStandBtn);
    }
    
    private void RewardItemRender(int index, GObject item)
    {
        List<ItemData> rewardItemList = item.parent.data as List<ItemData>;
        ((UI_ItemCom)item).SetItemDataWithGuid(rewardItemList[index], true);
    }

    private void OnClickPVPBtn(EventContext context)
    {
        if (PvpRankDataManager.Instance.FightLeftCount > 0)
        {
            PvpRankVo pvpRankVo = (PvpRankVo) (context.sender as GButton).data;
            if (pvpRankVo != null)
            {
                var builder = GlobalFirstChallengeReq_CS.CreateBuilder();
                builder.BattlePower = RoleManager.Instance.TotalFight;
                builder.RankPos = (uint) pvpRankVo.Rank;
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_GlobalFirstChallengeReq_CS, builder.Build());
            }
        }
        else
        {
            OnClickAddNumBtn();
        }

    }

    private void OnClickStandBtn(EventContext context)
    {
        if (PvpRankDataManager.Instance.FightLeftCount > 0)
        {
            int pos = (int) (context.sender as GButton).data;
            var builder = GlobalFirstTakeOverRank_CS.CreateBuilder();
            builder.RankPos = (uint) pos;
            builder.BattlePower = RoleManager.Instance.TotalFight;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_GlobalFirstTakeOverRank_CS, builder.Build());
        }else
        {
            OnClickAddNumBtn();
        }
    }

    private void OnClickReportBtn()
    {
        UIManager.Instance.ShowUIPanel("BattleReportMain");
    }

    private void OnClickAddNumBtn()
    {
        if (PvpRankDataManager.Instance.FightBuyCnt < int.Parse(_common15.Param3))
        {
            UIManager.Instance.ShowUIPanel("BuyFightTimes");
        }
    }

    private void OnClickHelpBtn()
    {
        UIManager.Instance.ShowUIPanel("Help", HelpType.Help_txdy);
    }

    private void OnClickMySelfItem()
    {
        PvpRankVo rankVo = PvpRankDataManager.Instance.IsMyInRank();
        if (rankVo != null)
        {
            this.RankingListMain.rankingList.ScrollToView(rankVo.Rank-1);
        }
    }

    private void PlayLobbyBGM()
    {
        var view = UIManager.Instance.FindByName("Lobby") as LobbyView;
        if (view != null)
        {
            view?.PlaySceneBGM();
        }
    }
}
