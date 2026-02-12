
using System;
using System.Collections.Generic;
using System.Reflection;
using Common;
using CommonEx;
using Config;
using FairyGUI;
using DungeonMap;
using Engine;
using Lobby;
using msg;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;
using Random = UnityEngine.Random;

public class DungeonMapView : UIViewBase
{
    private UI_DungeonMain dungeonMain => this.main as UI_DungeonMain;
    
    private Dictionary<int, int> _skillIdList = new Dictionary<int, int>(6);
    private ConfigDungeonStageUnit _dungeonStageUnit = null;
    private ConfigEventStageUnit _eventStageUnit = null;
    private ConfigStageMonsterAttrUnit _stageMonsterAttrUnit = null;
    private DungeonType _dungeonType = DungeonType.NONE;
    private int mStageMonsterWave = 0;
    private int mCurGuankaIndex = 0;
    private float orgBattleRootY = 154;//这边写死，获取不到fairygui的设计位置
    private bool _isInit;
    private float _activeSkillBaseCD;
    
    public DungeonMapView()
    {
        this.name = "DungeonMap";
        this.package = "DungeonMap";
        this.component = "DungeonMain";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        DungeonMapBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _dungeonStageUnit = null;
        _eventStageUnit = null;
        _stageMonsterAttrUnit = null;
        
        if (values[0] is ConfigDungeonStageUnit)
        {
            _dungeonStageUnit = values[0] as ConfigDungeonStageUnit;
            _dungeonType = (DungeonType)_dungeonStageUnit.Type;
        }else if (values[0] is ConfigEventStageUnit)
        {
            _eventStageUnit = values[0] as ConfigEventStageUnit;
            _dungeonType = DungeonType.BossWithPet; //(DungeonType)_eventStageUnit.Type;
        }else if (values[0] is FishingBossType)
        {
            if (values[1] is ConfigStageMonsterAttrUnit)
            {
                _stageMonsterAttrUnit = values[1] as ConfigStageMonsterAttrUnit;
            }

            if (values[1] is ConfigEventStageUnit)
            {
                _eventStageUnit = values[1] as ConfigEventStageUnit;
            }

            _dungeonType = DungeonType.FishingBoss;
        }
    }

    protected override void OnInit()
    {
        base.OnInit();
        
        ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.btnSkillAuto.onClick.Set(OnClickAutoSkill);
        ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.listSkills.onClickItem.Add(OnClickSkillItem);
        ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.listSkills.itemRenderer = ItemListSkillRender;
        ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.activeSkill.onClick.Add(OnClickHeroSkill);
        ((UI_battleRoot)this.dungeonMain.panel.battleRoot).bossTxList.itemRenderer = BossTxRender;
        
        this.dungeonMain.panel.runBtn.onClick.Add(this.OnClickOutMapBtn);
    
        ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).monsterGroupList.itemRenderer = ItemRendererGroupList;
        
        EventDispatcher.GameWorld.Regist<int, EN_CAMP_TYPE>(EventDefine.FIGHT_DATA_HERO_CASTSKILL, OnCastSkill);
        EventDispatcher.GameWorld.Regist<ConfigDungeonStageUnit, int>(EventDefine.EVENT_DUNGEON_STAGE_DATA_INFO, OnUpdateStageInfo);
        EventDispatcher.GameWorld.Regist<ConfigEventStageUnit, int>(EventDefine.EVENT_DUNGEON_RANDOM_STAGE_DATA_INFO, OnUpdateStageInfo);
        EventDispatcher.GameWorld.Regist<ConfigStageMonsterAttrUnit, int>(EventDefine.EVENT_DUNGEON_FISHING_STAGE_DATA_INFO, OnUpdateStageInfo);
        EventDispatcher.GameWorld.Regist<int>(EventDefine.EVENT_DUNGEON_MONSTER_WAVE_DATA_INFO, OnUpdateMonsterWaveInfo);
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_Refresh_xpskill, UpdateXPSkill);
        EventDispatcher.GameWorld.Regist<float>(EventDefine.EVENT_UPDATE_BATTLE_SKILL_CD, this.ResetAllSkillCD);
        
        ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comMessage.visible = false;
        
    }

    protected override void OnShow()
    {
        base.OnShow();
        if (!_isInit)
        {
            _isInit = true;
            ((UI_battleRoot) this.dungeonMain.panel.battleRoot).topGroup.y = Math.Min(((UI_battleRoot) this.dungeonMain.panel.battleRoot).topGroup.y, orgBattleRootY - ((UI_battleRoot) this.dungeonMain.panel.battleRoot).y + ((UI_battleRoot) this.dungeonMain.panel.battleRoot).topGroup.y);
        }
        
        GameManager.Instance.SoundManager.PlayMusic((int)SoundType.CopyBGM);
        
        DungeonMapManager.Instance.IsInCopy = true;
        MapObjectManager.Instance.IsStopMapBattle = true;
        DungeonMapManager.Instance.InitGuanKaFSM(_dungeonType);
        UpdateSkillList();
        UpdateUIInfo();
        // OnClickAutoSkill();
        ShowBtnSkillAutoState();

        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_GOTO_DUNGEON_MAP_STATE, true);
        this.dungeonMain.panel.runBtn.touchable = false;
        GameManager.Instance.TimerManager.SetTimer(2.0f, () =>
        {
            this.dungeonMain.panel.runBtn.touchable = true;
        });
        // UpdateXPSkill();
        ((UI_battleRoot) this.dungeonMain.panel.battleRoot).touchPanel.visible = false;

        if (_dungeonType == DungeonType.BossWithPet)
        {
            this.dungeonMain.panel.txtDesc.visible = false;
            this.dungeonMain.panel.txtBattleContent.text = ConfigUtils.GetStringByKey(8032);
        }else if (_dungeonType == DungeonType.FishingBoss)
        {
            this.dungeonMain.panel.txtDesc.visible = false;
            this.dungeonMain.panel.txtBattleContent.text = ConfigUtils.GetStringByKey(8032);
        }
        else
        {
            this.dungeonMain.panel.txtDesc.visible = true;
            this.dungeonMain.panel.txtBattleContent.text = ConfigUtils.GetStringByKey(8031);
        }

        this.dungeonMain.panel.copyType.selectedIndex = 0;
        ((UI_battleRoot)this.dungeonMain.panel.battleRoot).copyCtrl.selectedIndex = 1;
        ((UI_battleRoot)this.dungeonMain.panel.battleRoot).copyBlood.icon = UIResource.GetBloodIconByName("boss");
        if (_dungeonType == DungeonType.BossWithPet)
        {
            this.dungeonMain.panel.copyType.selectedIndex = 1;
            if (DungeonMapManager.Instance.mapEventData.eventType == (int)StageEventType.Pet)
            {
                ((UI_battleRoot)this.dungeonMain.panel.battleRoot).copyBlood.icon = UIResource.GetBloodIconByName("pet");
            }
        }
    }

    protected override void OnHide()
    {
        base.OnHide();
        
        for (int i = 0; i < ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.listSkills.numChildren; i++)
        {
            UI_BtnSkill btnSkill = (UI_BtnSkill) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.listSkills.GetChildAt(i);
            if (null != btnSkill)
            {
                GTween.Kill(btnSkill);
                btnSkill.mask.fillAmount = 0;
            }
        }
        GTween.Kill(this);
        
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_GOTO_DUNGEON_MAP_STATE, false);

        PlaySceneBGM();
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist<int, EN_CAMP_TYPE>(EventDefine.FIGHT_DATA_HERO_CASTSKILL, OnCastSkill);
        EventDispatcher.GameWorld.UnRegist<ConfigDungeonStageUnit, int>(EventDefine.EVENT_DUNGEON_STAGE_DATA_INFO, OnUpdateStageInfo);
        EventDispatcher.GameWorld.UnRegist<ConfigEventStageUnit, int>(EventDefine.EVENT_DUNGEON_RANDOM_STAGE_DATA_INFO, OnUpdateStageInfo);
        EventDispatcher.GameWorld.Regist<ConfigStageMonsterAttrUnit, int>(EventDefine.EVENT_DUNGEON_FISHING_STAGE_DATA_INFO, OnUpdateStageInfo);
        EventDispatcher.GameWorld.UnRegist<int>(EventDefine.EVENT_DUNGEON_MONSTER_WAVE_DATA_INFO, OnUpdateMonsterWaveInfo);
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_Refresh_xpskill, UpdateXPSkill);
        EventDispatcher.GameWorld.UnRegist<float>(EventDefine.EVENT_UPDATE_BATTLE_SKILL_CD, this.ResetAllSkillCD);

        PlaySceneBGM();
    }

    private void ItemRendererGroupList(int index, GObject item)
    {
        ((UI_MonsterGroupItem) item).type.selectedIndex = (index == 0) ? 0 : 1;
        if (index <= mCurGuankaIndex)
        {
            ((UI_MonsterGroupItem) item).full.selectedIndex = 1;
            if (index == mCurGuankaIndex)
            {
                ((UI_MonsterGroupItem) item).fullBar.TweenValue(100, 2f);
            }
            else
            {
                ((UI_MonsterGroupItem) item).fullBar.value = 100;
            }
        }
        else
        {
            ((UI_MonsterGroupItem) item).fullBar.value = 0;
            ((UI_MonsterGroupItem) item).full.selectedIndex = 0;
        }
    }

    private void OnClickOutMapBtn()
    {
        MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
        {
            OkCallBack = OutMapSuccess
        };
        UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.GetStringByKey(10077), param);
    }

    private void OutMapSuccess()
    {
        DungeonMapManager.Instance.IsInCopy = false;
        MapObjectManager.Instance.IsStopMapBattle = false;
        UIManager.Instance.CloseUIPanel("DungeonMap");
        if (_dungeonType == DungeonType.BossWithPet)
        {
            // DungeonMapManager.Instance.mapEventData = null;
            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);

            if (DungeonMapManager.Instance.mapEventData.eventType == (int)StageEventType.Relic || DungeonMapManager.Instance.mapEventData.eventType == (int)eRandomEventType.eRandomEventType_RandomBox)
            {
                //遗迹boss
                DungeonMapManager.Instance.mapEventData = null;
                DungeonMapManager.Instance.eventMonsterData = null;
            }
            else
            {
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_RESULT_UPDATE, false, DungeonMapManager.Instance.batchStuffId, DungeonMapManager.Instance.mapEventData.guid);
                DungeonMapManager.Instance.mapEventData = null;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
            }
        }

        if (_dungeonType == DungeonType.FishingBoss)
        {
            DungeonMapManager.Instance.mapEventData = null;
            var view = UIManager.Instance.FindByName("ChapterMap") as  ChapterMapView;
            if (view != null)
            {
                view?.CheckHeroNearFishingPos(true);
                view.curFishingPosData = null;  // 防止状态残留
            }
        }
        
    }
    
    private void ShowBtnSkillAutoState()
    {
        // var hero = MapObjectManager.Instance.GetLocalHero();
        //
        // if (hero != null)
        // {
            ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.btnSkillAuto.ctrlAuto.selectedIndex = IsAutoXPAttack ? 1 : 0;
           
            foreach (var item in RoleManager.Instance.GetSkillProxyDict())
            {
                item.Value.IsAutoXPAttack = IsAutoXPAttack;
            }
            if (RoleManager.Instance.GetMapHeroSkillProxy() != null)
                RoleManager.Instance.GetMapHeroSkillProxy().IsAutoXPAttack = IsAutoXPAttack;
        // }
        
    }
    
    private bool IsAutoXPAttack = true;
    private void OnClickAutoSkill()
    {
        var hero = MapObjectManager.Instance.GetLocalHero();

        if (hero != null)
        {
            IsAutoXPAttack = !IsAutoXPAttack;
            ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.btnSkillAuto.ctrlAuto.selectedIndex = IsAutoXPAttack ? 1 : 0;
           
            foreach (var item in RoleManager.Instance.GetSkillProxyDict())
            {
                item.Value.IsAutoXPAttack = IsAutoXPAttack;
            }
            if (RoleManager.Instance.GetMapHeroSkillProxy() != null)
                RoleManager.Instance.GetMapHeroSkillProxy().IsAutoXPAttack = IsAutoXPAttack;
        }
        
    }

    private void OnClickHeroSkill(EventContext context)
    {
        if (RoleManager.Instance.GetMapHeroSkillProxy() != null)
        {
            RoleManager.Instance.GetMapHeroSkillProxy().CastXPSkill();
        }
    }
    
    private void OnClickSkillItem(EventContext context)
    {
        var index = ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.listSkills.GetChildIndex((GObject) context.data);

        if (index != -1)
        {
            bool isHasSkill = false;
            foreach (var item in RoleManager.Instance.GetSkillProxyDict())
            {
                if (item.Key == index && item.Value.ProxyAttr.skillXPID > 0)
                {
                    item.Value.CastXPSkill();
                    isHasSkill = true;
                    break;
                }
            }

            if(!isHasSkill)
            {
                //var stateMap =  FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType) index);
                //UIManager.Instance.Toast(stateMap.Item2);
                UIManager.Instance.Toast(ConfigUtils.GetStringByKey(5170));
            }
        }
    }
    
    protected override void OnUpdate()
    {
        base.OnUpdate();

        ((UI_BossTime)((UI_battleRoot)this.dungeonMain.panel.battleRoot).copyBossTime).title.text = ConfigUtils.FormatStringByKey(36, (int)DungeonMapManager.Instance.timerMonsterBoss.GetRemain());
        ((UI_BossTime)((UI_battleRoot)this.dungeonMain.panel.battleRoot).copyBossTime1).title.text = ConfigUtils.FormatStringByKey(36, (int)DungeonMapManager.Instance.timerMonsterBoss.GetRemain());

         // ((UI_BossTime)((UI_battleRoot)this.dungeonMain.panel.battleRoot).copyBossTime).title.text = StringUtils.GetTimeString((int)DungeonMapManager.Instance.timerMonsterBoss.GetRemain());
        //UpdateBossTimeProgressBar();

        // ulong endTime =  MapObjectManager.Instance.InvincibleEndTime;
        // int totalSecond = (int)(endTime - ServerTimeManager.Instance.CurServerTime);
        // if (totalSecond > 0 && _totalXPSkillCd > 0)
        // {
        //     ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.xpskill.mask.fillAmount = totalSecond/_totalXPSkillCd;
        // }
        // else
        // {
        //     ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.xpskill.mask.fillAmount = 0;
        // }

    }
    private void UpdateBossTimeProgressBar()
    {
        double v = ((UI_BlueBar3) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).copyBossTime).bar.value;
        float curVelocity = 0;
        float maxValue = DungeonMapManager.Instance.MonsterBossTime;
        ((UI_BlueBar3) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).copyBossTime).bar.value = Mathf.SmoothDamp((float) v,
            DungeonMapManager.Instance.timerMonsterBoss.GetRemain() / maxValue * 100, ref curVelocity, Time.deltaTime);
    }
    
    private void UpdateUIInfo()
    {
        UpdateRoleInfo();
        UpdateBossTxInfo();

        for (int i = 0; i < ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.listSkills.numChildren; i++)
        {
            UI_BtnSkill btnSkill = (UI_BtnSkill) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.listSkills.GetChildAt(i);
            if (null != btnSkill)
            {
                btnSkill.mask2.fillAmount = 0;
                btnSkill.touchable = true;
            }
        }
        
        // 处理主动技能CD
        UI_BtnSkill activeSkill = ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.activeSkill;
        activeSkill.mask2.fillAmount = 0;
    
        // 计算实际冷却时间（应用冷却缩减）
        float time = DataManager.Instance.GetRoleData().FightAttrVo.SkillCd;
        float skillCdReduction = DataManager.Instance.GetRoleData().FightAttrVo.SkillCd * ConstDefine.CONFIG_PLACE_EX;
        float actualCD = _activeSkillBaseCD * (1 - skillCdReduction);
        
        //刚进战斗 默认为0，可释放
        actualCD = 0;
        
        CastSkillCD(activeSkill, actualCD);
        
        ((UI_battleRoot) this.dungeonMain.panel.battleRoot).copyBossTime.visible = true;
        ((UI_battleRoot) this.dungeonMain.panel.battleRoot).copyBossTime1.visible = true;
    }

    private void UpdateRoleInfo()
    {
        ((UI_ComUserInfo)this.dungeonMain.panel.userInfo).UpdateUserInfo();
    }

    private List<ConfigMonsterEntryUnit> monsterEntryList =  new List<ConfigMonsterEntryUnit>();
    /// <summary>
    /// boss特性
    /// </summary>
    private void UpdateBossTxInfo()
    {
        monsterEntryList.Clear();
        
        List<int> lstMonsterID = MapObjectManager.Instance.lstMonsterID;
        int monsterId = lstMonsterID[0];
        MapMonsterObject monsterObject = MapObjectManager.Instance.GetMonsterObjectById(monsterId);
        List<int> entryList = new List<int>();
        entryList = monsterObject.entryList;
        List<ConfigMonsterEntryUnit> monsterEntryGroupUnits = new List<ConfigMonsterEntryUnit>();
        if (_dungeonStageUnit != null)
        {
            monsterEntryGroupUnits = ConfigUtils.GetMonsterEntryByGroup(_dungeonStageUnit.EntryGroup);
        }

        if (_eventStageUnit != null)
        {
            monsterEntryGroupUnits = ConfigUtils.GetMonsterEntryByGroup(_eventStageUnit.EntryGroup);
        }

        if (_stageMonsterAttrUnit != null)
        {
            monsterEntryGroupUnits = ConfigUtils.GetMonsterEntryByGroup(_stageMonsterAttrUnit.EntryGroup);
        }

        if (entryList != null && entryList.Count > 0)
        {
            for (int i = 0; i < entryList.Count; i++)
            {
                for (int j = 0; j < monsterEntryGroupUnits.Count; j++)
                {
                    if (entryList[i] == j)
                    {
                        monsterEntryList.Add(monsterEntryGroupUnits[j]);
                    }
                }
            }
        }

        if (monsterEntryList != null && monsterEntryList.Count > 0)
        {
            ((UI_battleRoot) this.dungeonMain.panel.battleRoot).bossTxList.visible = true;
            ((UI_battleRoot)this.dungeonMain.panel.battleRoot).bossTxList.numItems = monsterEntryList.Count;
        }
        else
        {
            ((UI_battleRoot) this.dungeonMain.panel.battleRoot).bossTxList.visible = false;
            ((UI_battleRoot)this.dungeonMain.panel.battleRoot).bossTxList.numItems = 0;
        }
        
        
    }

    private void BossTxRender(int index, GObject item)
    {
        if (monsterEntryList[index] == null) return;
        
        ((UI_BtnBuff)item).icon = UIResource.GetMonsterEntryIcon(monsterEntryList[index].Icon);
        ((UI_BtnBuff)item).data = index;
        ((UI_BtnBuff)item).onClick.Set(OnClickBossTxTips);
    }

    private void OnClickBossTxTips(EventContext context)
    {
        int index = (int)((UI_BtnBuff)context.sender).data;
        
        TipsManger.Instance.ShowPopupTip((UI_BtnBuff)context.sender, Tipstype.CopyBossTx, ConfigUtils.GetTextById(monsterEntryList[index].MonsterEntryName),
            ConfigUtils.GetTextById(monsterEntryList[index].MonsterEntryDoc), monsterEntryList[index].Icon);
    }

    #region 技能相关
    
    private void ItemListSkillRender(int index, GObject item)
    {
        if (_skillIdList.TryGetValue(index, out int skillId))
        {
            item.data = skillId;
            if (skillId > 0)
            {
                ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(skillId);
                ((UI_BtnSkill) item).skillIcon.skillIcon.url = UIResource.GetItemUrl(skillUnit.SkillIcon);
                ((UI_BtnSkill)item).qualityIcon.visible = true;
                ((UI_BtnSkill)item).qualityCtrl.selectedIndex = skillUnit.SkillQuality - 1;
                ((UI_BtnSkill) item).skillIcon.skillIcon.fixMode = true;
            }
        }
        
        if (SkillInfoManager.Instance.UnLockSkillPos > index)
        {
            ((UI_BtnSkill) item).hasSkill.selectedIndex = skillId > 0 ? 0 : 1;
            ((UI_BtnSkill)item).qualityIcon.visible = skillId > 0;
        }
        else
        {
            ((UI_BtnSkill) item).hasSkill.selectedIndex = 2;
            ((UI_BtnSkill)item).qualityIcon.visible = false;
        }

        if (((UI_BtnSkill)item).hasSkill.selectedIndex == 1)
        {
            ((UI_BtnSkill) item).redDot.visible = SkillInfoManager.Instance.IsHasNoUpLoadSkill();
            ((UI_BtnSkill)item).qualityIcon.visible = false;
        }

    }
    
    private void UpdateSkillList()
    {
        _skillIdList.Clear();
        // var hero = MapObjectManager.Instance.GetLocalHero();
        // if(hero != null)
        //     _skillIdList.Add(0,hero.Attr.skillXPID);
        
        UpdateActiveSkill();// 主动技能
        foreach (var skill in RoleManager.Instance.GetSkillProxyDict())
        {
            if(!_skillIdList.ContainsKey(skill.Key))
                _skillIdList.Add(skill.Key, skill.Value.ProxyAttr.skillXPID);
            skill.Value.IsAutoXPAttack = ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.btnSkillAuto.ctrlAuto.selectedIndex == 1;
        }
        ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.listSkills.numItems = 5;

        //UpdateBottomRedDot();
    }
    
    private void ResetAllSkillCD(float cdTime)
    {
        if (cdTime == 0)
        {
            for (int i = 0; i < ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.listSkills.numChildren; i++)
            {
                UI_BtnSkill btnSkill = (UI_BtnSkill) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.listSkills.GetChildAt(i);
                if (null != btnSkill)
                {
                    btnSkill.touchable = true;
                    btnSkill.mask2.fillAmount = 0;
                    GTween.Kill(btnSkill);
                }
            }
        
            // 处理主动技能CD
            UI_BtnSkill activeSkill = ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.activeSkill;
            activeSkill.mask2.fillAmount = 0;
            GTween.Kill(activeSkill);
        }
        else
        {
            foreach (var tweener in tweenerDic)
            {
                if (tweener.Value != null)
                {
                    float remainingTime = tweener.Value.time - tweener.Value.time * tweener.Value.tween.normalizedTime;
                    float targetTime = remainingTime - tweener.Value.time * cdTime;
                    float timeScale = 1;
                    // 使用反射获取 _timeScale 字段
                    FieldInfo timeScaleField = typeof(GTweener).GetField("_timeScale", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (timeScaleField != null)
                    {
                        float currentTimeScale = (float)timeScaleField.GetValue(tweener.Value.tween);
                        timeScale = currentTimeScale + remainingTime / targetTime;
                        if (timeScale < 1)
                        {
                            timeScale = 1;
                        }
                    }
                    tweener.Value.tween.SetTimeScale(timeScale);
                    //tweener.Value.tween.SetDuration(tweener.Value.tween.duration - tweener.Value.time * cdTime);
                }
            }
        }
    }
    
    // 主动技能
    private void UpdateActiveSkill()
    {
        MapHeroObject heroObject = MapObjectManager.Instance.GetLocalHero();
        if (heroObject != null)
        {
            int activeSkillId = heroObject.HeroAttr.HeroUnit.ActiveSkill;
            ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(activeSkillId);
            ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.activeSkill.skillIcon.skillIcon.url = UIResource.GetHeroSkillIcon(2004602.ToString());//美术要求暂时写死
            ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.activeSkill.qualityIcon.visible = true;
            ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.activeSkill.qualityCtrl.selectedIndex = skillUnit.SkillQuality - 1;
            if (skillUnit != null)
            {
                ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.activeSkill.skillIcon.skillIcon.url = UIResource.GetHeroSkillIcon(2004602.ToString());//美术要求暂时写死
                ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.activeSkill.qualityIcon.visible = true;
                ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.activeSkill.qualityCtrl.selectedIndex = skillUnit.SkillQuality - 1;
            
                // 缓存基础CD
                _activeSkillBaseCD = skillUnit.Cd * ConstDefine.CONFIG_PLACE_EX;
            }
        }
    }
    
    private GTweener tweener;
    private Dictionary<int, SkillCDInfo> tweenerDic = new Dictionary<int, SkillCDInfo>();
    private void CastSkillCD(UI_BtnSkill item, float time)
    {
        GTween.Kill(item);
        tweener = GTween.To(1, 0, time)
            .SetTarget(item)
            .SetEase(EaseType.Linear)
            .OnStart(() =>
            {
                // item.mask.fillAmount = 0;
                item.mask2.fillAmount = 0;
                item.touchable = false;
            })
            .OnUpdate((fillnum) =>
            {
                item.mask2.fillAmount = fillnum.value.x;
                //tweenerDic[item.GID].tween.SetDuration(tweenerDic[item.GID].tween.duration); //重新设置 倒计时时间
            })
            .OnComplete(() =>
            {
                // item.mask.fillAmount = 0;
                item.mask2.fillAmount = 0;
                item.touchable = true;
                tweener = null;
                tweenerDic[item.GID] = null;
            });
        
        SkillCDInfo info = new SkillCDInfo();
        info.tween = tweener;
        info.time = time;
        tweenerDic[item.GID] = info;
    }
    
    private void OnCastSkill(int skillID, EN_CAMP_TYPE campType)
    {
        if (IsShow() && IsOnStage())
        {
            if (campType != EN_CAMP_TYPE.HERO) return;
            
            var skillType = ConfigUtils.GetSkillById(skillID);
            int skillIndex = -1;
            foreach (var item in _skillIdList)
            {
                if (item.Value == skillID)
                {
                    skillIndex = item.Key;
                    break;
                }
            }

            if (skillIndex >= ConstDefine.PetSkillIndex)
                return; //宠物技能  不需要UI表现
            
            // 判断是否为英雄主动技能
            bool isActiveSkill = false;//(skillType.SkillType == (int)EN_SKILL_TYPE.XP);
            if (skillType.SkillType == (int)EN_SKILL_TYPE.XP)
            {
                MapHeroObject heroObject = MapObjectManager.Instance.GetLocalHero();
                if (heroObject != null)
                {
                    int activeSkillId = heroObject.HeroAttr.HeroUnit.ActiveSkill;
                    if (activeSkillId == skillType.Id)
                    {
                        isActiveSkill = true;
                    }
                }
            }
            
            if(skillType != null && (skillType.SkillType == (int) EN_SKILL_TYPE.ChoukaSkill || skillType.SkillType == (int) EN_SKILL_TYPE.XP))
                SceneEffect();
            
            float actualCD;
            if (isActiveSkill)
            {
                actualCD = _activeSkillBaseCD * (1 - DataManager.Instance.GetRoleData().FightAttrVo.SkillCd * ConstDefine.CONFIG_PLACE_EX);
                CastSkillCD(((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.activeSkill, actualCD);
            }

            if (skillType != null && skillIndex != -1)
            {
                float fCDTime = skillType.Cd * ConstDefine.CONFIG_PLACE_EX/MapObjectManager.Instance.Speed;
                fCDTime = Math.Max(0, fCDTime * (1 - DataManager.Instance.GetRoleData().FightAttrVo.SkillCd));
                var skillBtn = ((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills.listSkills.GetChildAt(skillIndex) as UI_BtnSkill;

                if (skillBtn != null)
                {
                    if (skillBtn.tapSpine != null)
                    {
                        skillBtn.tapSpine.visible = true;
                        Utils.PlaySpineAnim(skillBtn.tapSpine, "chuxian", false, () =>
                        {
                            skillBtn.tapSpine.visible = false;
                        });
                    }
 
                    CastSkillCD(skillBtn, fCDTime);
                }
            }
        }

    }
    #endregion
    
    #region 关卡信息更新

    private void OnUpdateStageInfo(ConfigDungeonStageUnit stageUnit, int guankaIndex)
    {
        if (this.IsShow() && this.IsOnStage())
        {
            // ((UI_battleRoot) this.dungeonMain.panel.battleRoot).stageName.text = stageUnit.Name;
            ((UI_battleRoot) this.dungeonMain.panel.battleRoot).copyBossStageName.text = ConfigUtils.GetTextById(stageUnit.Name,stageUnit.NameParam);
            this.mCurGuankaIndex = guankaIndex;
            this.mStageMonsterWave = stageUnit.MonsterData.Count;

            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).visible = this.mStageMonsterWave > 1;
            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).monsterGroupList.numItems = this.mStageMonsterWave;
            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).monsterGroupList.ResizeToFit();
            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).monsterGroupList.EnsureBoundsCorrect();
        }

    }

    private void OnUpdateStageInfo(ConfigEventStageUnit stageUnit, int guankaIndex)
    {
        if (this.IsShow() && this.IsOnStage())
        {
            ((UI_battleRoot) this.dungeonMain.panel.battleRoot).copyBossStageName.text = ConfigUtils.GetTextById(stageUnit.Name);
            this.mCurGuankaIndex = guankaIndex;
            this.mStageMonsterWave = 1;

            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).visible = this.mStageMonsterWave > 1;
            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).monsterGroupList.numItems = this.mStageMonsterWave;
            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).monsterGroupList.ResizeToFit();
            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).monsterGroupList.EnsureBoundsCorrect();
        }

    }
    
    //钓鱼
    private void OnUpdateStageInfo(ConfigStageMonsterAttrUnit stageUnit, int guankaIndex)
    {
        if (this.IsShow() && this.IsOnStage())
        {
            ((UI_battleRoot) this.dungeonMain.panel.battleRoot).copyBossStageName.text = "普通Boss";
            this.mCurGuankaIndex = guankaIndex;
            this.mStageMonsterWave = 1;

            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).visible = this.mStageMonsterWave > 1;
            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).monsterGroupList.numItems = this.mStageMonsterWave;
            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).monsterGroupList.ResizeToFit();
            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).monsterGroupList.EnsureBoundsCorrect();
        }

    }
    
    private void OnUpdateMonsterWaveInfo(int guankaIndex)
    {
        if (this.IsShow() && this.IsOnStage())
        {
            this.mCurGuankaIndex = guankaIndex;
            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).monsterGroupList.numItems = this.mStageMonsterWave;
            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).monsterGroupList.ResizeToFit();
            ((UI_MonsterGroupBar) ((UI_battleRoot) this.dungeonMain.panel.battleRoot).monsterGroup).monsterGroupList.EnsureBoundsCorrect();
        }
    }

    #endregion
    
    public GComponent GetBattleRoot()
    {
        return this.dungeonMain.panel.battleRoot;
    }
    
    public GComponent GetRoot()
    {
        return this.dungeonMain;
    }

    public GComponent GetBattleBGRoot()
    {
        return ((UI_battleRoot) this.dungeonMain.panel.battleRoot).bg;
    }

    public GComponent GetBattleSceneRoot()
    {
        return ((UI_battleRoot) this.dungeonMain.panel.battleRoot).scene;
    }

    public GComponent GetBattleSceneObjRoot()
    {
        return ((UI_battleRoot) this.dungeonMain.panel.battleRoot).scene.obj;
    }

    public GComponent GetBattleSceneFlyRoot()
    {
        return ((UI_battleRoot) this.dungeonMain.panel.battleRoot).scene.fly;
    }
    
    // private float _totalXPSkillCd;
    // private void UpdateXPSkill()
    // {
    //     MapHeroObject heroObject = MapObjectManager.Instance.GetLocalHero();
    //     if (heroObject != null)
    //     {
    //         ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(heroObject.HeroAttr.HeroUnit.SkillXp);
    //         _totalXPSkillCd = skillUnit.Cd * ConstDefine.CONFIG_PLACE_TIME;
    //         ((UI_ComSkills)(((UI_battleRoot) this.dungeonMain.panel.battleRoot).comSkills)).xpskill.skillIcon.skillIcon.url = UIResource.GetSkillIcon(skillUnit.SkillIcon);
    //     }
    // }
    
    private void SceneEffect()
    {
        int time = Random.Range(1, 4);
        ((UI_battleRoot)this.dungeonMain.panel.battleRoot).scene.sceneT.Play(time, 1f, null);
        ((UI_battleRoot)this.dungeonMain.panel.battleRoot).bg.sceneT.Play(time, 1f, null);
    }

    /// <summary>
    /// 播放退出场景的BGM
    /// </summary>
    private void PlaySceneBGM()
    {
        var view = UIManager.Instance.FindByName("Lobby") as LobbyView;
        if (view != null)
        {
            view?.PlaySceneBGM();
        }
    }
}
