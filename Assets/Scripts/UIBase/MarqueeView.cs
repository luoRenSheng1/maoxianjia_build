using CommonEx;
using Engine;
using FairyGUI;
using UnityEngine;

public class MarqueeView : UIViewBase
{ 
    private UI_Marquee marqueeView => this.main as UI_Marquee;

    public MarqueeView()
    {
        this.name = "Marquee";
        this.package = "CommonEx";
        this.component = "Marquee";
        this.removePackage = true;
        this.type = UIType.Tip;
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.marqueeView.bg.visible = false;// 默认隐藏背景图
        // 初始化跑马灯管理器
        MarqueeManager.Instance.Initialize(this.marqueeView.content, this.marqueeView.bg);
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonExBinder.BindAll();
    }

    protected override void OnShow()
    {
        base.OnShow();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        // 跑马灯
        MarqueeManager.Instance.Marquee();
    }

}