
using System.Collections.Generic;
using Common;
using Engine;
using FairyGUI;
/// <summary>
/// 新功能解锁提示界面
/// </summary>
public class FuncOpenView : UIViewBase
{
    private UI_FuncOpen FuncOpen => this.main as UI_FuncOpen;
    private List<int> _funcIds;
    private float _timer;
    public FuncOpenView()
    {
        this.type = UIType.Normal;
        this.name = "FuncOpen";
        this.package = "Common";
        this.component = "FuncOpen";
        this.type = UIType.Tip;
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _funcIds = values[0] as List<int>;
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.FuncOpen.sortingOrder = 888888;
        this.FuncOpen.funcList.itemRenderer = FuncOpenItemRender;
    }

    protected override void OnShow()
    {
        base.OnShow();
        
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.FunctionUnlockSE);
        
        _timer = 0;
        this.FuncOpen.funcList.numItems = _funcIds.Count;
        this.FuncOpen.img.url = UIResource.GetImageUrlWithLang("g4", "Common");
    }

    private void FuncOpenItemRender(int index, GObject item)
    {
        int funcId = _funcIds[index];
        FunPrevInfo funInfo = FuncPreviewManger.Instance.GetFunInfo(funcId);
        ((UI_FuncOpenItem)item).funcIcon.url = UIResource.GetFuncPreIcon(funInfo.SystemUnit.Icon.ToString());
        // ((UI_FuncOpenItem)item).funcName.text = funInfo.SystemUnit.Name;
        ((UI_FuncOpenItem)item).funcName.text = ConfigUtils.GetTextById(funInfo.SystemUnit.Name,funInfo.SystemUnit.NameParam);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        _timer += UnityEngine.Time.deltaTime;
        if (_timer > 2.0f)
        {
            _timer = 0;
            UIManager.Instance.CloseUIPanel("FuncOpen");
        }
    }
}
