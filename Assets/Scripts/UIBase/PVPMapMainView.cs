
using System;
using System.Collections.Generic;
using BestHTTP.Extensions;
using Common;
using CommonEx;
using Config;
using FairyGUI;
using Engine;
using Lobby;
using PVPMap;
using EventDispatcher = EngineBase.EventDispatcher;
using Random = UnityEngine.Random;
using UI_battleRoot = PVPMap.UI_battleRoot;

public class PVPMapMainView : UIViewBase
{
    private UI_PVPMapMain PVPMain => this.main as UI_PVPMapMain;
    
    private Dictionary<int, int> _skillIdList1 = new Dictionary<int, int>(6);
    private Dictionary<int, int> _skillIdList2 = new Dictionary<int, int>(6);
    
    private int mCurGuankaIndex = 0;
    private float orgBattleRootY = 154;//这边写死，获取不到fairygui的设计位置
    private bool _isInit;

    private double _myFight;
    private double _otherFight;
    private int _myHeadIcon;
    private string _enemyHeadIcon;
    public PVPMapMainView()
    {
        this.type = UIType.Top;
        this.name = "PVPMapMain";
        this.package = "PVPMap";
        this.component = "PVPMapMain";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        PVPMapBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _myFight = (double) values[0];
        _otherFight = (double) values[1];
        _myHeadIcon = (int)values[2];
        _enemyHeadIcon = (string) values[3];
    }

    protected override void OnInit()
    {
        base.OnInit();

        ((UI_ComSkills) this.PVPMain.panel.battleRoot.comSkills).listSkills.itemRenderer = ItemListSkillRender;

        EventDispatcher.GameWorld.Regist<int, EN_CAMP_TYPE>(EventDefine.FIGHT_DATA_HERO_CASTSKILL, OnCastSkill);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_HERO_HP_CHANGE, UpdateRoleInfo);
    }

    protected override void OnShow()
    {
        base.OnShow();
        GameManager.Instance.SoundManager.PlayMusic((int)SoundType.PVPBGM);
        PVPMapManager.Instance.IsInPVP = true;
        MapObjectManager.Instance.IsStopMapBattle = true;
        PVPMapManager.Instance.InitGuanKaFSM();
        UpdateSkillList();
        UpdateUIInfo();
        OnClickAutoSkill();
        this.PVPMain.panel.battleRoot.myFightLb.text = StringUtils.FormatCurrency(_myFight);
        this.PVPMain.panel.battleRoot.otherFightLb.text = StringUtils.FormatCurrency(_otherFight);
        this.PVPMain.vsCom.timeLb.text = PVPMapManager.Instance.PVPTotalTime.ToString("d2");

        if (UIManager.Instance.FindByName("StageBigMap") is StageBigMapView bigMapView && bigMapView.IsShow())
        {
            UIManager.Instance.CloseUIPanel("StageBigMap");
        }

        // this.PVPMain.myHeadIcon.icon = UIResource.GetItemUrl(DataManager.Instance.GetRoleData().GetAvatarUrl());
        this.PVPMain.myHeadIcon.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(_myHeadIcon).Icon);
        this.PVPMain.enemyHeadIcon.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(_enemyHeadIcon.ToInt32()).Icon);
    }

    protected override void OnHide()
    {
        base.OnHide();
        
        for (int i = 0; i < ((UI_ComSkills) this.PVPMain.panel.battleRoot.comSkills).listSkills.numChildren; i++)
        {
            UI_BtnSkill btnSkill = (UI_BtnSkill) ((UI_ComSkills) this.PVPMain.panel.battleRoot.comSkills).listSkills.GetChildAt(i);
            if (null != btnSkill)
            {
                GTween.Kill(btnSkill);
                btnSkill.mask.fillAmount = 0;
            }
        }
        
        GTween.Kill(this);
        
        this.PVPMain.vsCom.timeLb.text = PVPMapManager.Instance.PVPTotalTime.ToString("d2");
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist<int, EN_CAMP_TYPE>(EventDefine.FIGHT_DATA_HERO_CASTSKILL, OnCastSkill);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_HERO_HP_CHANGE, UpdateRoleInfo);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        int totalTime = (int) PVPMapManager.Instance.timerPvp.GetRemain();
        if (totalTime >= 1)
            this.PVPMain.vsCom.timeLb.text = totalTime.ToString("d2");
    }

    private void OnClickAutoSkill()
    {
        var hero = MapObjectManager.Instance.GetLocalHero();

        if (hero != null)
        {
            hero.IsAutoXPAttack = true;

            foreach (var item in RoleManager.Instance.GetSkillProxyDict())
            {
                item.Value.IsAutoXPAttack = true;
            }
            
            foreach (var item in RoleManager.Instance.GetEnemySkillProxyDict())
            {
                item.Value.IsAutoXPAttack = true;
            }
        }
        
    }


    private void UpdateUIInfo()
    {
        UpdateRoleInfo();
        ((UI_ComSkills) this.PVPMain.panel.battleRoot.comSkills).listSkills.touchable = false;
        for (int i = 0; i < ((UI_ComSkills) this.PVPMain.panel.battleRoot.comSkills).listSkills.numChildren; i++)
        {
            UI_BtnSkill btnSkill = (UI_BtnSkill) ((UI_ComSkills) this.PVPMain.panel.battleRoot.comSkills).listSkills.GetChildAt(i);
            if (null != btnSkill)
            {
                btnSkill.mask.fillAmount = 0;
            }
        }
    }

    private void UpdateRoleInfo()
    {
        if (IsShow() && IsOnStage())
        {
            MapHeroObject myHero = MapObjectManager.Instance.GetHeroObjectById(1);
            if (myHero != null)
            {
                this.PVPMain.myHpBar.max = myHero.HeroAttr.HPMax;
                this.PVPMain.myHpBar.min = 0;
                this.PVPMain.myHpBar.value = myHero.HeroAttr.HP;
            }
            
            MapHeroObject enemyHero = MapObjectManager.Instance.GetHeroObjectById(2);
            if (enemyHero != null)
            {
                this.PVPMain.enemyHpBar.max = enemyHero.HeroAttr.HPMax;
                this.PVPMain.enemyHpBar.min = 0;
                this.PVPMain.enemyHpBar.value = enemyHero.HeroAttr.HP;
            }

        }

    }

    private void UpdateSkillList()
    {
        _skillIdList1.Clear();
        foreach (var skill in RoleManager.Instance.GetSkillProxyDict())
        {
            if (!_skillIdList1.ContainsKey(skill.Key))
            {
                _skillIdList1.Add(skill.Key, skill.Value.ProxyAttr.skillXPID);
            }
            skill.Value.IsAutoXPAttack = true;
        }

        ((UI_ComSkills) this.PVPMain.panel.battleRoot.comSkills).listSkills.numItems = 6;

        _skillIdList2.Clear();
        foreach (var skill in RoleManager.Instance.GetEnemySkillProxyDict())
        {
            if (!_skillIdList2.ContainsKey(skill.Key))
            {
                _skillIdList2.Add(skill.Key, skill.Value.ProxyAttr.skillXPID);
            }
            skill.Value.IsAutoXPAttack = true;
        }

    }
    
    private void ItemListSkillRender(int index, GObject item)
    {
        if (_skillIdList1.TryGetValue(index, out int skillId))
        {
            item.data = skillId;
            if (skillId > 0)
            {
                ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(skillId);
                ((UI_BtnSkill) item).skillIcon.skillIcon.url = UIResource.GetItemUrl(skillUnit.SkillIcon);
            }
        }
        if(SkillInfoManager.Instance.UnLockSkillPos >= index)
            ((UI_BtnSkill) item).hasSkill.selectedIndex = skillId > 0 ? 0 : 1;
        else
            ((UI_BtnSkill) item).hasSkill.selectedIndex = 2;
        if (((UI_BtnSkill) item).hasSkill.selectedIndex == 1)
            ((UI_BtnSkill) item).redDot.visible = SkillInfoManager.Instance.IsHasNoUpLoadSkill();

    }

    private void OnCastSkill(int skillID, EN_CAMP_TYPE campType)
    {
        if (IsShow() && IsOnStage())
        {
            if (campType == EN_CAMP_TYPE.HERO || campType == EN_CAMP_TYPE.FRIEND)
            {
                var skillType = ConfigUtils.GetSkillById(skillID);
                int skillIndex = -1;
                foreach (var item in _skillIdList1)
                {
                    if (item.Value == skillID)
                    {
                        skillIndex = item.Key;
                        break;
                    }
                }
            
                if(skillType != null && (skillType.SkillType == (int) EN_SKILL_TYPE.ChoukaSkill || skillType.SkillType == (int) EN_SKILL_TYPE.XP))
                    SceneEffect();

                if (skillType != null && skillIndex != -1)
                {
                    float fCDTime = skillType.Cd * ConstDefine.CONFIG_PLACE_EX/MapObjectManager.Instance.Speed;
                    var skillBtn = ((UI_ComSkills) this.PVPMain.panel.battleRoot.comSkills).listSkills.GetChildAt(skillIndex) as UI_BtnSkill;

                    if (skillBtn != null)
                    {
                        CastSkillCD(skillBtn, fCDTime);
                    }
                }
            }

        }
        

    }
    
    private void CastSkillCD(UI_BtnSkill item, float time)
    {
        GTween.Kill(item);
        GTween.To(1, 0, time)
            .SetTarget(item)
            .SetEase(EaseType.Linear)
            .OnStart(() =>
            {
                item.mask.fillAmount = 0;
                item.touchable = false;
            })
            .OnUpdate((fillnum) => { item.mask.fillAmount = fillnum.value.x; })
            .OnComplete(() =>
            {
                item.mask.fillAmount = 0;
                item.touchable = true;
            });
    }
    
    
    public GComponent GetBattleRoot()
    {
        return this.PVPMain.panel.battleRoot;
    }
    
    public GComponent GetRoot()
    {
        return this.PVPMain;
    }

    public GComponent GetBattleBGRoot()
    {
        return ((UI_battleRoot) this.PVPMain.panel.battleRoot).bg;
    }

    public GComponent GetBattleSceneRoot()
    {
        return ((UI_battleRoot) this.PVPMain.panel.battleRoot).scene;
    }

    public GComponent GetBattleSceneObjRoot()
    {
        return ((UI_battleRoot) this.PVPMain.panel.battleRoot).scene.obj;
    }

    public GComponent GetBattleSceneFlyRoot()
    {
        return ((UI_battleRoot) this.PVPMain.panel.battleRoot).scene.fly;
    }
    
    
    private void SceneEffect()
    {
        int time = Random.Range(1, 4);
        ((UI_battleRoot)this.PVPMain.panel.battleRoot).scene.sceneT.Play(time, 1f, null);
        ((UI_battleRoot)this.PVPMain.panel.battleRoot).bg.sceneT.Play(time, 1f, null);
    }
}
