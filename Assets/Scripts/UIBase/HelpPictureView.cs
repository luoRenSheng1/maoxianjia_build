
using System.Collections.Generic;
using System.Linq;
using Common;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using Village;
using EventDispatcher = EngineBase.EventDispatcher;

public class HelpPictureView : UIViewBase
{
    private UI_HelpPicture HelpPicture => this.main as UI_HelpPicture;
    
    private List<ConfigHelpUnit> helpInfos;
    private HelpType _helpType;

    public HelpPictureView()
    {
        this.name = "HelpPicture";
        this.package = "Common";
        this.component = "HelpPicture";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        VillageBinder.BindAll();
    }
    
    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _helpType = (HelpType)values[0];
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.HelpPicture.closeBtn2.onClick.Add(this.Hide);
        this.HelpPicture.closeBtn2.onClick.Add(this.HideWithSoundEffect);
        this.HelpPicture.helpList.itemRenderer = BuildHelpItemRender;
    }

    protected override void OnShow()
    {
        base.OnShow();
        OnHelpInfoUpdate();
    }

    private void BuildHelpItemRender(int index, GObject item)
    {
        ConfigHelpUnit helpUnit = helpInfos[index];
        ((UI_HelpItemPicture) item).title.text = helpUnit.Content;
        ((UI_HelpItemPicture) item).icon.url = UIResource.GetItemUrl(helpUnit.Icon.ToString());
        ((UI_HelpItemPicture) item).data = helpInfos[index];
    }

    private void OnHelpInfoUpdate()
    {
        helpInfos = ConfigUtils.GetHelpUnitsByType(_helpType);
        this.HelpPicture.helpList.numItems = helpInfos.Count;
        // this.HelpPicture.helpList.ResizeToFit();
    }
}