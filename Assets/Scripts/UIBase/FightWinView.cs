using System.Collections;
using System.Collections.Generic;
using Engine;
using FairyGUI;
using FightLoseAndWin;
using UnityEngine;

public class FightWinView : UIViewBase
{
    private UI_FightWinWindow FightWin => this.main as UI_FightWinWindow;
    
    private Coroutine _timerCoroutine;// 添加协程引用
    private int time;
    
    public FightWinView()
    {
        this.type = UIType.Normal;
        this.name = "FightWin";
        this.package = "FightLoseAndWin";
        this.component = "FightWinWindow";
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
        this.FightWin.nextStageBtn.onClick.Add(this.nextStageBtnOnClick);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        // 停止计时器协程
        if (_timerCoroutine != null) 
        {
            GameManager.Instance.StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
    }

    protected override void OnHide()
    {
        base.OnHide();
        // 停止计时器协程
        if (_timerCoroutine != null) 
        {
            GameManager.Instance.StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralWinSE);
        
        time = 10;
        this.FightWin.spine.spineAnimation.skeleton.SetToSetupPose();
        this.FightWin.spine.spineAnimation.state.ClearTracks();
        this.FightWin.spine.spineAnimation.state.SetAnimation(0, "idle", false);

        this.FightWin.desc.text = ConfigUtils.GetStringByKey(10201);
        // 启动计时器协程
        _timerCoroutine = GameManager.Instance.StartCoroutine(UpdateTimerCoroutine());
        UpdateShowTime();
    }

    private IEnumerator UpdateTimerCoroutine()
    {
        while (time >= 0)
        {
            time--;
            UpdateShowTime();
            yield return new WaitForSeconds(1f); // 每秒更新一次
        }
    }

    private void UpdateShowTime()
    {
        this.FightWin.timeLb.SetVar("value", time.ToString()).FlushVars();

        if (time < 0)
        {
            UIManager.Instance.CloseUIPanel("FightWin");
        }
    }
    
    private void nextStageBtnOnClick()
    {
        UIManager.Instance.CloseUIPanel("FightWin");
        
        // var view = UIManager.Instance.FindByName("Lobby") as LobbyView;
        // if (view != null)
        // {
        //     GList bottomList= view?.GetBottomList();
        //     GButton btn = bottomList.GetChildAt(2) as GButton;
        //     if(btn != null)
        //         btn.selected = true;
        //     view?.ChangeIndex(2, true, MapObjectManager.Instance.GuanKaStageId + 1);
        // }
        
        // 自动挑战下一关  ---- 当前为1-1，挑战1-2
        // var view = UIManager.Instance.FindByName("Lobby") as LobbyView;
        // if (view != null)
        // {
        //     GList bottomList= view?.GetBottomList();
        //     GButton btn = bottomList.GetChildAt(2) as GButton;
        //     if (btn != null)
        //     {
        //         btn.data = MapObjectManager.Instance.GuanKaStageId + 1;
        //         EventContext context = new EventContext();
        //         context.data = btn;
        //         view?.OnClickBottomItem(context);
        //     }
        //     
        // }
        
        // 自动挑战关卡 -----  挑战大地图上有黄色标记的关卡
        var view = UIManager.Instance.FindByName("Lobby") as LobbyView;
        if (view != null)
        {
            GList bottomList= view?.GetBottomList();
            GButton btn = bottomList.GetChildAt(2) as GButton;
            if (btn != null)
            {
                EventContext context = new EventContext();
                context.data = btn;
                view?.OnClickBottomItem(context);

                float time = 0.1f;
                if (!MapChapterManager.Instance.isLoadComplete) time = 3f;
                
                // 延迟等待界面加载完
                GameManager.Instance.TimerManager.SetTimer(time, () =>
                {
                    var mapView = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
                    if (mapView != null)
                    {
                        mapView?.OnClickFightCurStageBtn();
                    }
                });
            }
            
        }
        
    }
    
}
