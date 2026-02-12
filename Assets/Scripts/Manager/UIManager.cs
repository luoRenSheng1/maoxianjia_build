using Engine;
using EngineBase;
using System;
using System.Collections.Generic;
using Config;
using FairyGUI;
using msg;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : TSingleton<UIManager>
{
    protected List<UIViewBase> controllers = new List<UIViewBase>();
    protected List<UIViewBase> controllersAsyncLoad = new List<UIViewBase>();

    private GameObject uiRoot;

    private CTimer timerLoading = new CTimer();

    private System.Action actionLoadingDone;
    
    public override void Init()
    {
        base.Init();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void Dispose()
    {
        base.Dispose();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnInit()
    {
    }

    public void Tick(float deltaSeconds)
    {
        if (timerLoading.TimeOver())
        {
            HideLoading();
        }

        for (int i = controllers.Count-1; i >=0; i--)
        {
            try
            {
                controllers[i].Update();
            }
            catch (Exception e)
            {
                LogUtils.LogException(e);
            }
        }
    }

    public GameObject GetRootUI()
    {
        return GRoot.inst.container.gameObject;
    }

    private void OnSceneLoaded(Scene _null1, LoadSceneMode _null2)
    {
        ResumeCommonUI();
    }

    public void LoadCommonUI()
    {
    }

    public bool IsInited()
    {
        return true;
    }

    public void Cleanup()
    {
    }

    public void SaveCommonUI()
    {
        LogUtils.Log("SaveCommonUI");
    }

    public void ResumeCommonUI()
    {
        LogUtils.Log("ResumeCommonUI");
    }

    /// <summary>
    /// 显示加载等待
    /// </summary>
    /// <param name="duration"></param>
    public void ShowLoading(float duration = 6.0f)
    {
        GameManager.Instance.Pause = true;
        var reconnect = UIManager.Instance.FindByName("ReconnectWindow");
        if(reconnect != null && reconnect.IsShow())
            return;
        UIManager.Instance.ShowUIPanel("ReconnectWindow");
    }

    /// <summary>
    ///
    /// </summary>
    public void HideLoading()
    {
        GameManager.Instance.Pause = false;
        UIManager.Instance.CloseUIPanel("ReconnectWindow");
    }

    /// <summary>
    /// 显示LoadingUI
    /// </summary>
    /// <param name="actionDone"></param>
    public void ShowLoadingUI(System.Action actionDone)
    {
        actionLoadingDone = actionDone;
        ShowUIPanel("Loading");
    }

    public void DoLoadingUIDone()
    {
        if (actionLoadingDone != null)
        {
            actionLoadingDone.Invoke();
            actionLoadingDone = null;
        }
    }
    
    public void AddPackageIfNot(string packageName, Action onComplete)
    {
        var pkg = UIPackage.GetByName(packageName);

        if (pkg != null)
        {
            onComplete?.Invoke();
            return;
        }
        
        if (!Utils.IsLoadModelFromAssetBundle())
        {
            UIPackage.AddPackage("Assets/Editor Default Resources/UIPanel/" + packageName);

            onComplete?.Invoke();
        }
        else
        {
            ModelManager.Instance.LoadUIPanel(packageName, (name, ab) =>
            {
                if (ab != null)
                {
                    UIPackage.AddPackage(ab);
                }
                else
                {
                    LogUtils.LogError("AddPackageIfNot ab is null:" + packageName);
                }
                
                onComplete?.Invoke();
            });
        }
    }
    
    public void RemovePackage(string packageName)
    {
        UIPackage.RemovePackage(packageName);
    }

    public void SafeRemovePackage(string packageName)
    {
        if (!string.IsNullOrEmpty(packageName) && !packageName.Contains("Common") &&
            UIPackage.GetByName(packageName) != null)
        {
            UIPackage.RemovePackage(packageName);
        }
    }

    public UIViewBase ShowUIPanel(string name, params object[] values)
    {
        LogUtils.LogFormat("ShowUIPanel {0}", name);

        var ctrl = this.FindByName(name);

        if (ctrl != null)
        {
            ctrl.UpdateParams(values);
            // 界面已经存在，直接置顶
            this.TopController(ctrl, true);
        }
        else
        {
            // --- 创建界面
            var className = string.Format("{0}View", name);
            var type = Type.GetType(className);

            if (type != null)
            {
                ctrl = this.CreateController(name, type, true, values);
            }
            else
            {
                LogUtils.LogErrorFormat("ShowUIPanel Type Not Found ", name);
            }
        }

        return ctrl;
    }
    
    public void CloseUIPanel(string name)
    {
        LogUtils.LogFormat("CloseUIPanel {0}", name);

        var ctrl = this.FindByName(name);

        ctrl?.SetVisible(false);
    }
    
    public void CloseAllUIPanel()
    {
        GTween.Clean();
        foreach (var controller in controllers)
        {
            controller?.Destroy();
        }
        controllers.Clear();
        GameManager.Instance.StopAllCoroutines();
        NetManager.Instance.Cleanup();
        NetManager.Instance.OnInit();
    }
    
    public void CloseAllUIPanelExcept(string name)
    {
        foreach (var controller in controllers)
        {
            if(controller.name != name)
                controller?.Hide();
        }
    }

    public UIViewBase SpawnByName(string name, params object[] values)
    {
        LogUtils.LogFormat("SpawnByName {0}", name);

        var ctrl = this.FindByName(name);

        if (ctrl != null)
        {
            // 界面已经存在，直接置顶
            ctrl.UpdateParams(values);
            this.TopController(ctrl, false);
        }
        else
        {
            // --- 创建界面
            var className = string.Format("{0}View", name);
            var type = Type.GetType(className);

            if (type != null)
            {
                ctrl = this.CreateController(name, type, false, values);
            }
            else
            {
                LogUtils.LogErrorFormat("ShowUIPanel Type Not Found ", name);
            }
        }

        return ctrl;
    }

    public UIViewBase DestroyUIPanel(string name)
    {
        LogUtils.LogFormat("DestroyUIPanel {0}", name);

        var ctrl = this.FindByName(name);

        if (ctrl != null)
        {
            this.DestroyController(ctrl);
        }
        else
        {
            // 是否在异步队列
            var v = this.GetControllerAsyncLoad(name);

            if (v != null)
            {
                v.state = UIState.Dispose;
            }
        }

        return ctrl;
    }
    
    // 销毁UI
    public void DestroyController(UIViewBase ctrl)
    {
        if (ctrl == null)
        {
            return;
        }

        var IsTop = this.IsTopController(ctrl);
        this.controllers.Remove(ctrl);
        ctrl.Destroy();

        if (IsTop)
        {
            var ctrlTop = this.FindTopController();
            
            if (ctrlTop != null)
            {
                this.TopController(ctrlTop, false);
            }
        }
    }
    
    /// 获取UI（名称）
    public UIViewBase FindByName(string name)
    {
        foreach (var controller in controllers)
        {
            if (controller != null && controller.name.Equals(name))
            {
                return controller;
            }
        }

        return null;
    }
    /// <summary>
    /// 获取UI是否显示
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool IsShowByName(string name)
    {
        foreach (var controller in controllers)
        {
            if (controller != null && controller.name.Equals(name))
            {
                return controller.IsShow();
            }
        }

        return false;
    }

    protected UIViewBase GetControllerAsyncLoad(string name)
    {
        foreach (var controller in controllersAsyncLoad)
        {
            if (controller != null && controller.name.Equals(name))
            {
                return controller;
            }
        }

        return null;
    }

    protected void RecordControllerAsyncLoad(UIViewBase ctrl)
    {
        if (ctrl != null)
        {
            this.controllersAsyncLoad.Add(ctrl);
        }
    }

    protected void ClearControllerAsyncLoad(UIViewBase ctrl)
    {
        if (ctrl != null)
        {
            this.controllersAsyncLoad.Remove(ctrl);
        }
    }

    /// ---创建控制器（原型）
    protected UIViewBase CreateController(string name, Type type, bool show, params object[] values)
    {
        LogUtils.LogFormat("CreateController {0} {1}", type.Name, show);

        var ctrl = this.FindByName(name);

        if (ctrl != null)
        {
            return ctrl;
        }

        // --- 是否在异步队列
        var v = this.GetControllerAsyncLoad(name);

        if (v != null)
        {
            if (show)
            {
                v.state = UIState.Init;
            }
            else
            {
                v.state = UIState.Hide;
            }
            return v;
        }

        var newCtrl = Activator.CreateInstance(type) as UIViewBase;
        
        if (newCtrl != null)
        {
            newCtrl.name = name;
            
            if (show)
            {
                newCtrl.state = UIState.Init;
            }
            else
            {
                newCtrl.state = UIState.Hide;
            }
            
            // --- 加载UI
            if (string.IsNullOrEmpty(newCtrl.package) || string.IsNullOrEmpty(newCtrl.component))
            {
                LogUtils.LogErrorFormat("ui resource not set:" + newCtrl.name);
                this.OnCreateControllerDone(newCtrl, values);
            }
            else
            {
                // --- 记录异步队列
                this.RecordControllerAsyncLoad(newCtrl);
                AddPackageIfNot(newCtrl.package, () =>
                {
                    newCtrl.BindAll();
                    // 已经被外层销毁, 不继续创建
                    if (newCtrl.state != UIState.Dispose)
                    {
                        UIPackage.CreateObjectAsync(newCtrl.package, newCtrl.component,
                            (go) =>
                            {
                                // 清除异步队列记录
                                this.ClearControllerAsyncLoad(newCtrl);
                                // 异步加载成功
                                newCtrl.main = go.asCom;
                                newCtrl.main.fairyBatching = newCtrl.fairyBatching;
                                newCtrl.main.gameObjectName = newCtrl.name;
                                newCtrl.main.MakeFullScreen();
                                this.OnCreateControllerDone(newCtrl, values);
                            }
                        );
                    }
                });
            }
        }
        
        return newCtrl;
    }

    protected void OnCreateControllerDone(UIViewBase ctrl, params object[] values)
    {
        if (ctrl == null)
        {
            return;
        }

        var oldTop = this.FindTopController();
        var indexNew = this.GetTopControllerIndexByUIType(ctrl.type);
        var indexNow = Mathf.Max(indexNew + 1, 0);
        var indexMax = this.controllers.Count;
        if (indexNow >= indexMax)
        {
            this.controllers.Add(ctrl);
        }
        else
        {
            this.controllers.Insert(indexNow, ctrl);
        }

        GRoot.inst.AddChild(ctrl.main);
        this.SyncControllerIndex();

        ctrl.Init(values);

        if (ctrl.state == UIState.Hide || ctrl.state == UIState.Dispose)
        {
            ctrl.Hide();
        }
        else
        {
            ctrl.Show();
        }

        if (indexNow >= indexMax && ctrl.IsShow())
        {
            if (oldTop != null)
            {
                oldTop.UnFocus();
            }

            ctrl.OnFocus();
        }
    }

    public void SyncControllerIndex()
    {
        // --- 因为C#层可能也会操作GRoot，无法保证UIManager.controllers与GRoot序号完全一致
        // --- 因此以UIManager.controllers为模版保证显示顺序即可，需要遍历实现
        var indexLast = -1;
        
        for (int i = 0; i < this.controllers.Count; i++)
        {
            var ctrl = this.controllers[i];

            if (ctrl != null && ctrl.main != null)
            {
                var indexTmp = GRoot.inst.GetChildIndex(ctrl.main);
                if (indexTmp < 0)
                {
                    // --- 不存在GRoot中,跟在前一个的后面
                    GRoot.inst.AddChildAt(ctrl.main, indexLast + 1);
                }
                else
                {
                    if (indexTmp < indexLast)
                    {
                        // --- 排序异常,移动排序
                        GRoot.inst.SetChildIndex(ctrl.main, indexLast);
                    }
                }
                indexLast = GRoot.inst.GetChildIndex(ctrl.main);
            }
        }
    }

    // ---获取顶层UI
    public UIViewBase FindTopController()
    {
        var num = this.controllers.Count;

        if (num <= 0)
        {
            return null;
        }

        for (int i = this.controllers.Count - 1; i >= 0; i--)
        {
            var go = this.controllers[i];

            if (go != null && go.type != UIType.Tip && go.type != UIType.Newbie && go.IsShow() && go.name != "FightWin")
            {
                return go;
            }
        }

        return null;
    }

    public bool IsTopController(UIViewBase ctrl)
    {
        return this.FindTopController() == ctrl;
    }
    
    public bool IsTopController(string ctrlName)
    {
        var ctrl = UIManager.Instance.FindByName(ctrlName);
        if(ctrl != null)
            return this.FindTopController() == ctrl;
        return false;
    }
    
    // ---根据UI类型获取顶层index
    public int GetTopControllerIndexByUIType(UIType uiType)
    {
        var num = this.controllers.Count;
        
        if (num <= 0)
        {
            return 0;
        }

        var index = 0;

        for (int i = this.controllers.Count - 1; i >= 0; i--)
        {
            var go = this.controllers[i];
            
            if (go != null)
            {
                if (go.type <= uiType)
                {
                    index = i;
                    break;
                }
            }
        }

        return index;
    }

    public int GetControllerIndex(UIViewBase ctrl)
    {
        return this.controllers.IndexOf(ctrl);
    }
        
    // ---置顶UI
    public void TopController(UIViewBase ctrl, bool show)
    {
        if (ctrl == null)
        {
            return;
        }

        var indexLast = this.GetControllerIndex(ctrl);

        if (indexLast < 0)
        {
            return;
        }

        var oldTop = this.FindTopController();

        if (oldTop != ctrl)
        {
            this.controllers.Remove(ctrl);
            var indexNew = this.GetTopControllerIndexByUIType(ctrl.type);
            var indexNow = Math.Max(indexNew + 1, 0);
            var indexMax = this.controllers.Count;
            if (indexNow > indexMax)
            {
                this.controllers.Add(ctrl);
            }
            else
            {
                this.controllers.Insert(indexNow, ctrl);
            }

            this.SyncControllerIndex();
        }

        if (show)
        {
            if (!ctrl.IsShow())
            {
                ctrl.Show();
            }
            
            if (oldTop != null && oldTop != ctrl)
            {
                oldTop.UnFocus();
            }
            
            ctrl.OnFocus();
        }
        else
        {
            if (oldTop != null && oldTop != ctrl)
            {
                oldTop.UnFocus();
            }
            
            if (ctrl.IsShow())
            {
                ctrl.OnFocus();
            }
        }
    }
    
    /// <summary>
    /// 小弹窗提示  读取string表的配置
    /// </summary>
    /// <param name="key"></param>
    public void ToastByKey(int key)
    {
        Toast(ConfigUtils.GetStringByKey(key));
    }
    
    public void Toast(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            TipsManger.Instance.ShowTip(value);
            //ShowUIPanel("Toast", value);
        }
        else
        {
            LogUtils.LogWarning("value does not exist");
        }
    }
    
    public void Toast(string iconUr,string value)
    {
        if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(iconUr))
        {
            ShowUIPanel("Toast", iconUr, value);
        }
        else
        {
            LogUtils.LogWarning("value does not exist");
        }
    }

    public void CloseAllVillagePanel()
    {
        foreach (var controller in controllers)
        {
            if (controller.FuncType == FuncType.village)
            {
                CloseUIPanel(controller.name);
            }
        }
    }
    /// <summary>
    /// 关闭非引导需要的界面
    /// </summary>
    public void CloseAllNotGuidePanel()
    {
        foreach (var controller in controllers)
        {
            if (controller.GuideType != FuncType.Guide)
            {
                CloseUIPanel(controller.name);
            }
        }
    }

    private static float HIDE_LOADING_TIME = 0.50f;
    private List<int> _messageIDList = new List<int>();
    private bool _isShowLoading = false;
    public void ShowSCLoading(int msgId)
    {
        // Debug.LogWarningFormat("==显示Loading msgId={0}  time= {1}", msgId, UnityEngine.Time.time);
        _messageIDList.Add(msgId);
        if (_messageIDList.Count > 0 && !_isShowLoading)
        {
            _isShowLoading = true;
            GameManager.Instance.TimerManager.ClearTimer(SetSCLoading);
            GameManager.Instance.TimerManager.SetTimer(HIDE_LOADING_TIME, SetSCLoading);
        }
    }

    public void HideSCLoading(int msgId)
    {
        // Debug.LogWarningFormat("==关闭Loading msgId={0}  time= {1}", msgId, UnityEngine.Time.time);
        if (_messageIDList.Contains(msgId-1))
        {
            _messageIDList.Remove(msgId-1);
        }

        if (_messageIDList.Count == 0 && _isShowLoading)
        {
            _isShowLoading = false;
            UIManager.Instance.CloseUIPanel("SCLoading");
            GameManager.Instance.TimerManager.ClearTimer(SetSCLoading);
        }

    }

    private void SetSCLoading()
    {
        UIManager.Instance.ShowUIPanel("SCLoading");
    }

    public void SendToApplyRecharge(int paylistId)
    {
        var builder = ApplyRechargeGameOrder_CS.CreateBuilder();
        builder.PaylistId = paylistId;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ApplyRechargeGameOrder_CS, builder.Build());
    }
    public void Pay(ConfigPayListUnit payListUnit, string paylistId, string orderCpId)
    {
        PayManager.Instance.Pay(payListUnit.XmProductId, payListUnit.Desc, paylistId, double.Parse((payListUnit.RechargeAmount/100f).ToString("f2")), "USD", orderCpId, (
            paylistId =>
            {
                var builder = CommitGameOrder_CS.CreateBuilder();
                builder.PaylistId = int.Parse(paylistId);
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_CommitGameOrder_CS, builder.Build());
            }));
    }

    public void WatchAd(Action watchSuccess)
    {
        AdManager.Instance.WatchAd(watchSuccess);
    }
    
}