
using Config;
using DungeonMap;
using Engine;
using EngineBase;
using msg;

public class DungeonFailedView : UIViewBase
{
    private UI_DungeonFailed DungeonFailed => this.main as UI_DungeonFailed;
    public DungeonFailedView()
    {
        this.name = "DungeonFailed";
        this.package = "DungeonMap";
        this.component = "DungeonFailed";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        DungeonMapBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.DungeonFailed.againBtn.onClick.Add(this.OnClickAgainBtn);
    }

    protected override void OnShow()
    {
        base.OnShow();
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralLoseSE);
        ConfigDungeonChapterUnit chapterUnit = ConfigUtils.GetDungeonChapterById(DungeonMapManager.Instance.dungeonType);
        int total = ConfigUtils.GetDungeonTotalCount(chapterUnit.Id);
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(chapterUnit.ItemID);
        this.DungeonFailed.itemIcon.url = UIResource.GetItemUrl(itemTypeUnit.Icon);
        this.DungeonFailed.itemCntLb.SetVar("cur",ItemInfoManager.Instance.GetItemCount(chapterUnit.ItemID).ToString()).SetVar("total", total.ToString()).FlushVars();
        // this.DungeonFailed.img.url = UIResource.GetImageUrlWithLang("sb2","DungeonMap");
    }
    
    private void OnClickAgainBtn()
    {
        ConfigDungeonStageUnit stageUnit = ConfigUtils.GetDungeonStageByNandu(DungeonMapManager.Instance.GuanKaStage, (int) DungeonMapManager.Instance.dungeonType);
        // UIManager.Instance.ShowUIPanel("DungeonMap", stageUnit);
        
        UIManager.Instance.CloseUIPanel("DungeonFailed");

        var builder = Copy_Begin_CS.CreateBuilder();
        builder.CopyType = (eCopyType) stageUnit.Type;
        builder.StartStageId = (uint) ConfigUtils.GetDungeonStageByNandu(DungeonMapManager.Instance.GuanKaStage, stageUnit.Type).Id;
        Copy_Begin_CS copyBeginCs = builder.Build();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_Copy_Begin_CS, copyBeginCs);

    }
    
}
