using System;
using System.Collections.Generic;
using Config;
using UnityEngine;
using Engine;
using EngineBase;
using msg;


public class PVPMapManager : TSingleton<PVPMapManager>
{
    public bool IsInPVP;

    private static int MonsterUnitId = 10000;//怪物自增ID 从10000开始

    // 4      1
    // 3     主角
    // 5      2
    private static List<int> petPosY => MapObjectManager.petPosY;

    private static List<int> petPosX => MapObjectManager.petPosX;

    private float _recoveryTicker = 0;
    private float _totalRecoverTimer = 0;

    public int PVPTotalTime { get; private set; }
    private float _multiHp;
    private float _multiAtk;
    
    public void OnInit()
    {
        ConfigCommonUnit common1001 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(1001);
        _totalRecoverTimer = int.Parse(common1001.Param1);
        
        PVPTotalTime = int.Parse(ConfigDataGroup.GetInstance<ConfigCommon>().Get(17).Param1);
        
        ConfigCommonUnit common14 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(14);
        _multiHp = float.Parse(common14.Param1);
        _multiAtk = float.Parse(common14.Param2);
    }

    public override void Dispose()
    {
        GameManager.Instance.TimerManager.ClearTimerBySource(EN_TIMER_SOURCE.LEGION);
        base.Dispose();
    }
    
    public void Tick(float deltaSeconds)
    {
        if(!IsInPVP) return;
        MapObjectManager.Instance.TickByMapObject(deltaSeconds * MapObjectManager.Instance.Speed);
        LateTickByMapObject(deltaSeconds);
        UpdateState(GuanKaStep);
        _recoveryTicker += deltaSeconds;
        if (_recoveryTicker > _totalRecoverTimer)
        {
            _recoveryTicker -=_totalRecoverTimer;
            var myHero = MapObjectManager.Instance.GetLocalHero();
            if (myHero != null)
            {
                // 恢复生命
                myHero.PlayRecovery();
            }
            var enemyHero = MapObjectManager.Instance.GetHeroObjectById(2);
            if (enemyHero != null)
            {
                // 恢复生命
                enemyHero.PlayRecovery();
            }
        }
    }
    
    public void LateTickByMapObject(float deltaSeconds)
    {
        MapObjectManager.Instance.LateTickByMapObject(deltaSeconds * MapObjectManager.Instance.Speed);
    }
    

    #region 关卡有限状态机

    public EN_GUANKA_STEP GuanKaStep { get; private set; } = EN_GUANKA_STEP.INIT;
    
    public int GuanKaMonsterIndex { get; private set; }

    private CTimer timerShow1 = new CTimer();
    private CTimer timerShow2 = new CTimer();
    private CTimer timreFightResult = new CTimer();
    public CTimer timerPvp { get; private set; } = new CTimer();
    
    public bool fightLose { get; private set; } = false;

    private FightAttrVo fightAttrVo2;
    private FightAttrVo fightAttrVo1;
    public void InitGuanKaFSM()
    {
        // 通关
        var hero = MapObjectManager.Instance.GetLocalHero();
        
        if (hero != null)
        {
            hero.IsAutoAttack = false;
            hero.ClearMovePath();
            hero.EndCurSkill();
        }
        
        foreach (var t in MapObjectManager.Instance.lstPetObj)
        {
            t.IsAutoAttack = false;
            t.ClearMovePath();
            t.EndCurSkill();
        }
        
        GuanKaMonsterIndex = 0;
        GameManager.Instance.TimerManager.ClearTimerBySource(EN_TIMER_SOURCE.LEGION);
        GuanKaStep = EN_GUANKA_STEP.INIT;
        EnterState(GuanKaStep);
    }

    public void ChangeState(EN_GUANKA_STEP newState)
    {
        if (GuanKaStep == newState)
        {
            return;
        }
        
        ExitState(GuanKaStep, newState);

        GuanKaStep = newState;

        EnterState(GuanKaStep);
    }

    private void InitHeroAndPets()
    {
        if (MapObjectManager.Instance.GetLocalHero() == null)
        {
            // 初始形态，刷英雄，刷第一波怪
            fightAttrVo1 = DataManager.Instance.GetRoleData().FightAttrVo.DeepCopy();
            fightAttrVo1.Atk *= _multiAtk;
            fightAttrVo1.HP *= _multiHp;
            var hero = new HeroVo();
            hero.unitID = 1;
            hero.generalsType = HeroInfoManager.Instance.GetMyHero().HeroUnit.Id;
            hero.pos = new Vector3(MapObjectManager.FrontX2, MapObjectManager.HERO_Y, 0);
            hero.rot = Quaternion.Euler(0, 90, 0);
            MapObjectManager.Instance.UpdateHero(hero);
            
            List<PetItemInfo> petInfoList = PetInfoManager.Instance.GetBattlePetList();
            for (int i=petInfoList.Count-1; i >=0; i--)
            {
                PetVo pet = new PetVo();
                pet.unitID = 100 + i;
                pet.generalsType = petInfoList[i].petCfg.Id;
                pet.BattleIndex = petInfoList[i].BattleIndex;
                pet.pos = new Vector3(petPosX[petInfoList[i].BattleIndex], petPosY[petInfoList[i].BattleIndex], 0);
                pet.rot = Quaternion.Euler(0, 90, 0);
                MapObjectManager.Instance.UpdatePet(pet, petInfoList[i]);
                for (int j = MapObjectManager.Instance.lstPetObj.Count-1; j >=0; j--)
                {
                    MapObjectManager.Instance.lstPetObj[j].UpdatePetTotalAttribute(fightAttrVo1);
                }
            }
            
            // 对方，刷英雄，刷第一波怪
            fightAttrVo2 = PvpRankDataManager.Instance.OtherFightAttrVo.DeepCopy();
            fightAttrVo2.Atk *= _multiAtk;
            fightAttrVo2.HP *= _multiHp;
            var enemyHero = new HeroVo();
            enemyHero.CampType = EN_CAMP_TYPE.ENEMY;
            enemyHero.unitID = 2;
            enemyHero.generalsType = PvpRankDataManager.Instance.OtherPvpRankVo.HeroId;
            enemyHero.pos = new Vector3(720 - MapObjectManager.FrontX2, MapObjectManager.HERO_Y, 0);
            enemyHero.rot = Quaternion.Euler(0, -90, 0);
            MapObjectManager.Instance.UpdateHero(enemyHero);
            List<PetItemInfo> enemyPetInfoList = PvpRankDataManager.Instance.GetOtherBattlePetList();
            for (int i= enemyPetInfoList.Count-1; i >=0; i--)
            {
                PetVo pet = new PetVo();
                pet.CampType = EN_CAMP_TYPE.ENEMY;
                pet.unitID = 200 + i;
                pet.generalsType = enemyPetInfoList[i].petCfg.Id;
                pet.BattleIndex = enemyPetInfoList[i].BattleIndex;
                pet.pos = new Vector3(720 - petPosX[enemyPetInfoList[i].BattleIndex], petPosY[enemyPetInfoList[i].BattleIndex], 0);
                pet.rot = Quaternion.Euler(0, -90, 0);
                MapObjectManager.Instance.UpdatePet(pet, enemyPetInfoList[i]);
                for (int j = MapObjectManager.Instance.lstPetObj.Count-1; j >=0; j--)
                {
                    if(MapObjectManager.Instance.lstPetObj[j].PetAttr.Camp == EN_CAMP_TYPE.ENEMY)
                        MapObjectManager.Instance.lstPetObj[j].UpdatePetTotalAttribute(fightAttrVo2);
                }
            }
            RoleManager.Instance.SetHeroSkillProxy();
            RoleManager.Instance.SetEnemyHeroSkillProxy(fightAttrVo2, PvpRankDataManager.Instance.OtherPvpRankVo.HeroId);

            UIManager.Instance.ShowUIPanel("PVPFight",HeroInfoManager.Instance.GetMyHero().HeroUnit.Id,PvpRankDataManager.Instance.OtherPvpRankVo.HeroId, petInfoList, enemyPetInfoList, (double)RoleManager.Instance.TotalFight, (double)PvpRankDataManager.Instance.OtherPvpRankVo.Fight, DataManager.Instance.GetRoleData().userName, PvpRankDataManager.Instance.OtherPvpRankVo.PlayerName);
            GameManager.Instance.TimerManager.SetTimer(2.30f, () =>
            {
                UIManager.Instance.CloseUIPanel("PVPFight");
                ChangeState(EN_GUANKA_STEP.FIGHTING);
            });

        }

    }

    protected void EnterState(EN_GUANKA_STEP state)
    {
        switch (state)
        {
            case EN_GUANKA_STEP.INIT:
                {
                    MapObjectManager.Instance.DestroyMapObjectById(2);
                    MapObjectManager.Instance.DestroyMapObjectById(1);
                    foreach (var pet in MapObjectManager.Instance.lstPetObj)
                    {
                        MapObjectManager.Instance.DestroyMapObjectById(pet.Attr.unitID);
                    }
                    MapObjectManager.Instance.lstPetObj.Clear();
                    for (int i = MapObjectManager.Instance.lstMonsterID.Count-1; i >=0; i--)
                    {
                        int monsterId = MapObjectManager.Instance.lstMonsterID[i];
                        MapObjectManager.Instance.DestroyMapObjectById(monsterId);
                        MapObjectManager.Instance.lstMonsterID.Remove(monsterId);
                    }
                    
                    fightLose = false;

                    InitHeroAndPets();
                    
                }
                break;
            case EN_GUANKA_STEP.FIGHTING:
                {
                    if(fightLose) return;

                    GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.LEGION,1,0.1f, () =>
                    {
                        var hero = MapObjectManager.Instance.GetLocalHero();

                        if (hero != null)
                        {
                            hero.UpdateHeroTotalAttr(fightAttrVo1);
                            hero.IsAutoAttack = true;
                            hero.ClearMovePath();
                        }

                        var enemyHero = MapObjectManager.Instance.GetHeroObjectById(2);

                        if (enemyHero != null)
                        {
                            enemyHero.UpdateHeroTotalAttr(fightAttrVo2);
                            enemyHero.IsAutoAttack = true;
                            enemyHero.ClearMovePath();
                        }

                        foreach (var t in MapObjectManager.Instance.lstPetObj)
                        {
                            t.UpdatePetTotalAttribute();
                            t.IsAutoAttack = true;
                            t.ClearMovePath();
                        }

                        timreFightResult.Startup(1f/MapObjectManager.Instance.Speed);
                        timerPvp.Clear();
                        timerPvp.Startup(PVPMapManager.Instance.PVPTotalTime);
                    });

                }
                break;
            
             case EN_GUANKA_STEP.RESULT:
             {
                    var builder = GlobalFirstPvPBattleFinish_CS.CreateBuilder();
                    builder.OpponentData = PvpRankDataManager.Instance.OpponentData;
                    builder.IsWin = !fightLose;
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_GlobalFirstPvPBattleFinish_CS, builder.Build());
                    
                    timerPvp.Clear();
                    PVPMapManager.Instance.IsInPVP = false;
                    MapObjectManager.Instance.IsStopMapBattle = false;
                    UIManager.Instance.CloseUIPanel("PVPMapMain");
                }
                break;
             
            default:
                break;
        }
    }


    protected void ExitState(EN_GUANKA_STEP state, EN_GUANKA_STEP stateNext)
    {
        switch (state)
        {
            case EN_GUANKA_STEP.FIGHTING:
                {
                    timreFightResult.Clear();
                }
                break;

            default:
                break;
        }
    }

    protected void UpdateState(EN_GUANKA_STEP state)
    {
        switch (state)
        {
            case EN_GUANKA_STEP.FIGHTING:
                {
                    if (timreFightResult.ToNextTime())
                    {
                        if (MapObjectManager.Instance.GetHeroObjectById(2) == null)
                        {
                            fightLose = false;
                            ChangeState(EN_GUANKA_STEP.RESULT);
                        }
                        else if (!MapObjectManager.Instance.IsExistHero())
                        {
                            fightLose = true;
                            ChangeState(EN_GUANKA_STEP.RESULT);
                        }
                    }
                    
                    if (timerPvp.IsActive() && timerPvp.TimeOver())
                    {
                        fightLose = true;
                        ChangeState(EN_GUANKA_STEP.RESULT);
                    }
                }
                break;

            default:
                break;
        }
    }

    #endregion
}
