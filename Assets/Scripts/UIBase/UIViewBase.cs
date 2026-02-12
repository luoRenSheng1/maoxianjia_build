using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;
using Engine;

public enum UIType
{
    Background = 0,
    Normal = 1,
    Top = 2,
    Newbie = 3,
    Modal = 4,
    Tip = 5
}

public enum UIState
{
    Init = 0,
    Show = 1,
    Hide = 2,
    Dispose = 3
}

public enum FuncType
{
    none,
    village,
    Guide//引导必须页面
}

public enum UIBackWindowEvent
{
    ExitCtrl = 0,
    ExitApp = 1,
    Custom = 2
}

public class UIViewBase
{
    public string name { get; set; } = "";
    public string package { get; set; } = "";
    public string component { get; set; } = "";

    public UIType type { get; set; } = UIType.Top;
    public FuncType FuncType = FuncType.none;
    public FuncType GuideType = FuncType.none;
    public UIState state { get; set; } = UIState.Init;
    public bool fitStageSizeChange { get; set; } = false;
    public bool fairyBatching { get; set; } = true;
    public bool removePackage { get; set; } = true;
    public bool safeAreaInset { get; set; } = false;

    public GComponent main { get; set; } = null;
    protected List<GComponent> components { get; set; } = null;

    #region 内部事件

    protected virtual void OnInit()
    {
    }

    protected virtual void OnReSize()
    {
    }

    protected virtual void OnShow()
    {
        // if (this.name == "Lobby" || this.name == "Login" || this.name == "Loading" || this.name == "PayLoading" || this.name == "SCLoading") return;
        // GameManager.Instance.SoundManager.PlayEffectWithoutLoop(7);
    }

    protected virtual void OnHide()
    {
        // if (this.name == "Lobby" || this.name == "Login" || this.name == "Loading" || this.name == "PayLoading" || this.name == "SCLoading") return;
        // GameManager.Instance.SoundManager.PlayEffectWithoutLoop(5);
    }

    protected virtual void OnUpdate()
    {
    }

    protected virtual void OnDispose()
    {
    }

    protected virtual void OnUpdateParams(params object[] values)
    {
    }

    #endregion

    #region 生命周期接口

    public bool IsDead()
    {
        if (main != null && !main.isDisposed && this.state != UIState.Dispose)
        {
            return false;
        }

        return true;
    }

    public bool IsShow()
    {
        return state == UIState.Show;
    }

    // 是否在舞台显示
    public bool IsOnStage()
    {
        if (!IsDead())
        {
            if (this.main != null && this.main.parent != null)
            {
                return this.main.parent.visible;
            }
        }

        return false;
    }

    public void SetVisible(bool visible)
    {
        if (visible)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    #endregion

    public void Init(params object[] values)
    {
        LogUtils.LogWarningFormat("UIViewBase Init : {0}", this.name);

        if (safeAreaInset)
        {
            var panel = main.GetChild("panel");
            if (null != panel)
            {
                panel.asCom.SetUISafeAreaOffset();
            }
        }

        var frame = main.GetChild("frame");
        if (null != frame)
        {
            frame.asCom.onClick.Set(() => { SetVisible((false)); });
        }

        this.OnInit();
        //
        UpdateParams(values);
    }

    public void UpdateParams(params object[] values)
    {
        //LogUtils.LogWarning("UIViewBase UpdateParams : " + this.name);

        this.OnUpdateParams(values);
    }

    public void Show()
    {
        // LogUtils.LogWarning("UIViewBase Show : " + this.name);

        this.state = UIState.Show;
        if (this.main != null)
        {
            this.main.visible = true;
        }

        this.OnShow();
        this.ShowInComponentBase();
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVT_UI_SHOW, this.name);
    }

    public void Hide()
    {
        // LogUtils.LogWarning("UIViewBase Hide : " + this.name);

        this.state = UIState.Hide;
        if (this.main != null)
        {
            this.main.visible = false;
        }

        this.HideInComponentBase();
        this.OnHide();
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVT_UI_HIDE, this.name);



    }

    /// <summary>
    /// 关闭按钮带关闭音效
    /// </summary>
    public void HideWithSoundEffect()
    {
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralCloseSE);
        
        if (this.IsShow())
        {
            this.Hide();
        }
    }

    public void Destroy()
    {
        if (this.IsShow())
        {
            this.Hide();
        }

        this.state = UIState.Dispose;
        this.OnDispose();
        this.DestroyInComponentBase();
        if (this.main != null)
        {
            this.main.Dispose();
            this.main = null;
        }

        if (this.removePackage)
        {
            UIManager.Instance.SafeRemovePackage(this.package);
        }

        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVT_UI_DESTROY, this.name);

        LogUtils.LogWarning("UIViewBase Destroy : " + this.name);
    }

    public virtual void OnFocus()
    {
        //LogUtils.LogWarning("UIViewBase OnFocus : " + this.name);

        this.OnFocusInComponentBase();
    }

    public virtual void UnFocus()
    {
        //LogUtils.LogWarning("UIViewBase UnFocus : " + this.name);

        this.UnFocusInComponentBase();
    }

    public virtual void BindAll()
    {
    }

    /// 返回上一级界面，UIManager调用，0销毁该界面，1提示退出APP，2自定义事件
    public virtual UIBackWindowEvent OnWillBackWindow()
    {
        return UIBackWindowEvent.ExitCtrl;
    }

    /// Component 支持
    protected void RegisterComponentBase(GComponent c)
    {
        if (c == null)
        {
            return;
        }

        if (this.components == null)
        {
            this.components = new List<GComponent>();
        }

        this.components.Add(c);
    }

    protected void UnRegisterComponentBase(GComponent c)
    {
        if (c == null || this.components == null)
        {
            return;
        }

        if (components.Contains(c))
        {
            components.Remove(c);
        }
    }

    protected void ShowInComponentBase()
    {
        if (this.components == null)
        {
            return;
        }

        foreach (var component in components)
        {
            if (component != null)
            {
                component.visible = true;
            }
        }
    }

    protected void HideInComponentBase()
    {
        if (this.components == null)
        {
            return;
        }

        foreach (var component in components)
        {
            if (component != null)
            {
                component.visible = false;
            }
        }
    }

    protected void DestroyInComponentBase()
    {
        if (this.components == null)
        {
            return;
        }

        foreach (var component in components)
        {
            if (component != null)
            {
                component.Dispose();
            }
        }

        components.Clear();
    }

    protected void OnFocusInComponentBase()
    {
    }

    protected void UnFocusInComponentBase()
    {
    }

    public void OnStageSizeChanged()
    {
        /*if this.fitStageSizeChange and this.main then
        -- 恒定为竖屏
            --LogWarning('UIViewBase OnStageSizeChanged : ', this.__name, GRoot.inst.width, GRoot.inst.height)
        local height = math.max(GRoot.inst.width, GRoot.inst.height)
        local width = math.min(GRoot.inst.width, GRoot.inst.height)
        this.main:SetSize(width, height)
        end
        this.onReSize()*/
    }

    public void Update()
    {
        if(IsShow() && IsOnStage())
            this.OnUpdate();
    }
}