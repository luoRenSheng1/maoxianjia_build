using System;
using Common;
using Engine;
using FairyGUI;
using UnityEngine;

public class PowerUpView : UIViewBase
{
    private UI_PowerUp PowerUp => this.main as UI_PowerUp;

    private double _addFightVal;
    private double _startFightVal;
    private double _endFightVal;
    private bool _isSuper;

    private float _timer;
    public PowerUpView()
    {
        this.type = UIType.Tip;
        this.name = "PowerUp";
        this.package = "Common";
        this.component = "PowerUp";
        this.removePackage = true;
        this.safeAreaInset = false;
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.PowerUp.touchable = false;
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _addFightVal = (double) values[0];
        _endFightVal = (double) values[1];
        _startFightVal = _endFightVal - _addFightVal;
        _isSuper = (bool) values[2];

        _timer = 0f;
        this.PowerUp.spineUI.spineAnimation.skeleton.SetToSetupPose();
        this.PowerUp.spineUI.spineAnimation.state.ClearTracks();
        this.PowerUp.spineUI.spineAnimation.state.SetAnimation(0, "chuxian", false);
        if (_isSuper)
        {
            this.PowerUp.superCtrl.selectedIndex = 1;
            this.PowerUp.superUI.spineAnimation.skeleton.SetToSetupPose();
            this.PowerUp.superUI.spineAnimation.state.ClearTracks();
            this.PowerUp.superUI.spineAnimation.state.SetAnimation(0, "chuxian", false);
            
            GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.MilitaryPowerUpSE);
        }
        else
        {
            this.PowerUp.superCtrl.selectedIndex = 0;
        }
        __playNum();
    }
    
    void __playNum()
    {
        this.PowerUp.t0.Play(() =>
        {
            GTween.Kill(this, true);
            this.PowerUp.addValue.SetVar("value",StringUtils.FormatCurrency(_addFightVal)).FlushVars(); 
            GTween.ToDouble(_startFightVal, _endFightVal, 0.5f).SetEase(EaseType.Linear)
                .OnUpdate((GTweener tweener) =>
                {
                    this.PowerUp.fightValLb.text = "" + StringUtils.FormatCurrency(Math.Ceiling(tweener.value.d));
                });
        });
        
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        _timer += UnityEngine.Time.deltaTime;
        if (_timer > 1.8f)
        {
            _timer = 0f;
            UIManager.Instance.CloseUIPanel("PowerUp");
        }
    }

    protected override void OnHide()
    {
        base.OnHide();
        _timer = 0;
    }
}
