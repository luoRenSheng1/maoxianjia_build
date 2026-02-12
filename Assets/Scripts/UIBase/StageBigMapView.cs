
using System;
using System.Collections;
using System.Collections.Generic;
using BigMap;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Spine.Unity;
using UnityEngine;

public class StageBigMapView : UIViewBase
{
    private UI_StageBigMap StageBigMap => this.main as UI_StageBigMap;
    
    private Dictionary<string, List<Vector2>> _mapPathDict = new Dictionary<string, List<Vector2>>();
    private List<string> _mapTagList = new List<string>(){"a", "b", "c", "d","e","f"};

    private string _preMapTag;
    private int _preStageIndex;
    private string _curMapTag;
    private int _curStageIndex;
    
    private SkeletonAnimation _heroSpine; 
    
    private Coroutine timeCoroutine;
    private int currentTime; //倒计时
    
    public StageBigMapView()
    {
        this.type = UIType.Top;
        this.name = "StageBigMap";
        this.package = "BigMap";
        this.component = "StageBigMap";
        this.removePackage = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        BigMapBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _curMapTag = (string) values[0];
        _curStageIndex = (int) values[1];
        _preMapTag = (string) values[2];
        _preStageIndex = (int) values[3];
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.StageBigMap.sortingOrder = 99999;
        for (int i = 0; i < _mapTagList.Count; i++)
        {
            int index = 1;
            List<Vector2> path = new List<Vector2>();
            GObject gloader = this.StageBigMap.bgiMap.GetChild(_mapTagList[i] + index);
            while (gloader != null)
            {
                if (gloader != null && _mapTagList[i] != null )
                {
                    gloader.touchable = true;
                    gloader.data = (_mapTagList[i] + (index-1)).ToString();
                    gloader.onClick.Add(OnClickMapPos);
                }
                
                path.Add(gloader.xy);
                index++;
                gloader = this.StageBigMap.bgiMap.GetChild(_mapTagList[i] + index);
            }
            _mapPathDict.Add(_mapTagList[i], path);
            
        }

        this.StageBigMap.touchable = true;
        
        this.StageBigMap.bgiMap.bg.touchable = true;
        this.StageBigMap.bgiMap.bg.onClick.Add(() =>
        {
            StopTimeCoroutine();
            UIManager.Instance.DestroyUIPanel("StageBigMap");
            EngineBase.EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_STAGE_COMPLETE_RECV_SUCCESS);
        });
        
        EngineBase.EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CHOOSE_STAGE_RECV_CALLBACK, this.ChooseStageRecvCallback);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EngineBase.EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CHOOSE_STAGE_RECV_CALLBACK, this.ChooseStageRecvCallback);
    }

    //选关回调失败，按原来的流程开始下一关
    private void ChooseStageRecvCallback()
    {
        LogUtils.Log("==ChooseStageRecvCallback==");
        StartMove();
    }
    
    protected override void OnShow()
    {
        base.OnShow();
        TipsManger.Instance.ClosePopupTip();

        ConfigCommonUnit commonUnit900001 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(900001);
        currentTime = int.Parse(commonUnit900001.Param1); //读取common表 配置倒计时时间
        timeCoroutine = GameManager.Instance.StartCoroutine(StartCountDown());
        OnReadMove();
    }

    private void OnReadMove(string aniName = "idle",bool isMove = false)
    {
        try
        {
            Vector2 prePos = _mapPathDict[_preMapTag][_preStageIndex];
            Vector2 curPos = _mapPathDict[_curMapTag][_curStageIndex];
            this.StageBigMap.bgiMap.hero.SetXY(prePos.x, prePos.y);
            HeroInfo myHero = HeroInfoManager.Instance.GetMyHero();
            Utils.SetSpineModelOnFGUI(this.StageBigMap.bgiMap.hero, myHero.HeroUnit.Model, 60f, aniName, (o) =>
            {
                if(o is SkeletonAnimation animation)
                    _heroSpine = animation;
                // this.StageBigMap.scrollPane.ScrollToView(this.StageBigMap.bgiMap.hero, false);
                // Debug.Log(curPos.x + " " + curPos.y);
                this.StageBigMap.scrollPane.posX = curPos.x - 300;
                this.StageBigMap.scrollPane.posY = curPos.y - 400;
                this.StageBigMap.txtTime.SetXY(this.StageBigMap.scrollPane.posX, this.StageBigMap.scrollPane.posY);
                Vector3 forward = curPos - prePos;
                forward.y = 0;
                var rotNow = Quaternion.LookRotation(forward);
                _heroSpine.skeleton.ScaleX = rotNow.eulerAngles.y > 180.0f ? -1.0f : 1.0f;

                if (isMove)
                {
                    Vector2 curPos = _mapPathDict[_curMapTag][_curStageIndex];
                    GTweener tweener = this.StageBigMap.bgiMap.hero.TweenMove(curPos, 2f);
                    tweener.OnComplete(() =>
                    {
                        UIManager.Instance.CloseUIPanel("StageBigMap");
                        EngineBase.EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_STAGE_COMPLETE_RECV_SUCCESS);
                    });
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("地图配置错误{0}  {1}  {2}  {3}",_preMapTag,_preStageIndex,_curMapTag, _curStageIndex);
            EngineBase.EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_STAGE_COMPLETE_RECV_SUCCESS);
        }
    }

    //开始移动
    private void StartMove()
    {
        Utils.ClearSpineModelOnFGUI(this.StageBigMap.bgiMap.hero);
        OnReadMove("run", true);
    }
    
    //点击关卡位置
    private void OnClickMapPos(EventContext context)
    {
        StopTimeCoroutine();
        GLoader clickedLoader = context.sender as GLoader;
        LogUtils.Log("=================index="+clickedLoader.data);

        string sClickMapTag = clickedLoader.data.ToString().Substring(0, 1);
        char cClickMapTag = char.Parse(sClickMapTag);
        int clickStageIndex = int.Parse(clickedLoader.data.ToString().Substring(1, 1));
        char curMapTag = char.Parse(_curMapTag);

        //当前要进入的下一关
        int stageId = DataManager.Instance.mRoleData.stageId;
        // int chapterId = DataManager.Instance.mRoleData.chapterId;
        // int GuanKaMonsterIndex = MapObjectManager.Instance.GuanKaMonsterIndex;
        //当前要进入stage的数据  
        ConfigStageUnit curStageUnit = ConfigUtils.GetStageUnitByIdAndNode(stageId, MapObjectManager.Instance.GuanKaMonsterIndex);
        
        
        int stageCount = 0;
        int selectId = curStageUnit.Id;  //关卡表ID
        if (cClickMapTag == curMapTag) //同一个小地图中选
        {
            int num = _curStageIndex - clickStageIndex;
            int reduceIdNum = num * 5;
            selectId = curStageUnit.Id - reduceIdNum;
        }
        else
        {
            //TODO 策划还没确定具体实现 大等级 后面再改
            if (cClickMapTag > curMapTag)  //向前进的地图选
            {
                bool isStart = false;
                for (int i = 0; i < _mapTagList.Count; i++)
                {
                    if (_curMapTag == _mapTagList[i])
                    {
                        isStart = true;
                    }

                    if (isStart)
                    {
                        for (int j = 0; j < _mapPathDict[_mapTagList[i]].Count; j++)
                        {
                            if (_curMapTag == _mapTagList[i])
                            {
                                if (j > _curStageIndex ) //当前的数据是准备进入下一关的所以不用等于
                                {
                                    stageCount++;
                                }
                            }
                            else if (sClickMapTag == _mapTagList[i])
                            {
                                if (j <= clickStageIndex)
                                {
                                    stageCount++;
                                }
                            }
                            else
                            {
                                stageCount++;
                            }
                        }
                        
                        if (sClickMapTag == _mapTagList[i])
                        {
                            break;
                        }
                    }
                }
                LogUtils.Log("=前进几格=stageCount="+stageCount);
                if (stageCount > 0)
                {
                    int reduceIdNum = stageCount * 5;
                    selectId = curStageUnit.Id + reduceIdNum;
                }
            }
            else //向后退的地图选
            {
                bool isStart = false;
                for (int i = 0; i < _mapTagList.Count; i++)
                {
                    if (sClickMapTag == _mapTagList[i])
                    {
                        isStart = true;
                    }

                    if (isStart)
                    {
                        for (int j = 0; j < _mapPathDict[_mapTagList[i]].Count; j++)
                        {
                            if (sClickMapTag == _mapTagList[i])
                            {
                                if (j >= clickStageIndex)
                                {
                                    stageCount++;
                                }
                            }
                            else if (_curMapTag == _mapTagList[i])
                            {
                                if (j < _curStageIndex)
                                {
                                    stageCount++;
                                }
                            }
                            else
                            {
                                stageCount++;
                            }
                        }
                        
                        if (_curMapTag == _mapTagList[i])
                        {
                            break;
                        }
                    }
                }
                LogUtils.Log("=后退几格=stageCount="+stageCount);
                if (stageCount > 0)
                {
                    int reduceIdNum = stageCount * 5;
                    selectId = curStageUnit.Id - reduceIdNum;
                }
            }
        }
        
        // 选中的关卡数据
        ConfigStageUnit selStageUnitData = ConfigUtils.GetStageUnitByIndexId(selectId);
        if (selStageUnitData == null)
        {
            Debug.LogWarning("selStageUnitData is null, selectId=" + selectId);
            StartMove();
            return;
        }
        
        //最高通过的关卡
        int passStageIdMax = DataManager.Instance.GetRoleData().latestPassedStageId;
        List<ConfigStageUnit> passStageUnits = ConfigUtils.GetStageUnitById(passStageIdMax);
        ConfigStageUnit passStageMaxData = passStageUnits[passStageUnits.Count-1];
        //可以选择的下一关数据
        ConfigStageUnit nextStageData = ConfigUtils.GetStageUnitByIndexId(passStageMaxData.Id + 1);
        if (nextStageData == null)
        {
            Debug.LogWarning("nextStageData is null, passStageMaxData.id=" + (passStageMaxData.Id + 1));
            StartMove();
            return;
        }
        int nextStageId = nextStageData.LevelId;
        
         if (selStageUnitData.LevelId > nextStageId)  //未通关的关卡
        {
            UIManager.Instance.ToastByKey(8001);
            return;
        }
        else if(stageId == nextStageId && selStageUnitData.LevelId == nextStageId) 
        {
            //选中的刚好是下一关还没完成的关卡
            StartMove();
            return;
        }
        else
        {
            var builder = ChooseStage_CS.CreateBuilder();
            builder.StartStageId = (ulong)selStageUnitData.LevelId; //1001;
            builder.StartChapterId = (ulong)selStageUnitData.Chapter; //1000;
            Debug.Log("=发送数据===selectId="+selStageUnitData.Id+"==LevelId="+selStageUnitData.LevelId+"==ChapterId="+selStageUnitData.Chapter);
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ChooseStage_CS, builder.Build()); //通知服务器当前关卡完成
        }
    }
    
    /// <summary>
    /// 开启倒计时协程
    /// </summary>
    /// <returns></returns>
    IEnumerator StartCountDown()
    {
        if (currentTime > 0)
        {
            this.StageBigMap.txtTime.visible = true;
        }
        while (currentTime > 0)
        {
            this.StageBigMap.txtTime.SetVar("time", currentTime.ToString()).FlushVars();
            yield return GameManager.Instance.waitSec1;
            currentTime--;
        }
        this.StageBigMap.txtTime.SetVar("time", "0").FlushVars();
        this.StageBigMap.txtTime.visible = false;
        StartMove();
    }
    
    /// <summary>
    /// 停止倒计时 协程
    /// </summary>
    private void StopTimeCoroutine()
    {
        this.StageBigMap.txtTime.visible = false;
        if (timeCoroutine!=null)
        {
            GameManager.Instance.StopCoroutine(timeCoroutine);
        }
    }
    
    protected override void OnHide()
    {
        base.OnHide();
        Utils.ClearSpineModelOnFGUI(this.StageBigMap.bgiMap.hero);
        StopTimeCoroutine();
    }
}
