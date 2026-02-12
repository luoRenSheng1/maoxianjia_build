
using System.Collections;
using BigMap;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using Spine.Unity;
using UnityEngine;

public class FishingMainView : UIViewBase
{
    private UI_FishingMain _fishingMain => this.main as UI_FishingMain;

    private ConfigCommonUnit _common2011;
    private int _fishingTime;//垂钓时长
    private bool _hasClicked;//是否点击收杆
    private int widthVal = 15;

    private Coroutine _pulseCoroutine;
    private float _palyBtnTime;
    private bool isGuide = false;//引导标志
    private GTweener tweener = null;

    public FishingMainView()
    {
        this.name = "FishingMain";
        this.package = "BigMap";
        this.component = "FishingMain";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        BigMapBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        _common2011 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2011);
        this._fishingMain.fishingBtnAni.onClick.Add(OnClickFishingBtnAni);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        GTween.Kill(_fishingMain.fishingBar.fishFlag);
        GTween.Kill(_fishingMain);

        if (_pulseCoroutine != null)
        {
            GameManager.Instance.StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = null;
        }
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
    }

    protected override void OnHide()
    {
        base.OnHide();
        GTween.Kill(_fishingMain.fishingBar.fishFlag);
        GTween.Kill(_fishingMain);

        if (_pulseCoroutine != null)
        {
            GameManager.Instance.StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = null;
        }
    }

    protected override void OnShow()
    {
        base.OnShow();

        isGuide = false;
        tweener = null;
        _hasClicked = false;
        _fishingMain.alpha = 1f;
        _fishingMain.visible = true;

        _fishingTime = int.Parse(_common2011.Param2);//垂钓时长
        _palyBtnTime = _fishingTime;

        // 收杆按钮状态
        ShowFishingBtnAni();

        // 杀掉之前所有 tween
        GTween.Kill(_fishingMain.fishingBar.fishFlag, true);
        GTween.Kill(_fishingMain, true);

        // 重置 fishFlag 位置
        float bgStartX = _fishingMain.fishingBar.bg.x;
        _fishingMain.fishingBar.fishFlag.x = bgStartX - widthVal;

        AllotColorArea();


        //钓鱼引导-强制暂停钓鱼进度条，点击收杆按钮
        GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.Angling,
            giding = GuideID.guideId_3602,
            gid = GuideID.guideId_3603,
            tui = this._fishingMain.fishingBtnAni,
            isForce = true,
            isSend = true,
            //isLucency = true,//透明强制引导的黑色遮罩，但只能点击手指指向区域
            //scale = new Vector2(1f, 1.5f),
            //npcTxt = "Beginner_Doc_018",
            //npcPosType = PosType.Down,
            //gType = global::GuideType.Wait,
            cb = () =>
            {
                //暂停进度条
                isGuide = true;
            },
            touchCB = () =>
            {
                GuideManager.Instance.HideGuide();
            }
        });
    }

    /// <summary>
    /// 收杆按钮动画
    /// </summary>
    private void ShowFishingBtnAni()
    {
        // 停止现有的协程
        if (_pulseCoroutine != null)
        {
            GameManager.Instance.StopCoroutine(_pulseCoroutine);
        }

        _pulseCoroutine = GameManager.Instance.StartCoroutine(PulseAnimation());
    }

    private IEnumerator PulseAnimation()
    {
        while (_palyBtnTime > 0)
        {
            _palyBtnTime -= 0.25f;
            this._fishingMain.fishingBtnAni.t0.Play();
            yield return new WaitForSeconds(0.25f);
        }
    }

    /// <summary>
    /// 收杆按钮
    /// </summary>
    private void OnClickFishingBtnAni()
    {
        if (_hasClicked) return;
        _hasClicked = true;

        float fishX = _fishingMain.fishingBar.fishFlag.x;
        float greenStart = _fishingMain.fishingBar.greenBar.x;
        float greenEnd = greenStart + _fishingMain.fishingBar.greenBar.width;

        bool success = fishX >= greenStart && fishX <= greenEnd;

        EndFishing(success);
    }

    /// <summary>
    /// 分配颜色区域
    /// </summary>
    private void AllotColorArea()
    {
        float total = _fishingTime;

        // 初始化鱼的位置
        float bgStartX = _fishingMain.fishingBar.bg.x;
        float bgWidth = _fishingMain.fishingBar.bg.width;

        float startX = bgStartX - widthVal;
        float endX = bgStartX + bgWidth - widthVal;

        // 强制重置 fishFlag 位置
        _fishingMain.fishingBar.fishFlag.x = startX;

        // 生成随机比例
        float[] randomRates = GenerateRandomRates(3);// 随机比例总和为1
        float yellowRate = randomRates[0];
        float greenRate = randomRates[1];
        float blueRate = randomRates[2];

        // 设置颜色区域宽度（比例宽度）
        float yellowWidth = (bgWidth - 2 * widthVal) * yellowRate;
        float greenWidth = (bgWidth - 2 * widthVal) * greenRate;
        float blueWidth = (bgWidth - 2 * widthVal) * blueRate;

        _fishingMain.fishingBar.yellowBar.width = yellowWidth;
        _fishingMain.fishingBar.greenBar.width = greenWidth;
        _fishingMain.fishingBar.blueBar.width = blueWidth;

        // 设置三段区域的位置（依次排列:黄、绿、蓝）
        _fishingMain.fishingBar.yellowBar.x = bgStartX + widthVal;
        _fishingMain.fishingBar.greenBar.x = bgStartX + widthVal + yellowWidth;
        _fishingMain.fishingBar.blueBar.x = bgStartX + widthVal + yellowWidth + greenWidth;

        float guideX = _fishingMain.fishingBar.greenBar.x + _fishingMain.fishingBar.greenBar.width * 0.5f;

        //  鱼flag动画（从左到右）
        tweener = GTween.To(startX, endX, total)
            .SetEase(EaseType.Linear)
            .SetTarget(_fishingMain.fishingBar.fishFlag)
            .OnUpdate((t) =>
            {
                float posX = (float)t.value.x;
                if (isGuide)
                {
                    //引导过程锁定在一个地方
                    _fishingMain.fishingBar.fishFlag.x = guideX;
                    tweener.Kill();
                    tweener = null;
                }
                else
                {
                    _fishingMain.fishingBar.fishFlag.x = posX;
                }
            })
            .OnComplete(() =>
            {
                if (!_hasClicked)
                {
                    EndFishing(false); // 自动失败
                }
            });

    }

    private void EndFishing(bool success)
    {
        if (success)
        {
            Debug.Log("钓鱼成功！");
            MapChapterManager.Instance._fishingSuccess = true;
            MapChapterManager.Instance.SendBeginFishing();
        }
        else
        {
            Debug.Log("钓鱼失败！");
            MapChapterManager.Instance._fishingSuccess = false;
            MapChapterManager.Instance.SendFishingFailed();
            UIManager.Instance.ShowUIPanel("FishingFail");
        }

        GObject panel = this._fishingMain;
        panel.alpha = 1f;
        // 渐隐关闭界面
        GTween.Kill(panel);
        GTween.To(1f, 0f, 0.4f)
            .SetTarget(panel)
            .OnUpdate(t =>
            {
                float val = (float)t.value.x;
                panel.alpha = val;
            })
            .OnComplete(() =>
            {
                SetHeroState();
                UIManager.Instance.CloseUIPanel("FishingMain");
                var view = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
                view?.StopFishingCoroutine();
            });
    }

    /// <summary>
    /// 设置钓完鱼之后英雄的状态
    /// </summary>
    private void SetHeroState()
    {
        var view = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
        SkeletonAnimation heroAni = view?.GetHeroAni();
        heroAni.state.SetAnimation(0, HeroState.diaoyv4.ToString(), false);
        heroAni.state.AddAnimation(0, HeroState.idle.ToString(), true, 0f);
    }

    private float[] GenerateRandomRates(int count)
    {
        float[] values = new float[count];
        float total = 0f;
        for (int i = 0; i < count; i++)
        {
            values[i] = UnityEngine.Random.Range(0.2f, 1f); // 避免过小
            total += values[i];
        }

        for (int i = 0; i < count; i++)
        {
            values[i] /= total;
        }

        return values;
    }

}
