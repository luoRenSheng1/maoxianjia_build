
using Common;
using Config;
using Engine;
using Spine.Unity;
using UnityEngine;

public class SkillEffectPreView : UIViewBase
{
    private UI_SkillShow skillShow => this.main as UI_SkillShow;

    private int _skillId;
    private SkeletonAnimation animation;
    
    public SkillEffectPreView()
    {
        this.name = "SkillEffectPre";
        this.package = "Common";
        this.component = "SkillShow";
        this.removePackage = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.skillShow.sortingOrder = 100001;
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _skillId = (int) values[0];
    }

    protected override void OnShow()
    {
        base.OnShow();
        ConfigSkillEffectUnit skillUnit = ConfigUtils.GetSkillEffectById(_skillId);
        if (skillUnit.Path != "")
        {
            Utils.ShowUIPrefab(this.skillShow.skillHolder, skillUnit.Path, 100f, "Effect/", o =>
            {
                float len = Utils.GetParticleLength((o as GameObject)?.transform);
                len = Mathf.Min(3f, len);
                //Debug.Log("动画时长:"+len);
                GameManager.Instance.TimerManager.SetTimer(len, Hide);
            });
        }
        else  // 默认近战技能做在模型身上
        {
            ConfigHeroUnit heroUnit = ConfigUtils.GetHeroBySkillId(_skillId);
            Utils.SetSpineModelOnFGUI(this.skillShow.skillHolder, heroUnit.Model, 100f, "skill", (o) =>
            {
                if (o is SkeletonAnimation animator)
                {
                    animation = animator;
                    var skeletonData = animation.skeletonDataAsset.GetSkeletonData(true);
                    var animation1 = skeletonData.FindAnimation("skill");
                    float len = animation1.Duration;
                    GameManager.Instance.TimerManager.SetTimer(len, Hide);
                }
            });
        }
        
        ConfigSkillUnit cfgSkill = ConfigUtils.GetSkillById(_skillId);
        if(cfgSkill.StartSound > 0 && !VillageInfoManager.Instance.IsInVillageHome)
            GameManager.Instance.SoundManager.PlayEffectWithoutLoop(cfgSkill.StartSound);
    }

    protected override void OnHide()
    {
        base.OnHide();
        Utils.HideUIPrefab(this.skillShow.skillHolder);
        GameManager.Instance.TimerManager.ClearTimer(Hide);
    }
}
