using System.Collections.Generic;
using Common;
using Config;
using Engine;
using FairyGUI;
using Shop;

public class HelpView : UIViewBase
{ 
    private UI_help Help => this.main as UI_help;

    private List<ConfigHelpUnit> _helpInfos;
    private HelpType _helpType;
    
    public HelpView()
    {
        this.name = "Help";
        this.package = "Common";
        this.component = "help";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        ShopBinder.BindAll();
    }
    
    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _helpType = (HelpType)values[0];
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.Help.closeBtn.onClick.Add(this.Hide);
        this.Help.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.Help.helpList.itemRenderer = HelpItemRender;
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateShopHelp();
    }

    private void HelpItemRender(int index, GObject item)
    {
        ConfigHelpUnit helpUnit = _helpInfos[index];
        // ((UI_helpItem) item).content.text = helpUnit.Content;
        ((UI_helpItem) item).content.text = ConfigUtils.GetTextById(helpUnit.Content,helpUnit.ContentParam);
        ((UI_helpItem) item).data = _helpInfos[index];
    }

    private void UpdateShopHelp()
    {
        _helpInfos = ConfigUtils.GetHelpUnitsByType(_helpType);
        this.Help.helpList.numItems = _helpInfos.Count;
        this.Help.helpList.ResizeToFit();
    }
}
