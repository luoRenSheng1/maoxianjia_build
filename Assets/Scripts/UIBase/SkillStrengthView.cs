
using System.Collections.Generic;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using Pet;

public class SkillStrengthView : UIViewBase
{
    private UI_PetStrength PetStrength => this.main as UI_PetStrength;

    private List<SkillStrengthVo> _skillStrengthVos;
    float _delay = 0f;
    public SkillStrengthView()
    {
        this.name = "SkillStrength";
        this.package = "Pet";
        this.component = "PetStrength";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        PetBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _skillStrengthVos = values[0] as List<SkillStrengthVo>;
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.PetStrength.closeBtn.onClick.Add(this.Hide);
        this.PetStrength.petList.petList.itemRenderer = PetStrengthItemRender;
    }

    protected override void OnShow()
    {
        base.OnShow();
        
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.UpLvSE);
        
        // this.PetStrength.t0.Play(1, 0, null);
        // Utils.PlaySpineAnim(this.PetStrength.gxhdSpine, "Chuxian", false, () =>
        // {
        //     //Utils.PlaySpineAnim(this.GetRewardUI.gxhdSpine, "Loop", true);
        // });
        _skillStrengthVos.Sort((a, b)=>
        {
            int result = a.SkillUnit.SkillQuality > b.SkillUnit.SkillQuality ? 1 : (a.SkillUnit.SkillQuality==b.SkillUnit.SkillQuality ? 0 : -1);
            if (result == 0)
                result = a.SkillUnit.Id > b.SkillUnit.Id ? 1 : -1;
            return result;
        });
        this.PetStrength.petList.petList.numItems = _skillStrengthVos.Count;
        this.PetStrength.petList.petList.ScrollToView(0);

        GuideManager.Instance.StarGuideByData(new GuideData()
        {
            giding = GuideID.Click_SkillIntensify,
            gid = GuideID.Click_UIEquipIntensifyClose,
            tui = this.PetStrength.closeBtn,
            pType = PosType.Down,
            isForce = true,
            isSend = true,
            isLucency = false,
            isOver = true
        });
    }
    
    private void PetStrengthItemRender(int index, GObject item)
    {
        SkillStrengthVo skillStrengthVo = _skillStrengthVos[index];
        ((UI_ItemCom)((UI_PetStrengthItem) item).item).icon = UIResource.GetItemUrl(skillStrengthVo.SkillUnit.SkillIcon);
        ((UI_ItemCom)((UI_PetStrengthItem) item).item).ctrlQuality.selectedIndex = skillStrengthVo.SkillUnit.SkillQuality - 1;
        ((UI_PetStrengthItem) item).curLb.text = skillStrengthVo.PreLv.ToString();
        ((UI_PetStrengthItem) item).nextLb.text = skillStrengthVo.CurLv.ToString();
        ((UI_PetStrengthItem) item).visible = false;
        ((UI_ItemCom)((UI_PetStrengthItem) item).item).itemSpineEff.visible = false;
        _delay = index * 0.02f;
        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 1, _delay, () =>
        {
            ((UI_PetStrengthItem) item).visible = true;
            //((UI_ItemCom)((UI_PetStrengthItem) item).item).itemSpineEff.visible = true;
        });
    }

    protected override void OnHide()
    {
        base.OnHide();
        GameManager.Instance.TimerManager.ClearTimerBySourceID(EN_TIMER_SOURCE.UI, 1);
        EngineBase.EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_SKILL_LEVELUP_SUCCESS);
    }
}
