using System.Collections.Generic;
using Config;
using Engine;
using FairyGUI;
using FightLoseAndWin;

public class FightLoseView : UIViewBase
{
    private UI_FightLoseWindow FightLose => this.main as UI_FightLoseWindow;
    private List<ConfigFailurePromptUnit> _failureList;
    private List<ConfigFailurePromptUnit> _curFailureGoList;
    
    /// <summary>
    /// 防止多次调用到OnHide关闭
    /// </summary>
    private bool isClose = false;
    
    public FightLoseView()
    {
        this.type = UIType.Top;
        this.name = "FightLose";
        this.package = "FightLoseAndWin";
        this.component = "FightLoseWindow";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        FightLoseAndWinBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        _failureList = ConfigUtils.GetFailurePromptUnits();
        _curFailureGoList = new List<ConfigFailurePromptUnit>();
        this.FightLose.failGoList.itemRenderer = FailGoListRender;
    }

    protected override void OnShow()
    {
        base.OnShow();
        
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralLoseSE);
        
        _curFailureGoList.Clear();
        foreach (var item in _failureList)
        {
            if (item.FunctionEnabled == 0)
            {
                _curFailureGoList.Add(item);
            }
            else if (item.FunctionEnabled > 0 && FuncPreviewManger.Instance.FunIsOpened(item.FunctionEnabled))
            {
                _curFailureGoList.Add(item);
            }
        }
        _curFailureGoList.Sort((a, b) =>
        {
            int result = a.Priority > b.Priority ? -1 : (a.Priority == b.Priority ? 0 : 1);
            if (result == 0)
                result = a.Id > b.Id ? -1 : 1;
            return result;
        });
        this.FightLose.failGoList.numItems = _curFailureGoList.Count;
        if(_curFailureGoList.Count > 0)
            this.FightLose.failGoList.ScrollToView(0);
        this.FightLose.failGoList.opaque = false;
        // this.FightLose.img.url = UIResource.GetImageUrlWithLang("sb", "FightLoseAndWin");
        isClose = true;
    }

    private void FailGoListRender(int index, GObject item)
    {
        ConfigFailurePromptUnit promptUnit = _curFailureGoList[index];
        ((UI_FailItem) item).iconLoader.url = UIResource.GetFuncPreIcon(promptUnit.Icon);
        // ((UI_FailItem) item).descLb.text = promptUnit.Desc;
        ((UI_FailItem) item).descLb.text = ConfigUtils.GetTextById(promptUnit.Desc);
        ((UI_FailItem) item).tjCtrl.selectedIndex = promptUnit.Priority == 1 ? 0 : 1;
        ((UI_FailItem) item).gotoBtn.data = promptUnit;
        ((UI_FailItem) item).gotoBtn.onClick.Set(this.OnClickToGoStrength);
    }

    private void OnClickToGoStrength(EventContext context)
    {
        ConfigFailurePromptUnit promptUnit =(context.sender as GButton)?.data as ConfigFailurePromptUnit;
        //引导类型
        //1=跳转，2=手指
        if (promptUnit != null)
        {
            LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
            if (promptUnit.Type == 1)
            {
                switch (promptUnit.JumpPath)
                {
                    case "SkillToZhuzhao":
                        lobbyView?.OpenBottomPanelToEquipMake();
                        break;
                    case "SkipToSummonPet":
                        lobbyView?.OpenBottomPanel(4, 0);
                        break;
                    case "SkillToSummonSkill":
                        lobbyView?.OpenBottomPanel(4, 0);
                        break;
                    case "SkillToSummonHero":
                        UIManager.Instance.ShowUIPanel("SummonHero");
                        break;
                    case "SkillToDungeon":
                        // lobbyView?.OpenBottomPanel(3, 0);
                        UIManager.Instance.ShowUIPanel("DungeonStage");
                        break;
                }
            }
            else
            {
                //如果是2=手指 需要注明是什么功能 ，具体一个个写，要是主界面上的
                if (promptUnit.JumpPath == "ClickAttr")
                {
                    lobbyView?.ShowClickAttrFinger();
                }

            }
        }
        
        SetVisible(false);
    }
}
