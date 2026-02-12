
using System.Collections.Generic;
using Config;
using Engine;
using FairyGUI;
using Summon;

public class SummonHeroHelpView : UIViewBase
{
    private UI_SummonHeroHelp SummonHeroHelp => this.main as UI_SummonHeroHelp;
    
    private List<ConfigHelpUnit> _heroIntroList = new List<ConfigHelpUnit>();
    
    public SummonHeroHelpView()
    {
        this.name = "SummonHeroHelp";
        this.package = "Summon";
        this.component = "SummonHeroHelp";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        SummonBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.SummonHeroHelp.closeBtn.onClick.Add(this.Hide);
        this.SummonHeroHelp.closeBtn.onClick.Add(this.HideWithSoundEffect);
        _heroIntroList = ConfigUtils.GetHelpUnitsByType(HelpType.Help_SummonHeroHelp);
        this.SummonHeroHelp.heroIntroList.itemRenderer = HeroIntroRender;
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        this.SummonHeroHelp.heroIntroList.numItems = _heroIntroList.Count;
    }

    private void HeroIntroRender(int index, GObject item)
    {
        var unit = _heroIntroList[index];
        
        if (index == 0)
        {
            ((UI_HeroIntroItem)item).ctrl.selectedIndex = 0;
        }
        else
        {
            if (index % 2 == 0)
            {
                ((UI_HeroIntroItem)item).ctrl.selectedIndex = 2;
            }
            else
            {
                ((UI_HeroIntroItem)item).ctrl.selectedIndex = 1;
            }
        }

        if (index == _heroIntroList.Count - 1)
        {
            if (index % 2 == 0)
            {
                ((UI_HeroIntroItem)item).ctrl.selectedIndex = 4;
            }
            else
            {
                ((UI_HeroIntroItem)item).ctrl.selectedIndex = 3;
            }
        }
        
        ((UI_HeroIntroItem)item).desc.text = ConfigUtils.GetTextById(unit.Content);
    }

}
