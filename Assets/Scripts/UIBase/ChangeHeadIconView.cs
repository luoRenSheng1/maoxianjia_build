
using System.Collections.Generic;
using System.Linq;
using Common;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using Setting;

public class ChangeHeadIconView : UIViewBase
{
    private UI_HeadIconChangeMain ChangeHeadIcon => this.main as UI_HeadIconChangeMain;

    private List<ConfigHeroUnit> _heros;
    private List<HeroInfo> hasHeros;
    private string iconUrl;

    public ChangeHeadIconView()
    {
        this.name = "ChangeHeadIcon";
        this.package = "Setting";
        this.component = "HeadIconChangeMain";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        SettingBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.ChangeHeadIcon.headIconChange.iconList.itemRenderer = HeadIconRender;
        this.ChangeHeadIcon.headIconChange.changeBtn.onClick.Add(this.OnClickChangeHead);
        // this.ChangeHeadIcon.closeBtn.onClick.Add(this.Hide);
        this.ChangeHeadIcon.closeBtn.onClick.Add(this.HideWithSoundEffect);

        _heros = ConfigDataGroup.GetInstance<ConfigHero>().Data.Values.ToList();
        hasHeros = HeroInfoManager.Instance.GetHeroInfos();
    }

    protected override void OnDispose()
    {
        base.OnDispose();
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateHeadIcon();
    }

    private void HeadIconRender(int index, GObject item)
    {
        ConfigHeroUnit heroUnit = _heros[index];
        ((UI_HeadIcon) item).status.selectedIndex = 0;
        foreach (var hero in hasHeros)
        {
            if (hero.HeroUnit.Id == heroUnit.Id)
            {
                ((UI_HeadIcon) item).status.selectedIndex = 1;
            }
        }
        
        ((UI_HeadIcon) item).icon = UIResource.GetHeroBody(heroUnit.HeroBody);
        ((UI_HeadIcon) item).data = _heros[index].HeroBody;
        ((UI_HeadIcon) item).onClick.Set(OnGetIconUrl);
    }

    private void OnGetIconUrl(EventContext context)
    {
        string url = (string)((UI_HeadIcon) context.sender).data;
        if (url != null)
        {
            iconUrl = url;
        }
    }

    private void UpdateHeadIcon()
    {
        this.ChangeHeadIcon.headIconChange.iconList.numItems = _heros.Count;
    }

    private void OnClickChangeHead()
    {
        // return iconUrl;点击按钮changeBtn，触发OnClickChangeHead事件。将iconUrl传参给某个函数
        // 更换个人信息的头像

        // 更换主界面的头像
        // UIManager.Instance.ShowUIPanel("SettingMain", iconUrl);
        // UIManager.Instance.CloseUIPanel("ChangeHeadIcon");
        // UpdateHeadIconUrl(iconUrl);

    }

    public string UpdateHeadIconUrl()
    {
        // iconUrl = url;
        return iconUrl;
    }

}