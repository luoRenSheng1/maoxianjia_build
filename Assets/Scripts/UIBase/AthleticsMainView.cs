
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using PVPMap;
using EventDispatcher = EngineBase.EventDispatcher;

public class AthleticsMainView : UIViewBase
{
    private UI_AthleticsMain AthleticsMain => this.main as UI_AthleticsMain;

    private ConfigCommonUnit _common12;
    public AthleticsMainView()
    {
        this.name = "AthleticsMain";
        this.package = "PVPMap";
        this.component = "AthleticsMain";
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
        _common12 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(12);
        this.AthleticsMain.PvpItem.pvpList.itemRenderer = PVPListItemRender;
        // this.AthleticsMain.closeBtn.onClick.Add(this.Hide);
        this.AthleticsMain.closeBtn.onClick.Add(this.HideWithSoundEffect);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PVP_GET_REWARD_UPDATE, this.OnUpdatePvpReddot);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PVP_GET_REWARD_UPDATE, this.OnUpdatePvpReddot);
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.AthleticsMain.PvpItem.pvpList.numItems = 1;
        this.AthleticsMain.PvpItem.pvpList.ResizeToFit();
    }

    private void PVPListItemRender(int index, GObject item)
    {
        if (index == 0)
        {
            int totalTime = (int) (Utils.GetTimestampByDateStr(_common12.Param2) - (long)ServerTimeManager.Instance.CurServerTime);
            if (totalTime > 0)
            {
                ((UI_AthleticsItem) item).status.selectedIndex = 0;
                ((UI_AthleticsItem) item).timeLb.text = StringUtils.GetTimeString(totalTime);
            }
            else
            {
                ((UI_AthleticsItem) item).status.selectedIndex = 1;
            }
            ((UI_AthleticsItem) item).goBtn.onClick.Add(this.OnClickPVPBtn);
            
            ((UI_AthleticsItem) item).reddot.visible = (!PvpRankDataManager.Instance.YestdayMyRankAwardGet &&
                                                        PvpRankDataManager.Instance.YestdayMyRank >= 1 && PvpRankDataManager.Instance.YesterdayGFPlayCounter>0);
        }
    }

    private void OnClickPVPBtn()
    {
        var builder = GlobalFirstRank_CS.CreateBuilder();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_GlobalFirstRank_CS, builder.Build());
    }

    private void OnUpdatePvpReddot()
    {
        this.AthleticsMain.PvpItem.pvpList.numItems = 1;
    }
}
