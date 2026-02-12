
using System.Collections.Generic;
using Engine;
using FairyGUI;
using msg;
using PVPMap;

public class BattleReportMainView : UIViewBase
{
    private UI_BattleReportMain ReportMain => this.main as UI_BattleReportMain;
    
    private List<BattleReportVo> _battleReportVos;
    public BattleReportMainView()
    {
        this.name = "BattleReportMain";
        this.package = "PVPMap";
        this.component = "BattleReportMain";
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
        // this.ReportMain.closeBtn.onClick.Add(this.Hide);
        this.ReportMain.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.ReportMain.battleReport.reportList.itemRenderer = BattleReportItemRender;
    }

    protected override void OnShow()
    {
        base.OnShow();
        _battleReportVos = PvpRankDataManager.Instance.GetBattleReportList();
        this.ReportMain.battleReport.reportList.numItems = _battleReportVos.Count;
    }

    private void BattleReportItemRender(int index, GObject item)
    {
        BattleReportVo battleReportVo = _battleReportVos[index];
        ((UI_BattleReportItem) item).content.text = GetReportContent(battleReportVo.ReportType, battleReportVo.Param);
        ((UI_BattleReportItem) item).timeLb.SetVar("value",Utils.GetDateTime(battleReportVo.TimeStamp).ToString("yyyy-MM-dd HH:mm:ss")).FlushVars();
    }

    private string GetReportContent(GFWarReportType reportType, List<string> param)
    {
        int key = 0;
        switch (reportType)
        {
            case GFWarReportType.GFWarReportType_Win_ButNotFirst://挑战获胜，但是不是第一个获胜
                key = 10195;
                break;
            case GFWarReportType.GFWarReportType_InRank_BeChallenged_Failed://在排行榜被挑战失败
                key = 10191;
                break;
            case GFWarReportType.GFWarReportType_InRank_BeChallenged_Win://在排行榜被挑战胜利
                key = 10192;
                break;
            case GFWarReportType.GFWarReportType_NotInRank_Challenged_Failed://未在排行榜挑战失败
                key = 10194;
                break;
            case GFWarReportType.GFWarReportType_NotInRank_Challenged_Win://未在排行榜挑战胜利
                key = 10193;
                break;
            case GFWarReportType.GFWarReportType_NotInRank_TakenRank://未在排行榜,占领成功
                key = 10197;
                break;
            case GFWarReportType.GFWarReportType_TakeRank_ButNotFirst://自己未在排行榜，挑战排行榜空位，被其他玩家先占领
                key = 10196;
                break;
        }
        
        if(param.Count == 2 && param.Count > 1)
            return ConfigUtils.FormatStringByKey(key, param[0], param[1]);
        return ConfigUtils.FormatStringByKey(key, param[0]);
    }
}
