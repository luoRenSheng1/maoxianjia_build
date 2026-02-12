using System;
using Common;
using Engine;

public class TipInfoShowView : UIViewBase
{
    private UI_TipInfoShow boxWindow => this.main as UI_TipInfoShow;
    
    private string _content;

    public TipInfoShowView()
    {
        this.type = UIType.Tip;
        this.name = "TipInfoShow";
        this.package = "Common";
        this.component = "TipInfoShow";
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _content = values[0] as string;
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.boxWindow.close.onClick.Add(this.Hide);
        this.boxWindow.close.onClick.Add(this.HideWithSoundEffect);
        this.boxWindow.okBtn.onClick.Add(this.OnClickOkBtn);
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.boxWindow.content.text = _content;
        this.boxWindow.EnsureBoundsCorrect();
    }

    private void OnClickOkBtn()
    {
        this.Hide();
    }
}
