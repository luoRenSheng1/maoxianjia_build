using BigMap;
using Config;
using Engine;
using FairyGUI;
using msg;
using UnityEngine;

public class ChapterEventStageDetailView : UIViewBase
{
    private UI_ChapterEventStageDetail eventDetail => this.main as UI_ChapterEventStageDetail;

    // private ConfigStageUnit stageUnit;
    private MapStageData stageData;
    public ChapterEventStageDetailView()
    {
        this.package = "BigMap";
        this.name = "ChapterEventStageDetail";
        this.component = "ChapterEventStageDetail";
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
        this.eventDetail.btnFight.onClick.Add(this.OnClickFightBtn);
        this.eventDetail.Btn1.onClick.Add(this.OnClickFightBtn);
        this.eventDetail.Btn2.onClick.Add(this.OnClickCloseButton);
        this.eventDetail.Btn3.onClick.Add(this.OnShowPreview);
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        stageData = values[0] as MapStageData;
    }
    //protected override void OnHide()
    //{
    //    base.OnHide();
        //如果引导过程中，事件刷新，取消事件
    //}
    protected override void OnShow()
    {
        base.OnShow();
        RandomEventData taskData = stageData.stageTaskData;

        ConfigEventUnit eventData = ConfigUtils.GetEventDataById(taskData.eventId);
        if (eventData != null)
        {
            this.eventDetail.name.text = ConfigUtils.GetTextById(eventData.Name);
            this.eventDetail.desc.text = ConfigUtils.GetTextById(eventData.Desc.ToString());

            this.eventDetail.twoBtns.visible = false;
            this.eventDetail.oneBtns.visible = true;
            this.eventDetail.Btn3.visible = false;
            this.eventDetail.type.selectedIndex = 0;

            eRandomEventType e = (eRandomEventType)eventData.Type;
            switch (e)
            {
                case eRandomEventType.eRandomEventType_RandomBox://深埋宝藏
                    {
                        this.eventDetail.icon.url = UIResource.GetMapEventPopBgByEventId(eventData.Resource);
                        // this.eventDetail.txtBtn.text = ConfigUtils.GetStringByKey(8027);
                        this.eventDetail.twoBtns.visible = true;
                        this.eventDetail.oneBtns.visible = false;
                    
                        //策划说写死
                        this.eventDetail.Btn1.text = ConfigUtils.GetStringByKey(8027);
                        this.eventDetail.Btn2.text = "我再逛逛";

                        //深埋宝藏引导-点击挖宝
                        GuideManager.Instance.StarGuideByData(new GuideData()
                        {
                            fid = FuncOpenType.Sokoban,
                            giding = GuideID.guideId_3700,
                            gid = GuideID.guideId_3701,
                            tui = this.eventDetail.Btn1,
                            isForce = true,
                            isSend = true,
                            //isLucency = true,//透明强制引导的黑色遮罩，但只能点击手指指向区域
                            //scale = new Vector2(1f, 1.5f),
                            //npcTxt = "Beginner_Doc_019",
                            //npcPosType = PosType.Down,
                            //gType = global::GuideType.Wait,
                            touchCB = () =>
                            {
                                GuideManager.Instance.HideHandle();
                            }
                        });
                    }
                    break;
                case eRandomEventType.eRandomEventType_RuinsBuff://遗迹建筑
                    {
                        this.eventDetail.icon.url = UIResource.GetMapEventPopBgByEventId(eventData.Resource);
                        this.eventDetail.txtBtn.text = ConfigUtils.GetStringByKey(8028);
                        //遗迹建筑引导-点击开启遗迹
                        GuideManager.Instance.StarGuideByData(new GuideData()
                        {
                            fid = FuncOpenType.Relic,
                            giding = GuideID.guideId_3800,
                            gid = GuideID.guideId_3801,
                            tui = this.eventDetail.btnFight,
                            isForce = true,
                            isSend = true,
                            //isLucency = true,//透明强制引导的黑色遮罩，但只能点击手指指向区域
                            //scale = new Vector2(1f, 1.5f),
                            //npcTxt = "Beginner_Doc_018",
                            //npcPosType = PosType.Down,
                            //gType = global::GuideType.Wait,
                            //cb = () =>
                            //{
                            //}
                        });
                    }
                    break;
                case eRandomEventType.eRandomEventType_StageDropPet://搜寻宠物
                    {
                        this.eventDetail.type.selectedIndex = 1;
                        this.eventDetail.petIcon.url = UIResource.GetMapEventPopBgByEventId(eventData.Resource);
                        this.eventDetail.txtBtn.text = ConfigUtils.GetStringByKey(5169);
                        //搜寻宠物引导-点击【抓捕】
                        GuideManager.Instance.StarGuideByData(new GuideData()
                        {
                            fid = FuncOpenType.SearchPets,
                            giding = GuideID.guideId_4100,
                            gid = GuideID.guideId_4101,
                            tui = this.eventDetail.btnFight,
                            isForce = true,
                            isSend = true,
                            //isLucency = true,//透明强制引导的黑色遮罩，但只能点击手指指向区域
                            //scale = new Vector2(1f, 1.5f),
                            //npcTxt = "Beginner_Doc_018",
                            //npcPosType = PosType.Down,
                            //gType = global::GuideType.Wait,
                            //cb = () =>
                            //{
                            //}
                        });
                    }
                    break;
                case eRandomEventType.eRandomEventType_AdventureBusinessMan://奇遇商人
                    {
                        //策划说写死
                        this.eventDetail.txtBtn.text = "传送";
                        this.eventDetail.Btn3.visible = true;
                        //搜寻宠物引导-点击【传送】
                        GuideManager.Instance.StarGuideByData(new GuideData()
                        {
                            fid = FuncOpenType.AdventureBusiness,
                            giding = GuideID.guideId_3900,
                            gid = GuideID.guideId_3901,
                            tui = this.eventDetail.btnFight,
                            isForce = true,
                            isSend = true,
                            //isLucency = true,//透明强制引导的黑色遮罩，但只能点击手指指向区域
                            //scale = new Vector2(1f, 1.5f),
                            //npcTxt = "Beginner_Doc_018",
                            //npcPosType = PosType.Down,
                            //gType = global::GuideType.Wait,
                            //cb = () =>
                            //{
                            //}
                        });
                    }
                    break;
            }
        }

        Debug.Log("===== ChapterEventStageDetailView =====");

    }

    private void OnClickFightBtn(EventContext context)
    {
        RandomEventData taskData = stageData.stageTaskData;
        ConfigEventUnit eventData = ConfigUtils.GetEventDataById(taskData.eventId);
        var e = (eRandomEventType)eventData.Type;
        switch (e)
        {
            case eRandomEventType.eRandomEventType_RandomBox://深埋宝藏
                {
                    var msg = EnterBoxMap_CS.CreateBuilder();
                    msg.EventGuid = taskData.guid;//事件guid
                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_EnterBoxMap_CS, msg.Build());
                    //进入小地图副本
                    //var view = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
                    //view?.ShowSokobanMapView(0123456789);
                }
                break;
            case eRandomEventType.eRandomEventType_RuinsBuff: //遗迹建筑
                {
                    if (MapChapterManager.Instance.GetFinishEventCount((int)e) >= eventData.Number)
                    {
                        UIManager.Instance.Toast(ConfigUtils.GetStringByKey(8003));
                        return;
                    }

                    var view = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
                    if (view != null)
                    {
                        view?.EnterRuinMapPlayEffect();
                    }
                    
                    var builder = EnterRuin_CS.CreateBuilder();
                    builder.EventGuid = taskData.guid;
                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_EnterRuin_CS, builder.Build());
                }
                break;
            case eRandomEventType.eRandomEventType_StageDropPet://搜寻宠物
                {
                    var builder = AcceptWildPet_CS.CreateBuilder();
                    builder.EventGuid = (ulong)taskData.guid;
                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_AcceptWildPet_CS, builder.Build());
                }
                break;
            case eRandomEventType.eRandomEventType_AdventureBusinessMan://奇遇商人
                {
                    var msg = EnterCave_CS.CreateBuilder();
                    msg.EventGuid = taskData.guid;//事件guid
                    GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_EnterCave_CS, msg.Build());
                }
                break;
        }

        UIManager.Instance.CloseUIPanel("ChapterEventStageDetail");
    }

    private void OnClickCloseButton()
    {
        UIManager.Instance.CloseUIPanel("ChapterEventStageDetail");
    }

    /// <summary>
    /// 预览
    /// </summary>
    private void OnShowPreview()
    {
        UIManager.Instance.ShowUIPanel("AdventureCaveShop", false, stageData.stageTaskData.guid);
    }
}
