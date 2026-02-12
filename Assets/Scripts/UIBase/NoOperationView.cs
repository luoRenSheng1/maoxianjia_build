using System.Collections.Generic;
using Config;
using Engine;
using FairyGUI;
using NoOperation;
using UnityEngine;

public class NoOperationView : UIViewBase
{
    private UI_NoOperation NoOperation => this.main as UI_NoOperation;
    List<PetItemInfo> lstPet  = new List<PetItemInfo>();
    public NoOperationView()
    {
        this.type = UIType.Tip;
        this.name = "NoOperation";
        this.package = "NoOperation";
        this.component = "NoOperation";
        this.removePackage = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        NoOperationBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.NoOperation.sortingOrder = 1000000000;
        this.NoOperation.slider.min = 0;
        this.NoOperation.slider.max = 100;

        this.NoOperation.slider.onChanged.Add(this.OnSliderChange);
        this.NoOperation.slider.onGripTouchEnd.Add(this.OnGripTouchEnd);
        EngineBase.EventDispatcher.GameWorld.Regist<List<ConfigStageUnit>, int>(EventDefine.STAGE_DATA_INFO, OnUpdateStageInfo);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EngineBase.EventDispatcher.GameWorld.UnRegist<List<ConfigStageUnit>, int>(EventDefine.STAGE_DATA_INFO, OnUpdateStageInfo);
    }

    protected override void OnShow()
    {
        base.OnShow();
        List<ConfigStageUnit> stageUnits = ConfigUtils.GetStageUnitById(MapObjectManager.Instance.GuanKaStageId);
        OnUpdateStageInfo(stageUnits, MapObjectManager.Instance.GuanKaMonsterIndex);
        this.NoOperation.slider.value = 0;

        PetInfoManager.Instance.GetUpLoadPet(true);
        lstPet.Clear();
        foreach (var item in PetInfoManager.Instance.GetBattlePetList())
        {
            lstPet.Add(item);
        }

        while (lstPet.Count<5)
        {
            if(PetInfoManager.Instance.GetBattlePetList().Count == 0) break;
            foreach (var item in PetInfoManager.Instance.GetBattlePetList())
            {
                if(lstPet.Count < 5)
                    lstPet.Add(item);
            }
        }

        for (int i = 0; i < lstPet.Count; i++)
        {
            string strResName = ConfigUtils.GePetModelPathByID(lstPet[i].petCfg.Id);
            GObject gObject = this.NoOperation.petRoot.GetChild("p" + i);
            if(gObject != null)
                Utils.SetSpineModelOnFGUI(gObject.asGraph,strResName, 50f, "run");
            else
            {
                Debug.LogErrorFormat("奇怪了为啥会为空了。。。。。。。。。。。。。。。。{0}", i);
            }
        }
        this.NoOperation.petRoot.t0.Stop();
        this.NoOperation.petRoot.t0.ClearHooks();
        this.NoOperation.petRoot.t1.Stop();
        this.NoOperation.petRoot.t1.ClearHooks();
        this.NoOperation.petRoot.t2.Stop();
        this.NoOperation.petRoot.t2.ClearHooks();
        this.NoOperation.petRoot.t3.Stop();
        this.NoOperation.petRoot.t3.ClearHooks();
        this.NoOperation.petRoot.t4.Stop();
        this.NoOperation.petRoot.t4.ClearHooks();
        
        this.NoOperation.petRoot.t0.timeScale = this.NoOperation.petRoot.t1.timeScale =
            this.NoOperation.petRoot.t2.timeScale =
                this.NoOperation.petRoot.t3.timeScale = this.NoOperation.petRoot.t4.timeScale = 0.3f;
        this.NoOperation.petRoot.t0.Play(1, 0, null);
        this.NoOperation.petRoot.t0.SetHook("start", () =>
        {
            this.NoOperation.petRoot.t1.Play(1, 0, null);
            this.NoOperation.petRoot.t1.SetHook("start", () =>
            {
                this.NoOperation.petRoot.t2.Play(1, 0, null);
                this.NoOperation.petRoot.t2.SetHook("start", () =>
                {
                    this.NoOperation.petRoot.t3.Play(1, 0, null);
                    this.NoOperation.petRoot.t3.SetHook("start", () =>
                    {
                        this.NoOperation.petRoot.t4.Play(1, 0, null);
                        this.NoOperation.petRoot.t4.SetHook("start", () =>
                        {
                            this.NoOperation.petRoot.t0.Play(1, 0, null);
                        });
                    });
                });
            });
        });
    }

    protected override void OnHide()
    {
        base.OnHide();
        for (int i = 0; i < lstPet.Count; i++)
        {
            GObject gObject = this.NoOperation.petRoot.GetChild("p" + i);
            if(gObject != null)
                Utils.ClearSpineModelOnFGUI(gObject.asGraph);
        }
        lstPet.Clear();
        this.NoOperation.petRoot.t0.Stop();
        this.NoOperation.petRoot.t0.ClearHooks();
        this.NoOperation.petRoot.t1.Stop();
        this.NoOperation.petRoot.t1.ClearHooks();
        this.NoOperation.petRoot.t2.Stop();
        this.NoOperation.petRoot.t2.ClearHooks();
        this.NoOperation.petRoot.t3.Stop();
        this.NoOperation.petRoot.t3.ClearHooks();
        this.NoOperation.petRoot.t4.Stop();
        this.NoOperation.petRoot.t4.ClearHooks();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        this.NoOperation.dateLb.text = System.DateTime.Now.ToString("H:mm");
    }

    private void OnUpdateStageInfo(List<ConfigStageUnit> stageUnits, int guankaIndex)
    {
        // this.NoOperation.stageName.text = stageUnits[guankaIndex].Name;
        // this.NoOperation.stageName.text = ConfigUtils.GetTextById(stageUnits[guankaIndex].Name,stageUnits[guankaIndex].NameParam);
        this.NoOperation.stageName.text = string.Format(ConfigUtils.GetTextById(stageUnits[guankaIndex].Name), stageUnits[guankaIndex].Chapter, stageUnits[guankaIndex].LevelId % 100);
    }

    private void OnSliderChange()
    {
        if (this.NoOperation.slider.value >= 75)
        {
            this.NoOperation.slider.groupArrow.visible = false;
            Object.FindObjectOfType<NoOperationMono>().Reset();
            Object.FindObjectOfType<NoOperationMono>().isOpenCheck = true;
            Application.targetFrameRate = SystemAdapter.S_FRAMERATE_NORMAL;
            UIManager.Instance.DestroyUIPanel("NoOperation");
        }

    }

    private void OnGripTouchEnd()
    {
        if (this.NoOperation.slider.value < 95)
        {
            this.NoOperation.slider.value = 0;
        }

        this.NoOperation.slider.groupArrow.visible = true;
    }
    
}
