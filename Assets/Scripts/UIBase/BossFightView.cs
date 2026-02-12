
using Common;
using Config;
using Engine;

public class BossFightView : UIViewBase
{
    private UI_BossFightWindow bossFightUI => this.main as UI_BossFightWindow;

    private int bossId;
    public BossFightView()
    {
        this.name = "BossFight";
        this.package = "Common";
        this.component = "BossFightWindow";
        this.removePackage = true;
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        bossId = (int) values[0];
    }

    protected override void OnShow()
    {
        base.OnShow();
        var heroInfo = HeroInfoManager.Instance.GetMyHero();
        string strResName = ConfigUtils.GetHeroModelPathByID(heroInfo.HeroUnit.Id);
        Utils.SetSpineModelOnFGUI(this.bossFightUI.hero, strResName, 160);

        ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(bossId);
        var monsterModelPath = monsterUnit.Model;
        Utils.SetSpineModelOnFGUI(this.bossFightUI.monster, monsterModelPath, monsterUnit.BossSize);
    }
}
