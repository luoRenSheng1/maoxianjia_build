
using System;
using System.Collections.Generic;
using Config;
using Engine;
using msg;
using UnityEngine;

public class FightUtils
{
    public static bool IsWearEquip = false;
    public static int useRuneSkillNum = 0;
    public static double GetHeroFight()
    {
        double totalFight = 0;

        FightAttrVo fightAttrVo = DataManager.Instance.GetRoleData().FightAttrVo;
        double CriticalStrike = 0f;
        if (Mathf.Approximately(1.0f, (float) fightAttrVo.CriticalStrike))
            CriticalStrike = 1.0f;
        else
        {
            CriticalStrike =(float) fightAttrVo.CriticalStrike;
        }
        //生命转化战斗力公式=	战前总生命值*（1+战前吸血比例）+（1+战前减伤比例）+战前生命恢复
        // double hpFight = fightAttrVo.HP * (1 + fightAttrVo.Bloodsucking) + (1 + fightAttrVo.Mitigation) + fightAttrVo.Recovery;
        //攻击转化战斗力公式=	战前总生攻击      * （1+战前总暴击率）       *（战前总暴击伤害）             *战前总技能伤害*战前总魔法次数*（1+BOSS加成or小怪加成，取平均值）
        //double atkFight = fightAttrVo.Atk * (1 + CriticalStrike) * fightAttrVo.CriticalInjury * (1+fightAttrVo.SkillDamage) * (1+fightAttrVo.MagicTimes) * (1 + (fightAttrVo.BossDamageAdd + fightAttrVo.MonsterDamageAdd) / 2f);
        //战斗力公式=	转化后生命值*1+转化后攻击力*3
        //totalFight = hpFight + atkFight * 3;
        
        // 战前总技能次数  要等传承装备对应的技能ID  判断
        useRuneSkillNum = GetUseRuneSkillTimes();
        
        //生命转化战斗力公式=	战前总生命值*（1+战前吸血比例）*（1+闪避率）+（1+战前减伤比例）+战前生命恢复+战前攻击回复
        double hpFight = fightAttrVo.HP * (1 + fightAttrVo.Bloodsucking) * (1 + fightAttrVo.JoukRate) * (1 + fightAttrVo.Mitigation) + fightAttrVo.Recovery + fightAttrVo.AtkHPRecovery;
        //攻击转化战斗力公式=	战前总主属性*(1+总伤害倍率)*（1+战前总暴击率）*（战前总暴击伤害）*战前总技能伤害*战前总技能次数*（1+BOSS加成or小怪加成，取平均值）*（1+战前总无视防御率）*（1+战前总最终伤害）+战前总宠物伤害
        double atkFight = fightAttrVo.Atk * (1 + fightAttrVo.AtkMultiple) * (1 + CriticalStrike) * fightAttrVo.CriticalInjury * (1 + fightAttrVo.SkillDamage) * (1 + useRuneSkillNum) *
            (1 + (fightAttrVo.BossDamageAdd + fightAttrVo.MonsterDamageAdd) / 2f) * (1 + fightAttrVo.IgnoreDef) * fightAttrVo.BattleFinalAttack + fightAttrVo.PetAtk;
        //防御转化战斗力公式=	战前总物理防御+战前总魔法防御+战前总道术防御+（战前总格挡率*战前总格挡值）
        double defFight = fightAttrVo.PhysicDef + fightAttrVo.MagicDef + fightAttrVo.SorceryDef + (fightAttrVo.ParryRate * fightAttrVo.ParryValue);
        //战斗力公式=	转化后生命值*1+转化后攻击力*3+转化后防御力*1
        totalFight = hpFight + atkFight * 3 + defFight;
        
        //todo 打印
        // Debug.Log($"=11==HP=={fightAttrVo.HP}==Bloodsucking={fightAttrVo.Bloodsucking}==JoukRate={fightAttrVo.JoukRate}==Mitigation={fightAttrVo.Mitigation}==Recovery={fightAttrVo.Recovery}==AtkHPRecovery={fightAttrVo.AtkHPRecovery}");
        // Debug.Log($"=11==Atk=={fightAttrVo.Atk}==AtkMultiple={fightAttrVo.AtkMultiple}==CriticalStrike={CriticalStrike}==CriticalInjury={fightAttrVo.CriticalInjury}==SkillDamage={fightAttrVo.SkillDamage}" +
        //           $"===useRuneSkillNum={useRuneSkillNum}==BossDamageAdd={fightAttrVo.BossDamageAdd}==MonsterDamageAdd={fightAttrVo.MonsterDamageAdd}==IgnoreDef={fightAttrVo.IgnoreDef}==PetAtk={fightAttrVo.PetAtk}");
        // Debug.Log($"=11==totalFight={totalFight}==hpFight=={hpFight}==atkFight={atkFight}==defFight={defFight}==PhysicDef=={fightAttrVo.PhysicDef}==MagicDef={fightAttrVo.MagicDef}==SorceryDef={fightAttrVo.SorceryDef}==ParryRate={fightAttrVo.ParryRate}==ParryValue={fightAttrVo.ParryValue}");
        
        return Math.Ceiling(totalFight);
    }
    
    public static double GetTotalFight(double HP, double Bloodsucking, double Mitigation, double Recovery, double Atk, double CriticalStrike, double CriticalInjury, double SkillDamage, 
        double MagicTimes, double BossDamageAdd, double MonsterDamageAdd, double AtkHPRecovery,double JoukRate,double AtkMultiple,double IgnoreDef, double BattleFinalAttack,
        double PetAtk,double PhysicDef,double MagicDef,double SorceryDef,double ParryRate,double ParryValue)
    {
        double totalFight = 0;
        if (Mathf.Approximately(1.0f, (float) CriticalStrike))
            CriticalStrike = 1.0f;
        
        //生命转化战斗力公式=	战前总生命值*（1+战前吸血比例）*（1+闪避率）+（1+战前减伤比例）+战前生命恢复+战前攻击回复
        //攻击转化战斗力公式=	战前总主属性*(1+总伤害倍率)*（1+战前总暴击率）*（战前总暴击伤害）*战前总技能伤害*战前总技能次数*（1+BOSS加成or小怪加成，取平均值）*（1+战前总无视防御率）*（1+战前总最终伤害）+战前总宠物伤害
        //防御转化战斗力公式	战前总物理防御+战前总魔法防御+战前总道术防御+（战前总格挡率*战前总格挡值）
        //战斗力公式=	转化后生命值*1+转化后攻击力*3+转化后防御力*1
        
        int maxMagicTimes = GetUseRuneSkillTimes();
        
        double hpFight = HP * (1 + Bloodsucking) * (1 + JoukRate) * (1 + Mitigation) + Recovery + AtkHPRecovery;
        double atkFight = Atk * (1 + AtkMultiple) * (1 + CriticalStrike) * CriticalInjury * (1 + SkillDamage) * (1 + maxMagicTimes) *
            (1 + (BossDamageAdd + MonsterDamageAdd) / 2f) * (1 + IgnoreDef)  * BattleFinalAttack + PetAtk;
        double defFight = PhysicDef + MagicDef + SorceryDef + (ParryRate * ParryValue);
        totalFight = hpFight + atkFight * 3 + defFight;
        
        //todo 打印
        // Debug.Log($"=22==HP=={HP}==Bloodsucking={Bloodsucking}==JoukRate={JoukRate}==Mitigation={Mitigation}==Recovery={Recovery}==AtkHPRecovery={AtkHPRecovery}");
        // Debug.Log($"=22==Atk=={Atk}==AtkMultiple={AtkMultiple}==CriticalStrike={CriticalStrike}==CriticalInjury={CriticalInjury}==SkillDamage={SkillDamage}" +
        //           $"===useRuneSkillNum={maxMagicTimes}==BossDamageAdd={BossDamageAdd}==MonsterDamageAdd={MonsterDamageAdd}==IgnoreDef={IgnoreDef}==PetAtk={PetAtk}");
        // Debug.Log($"=22==totalFight={totalFight}==hpFight=={hpFight}==atkFight={atkFight}==defFight={defFight}==PhysicDef=={PhysicDef}==MagicDef={MagicDef}==SorceryDef={SorceryDef}==ParryRate={ParryRate}==ParryValue={ParryValue}");

        return Math.Ceiling(totalFight);
        
        // double hpFight = HP * (1 + Bloodsucking) + (1 + Mitigation) + Recovery;
        // double atkFight = Atk * (1 + CriticalStrike) * CriticalInjury * (1 + SkillDamage) * (1+ MagicTimes) * (1 + (BossDamageAdd + MonsterDamageAdd) / 2f);
        // totalFight = hpFight + atkFight * 3;
        // double total = 0;
        // double runFight = atkFight * maxMagicTimes;//Atk * (1 + CriticalStrike) * CriticalInjury * (1 + SkillDamage) * (1 + maxMagicTimes) *(1 + (BossDamageAdd + MonsterDamageAdd) / 2f);
        // total = runFight* maxMagicTimes;
        // double runFight = atkFight * maxMagicTimes;
        // return Math.Ceiling(totalFight + runFight);
    }
    
    private static Dictionary<int,double> equipDataDic = new Dictionary<int,double>();
    private static Dictionary<int,double> oldEquipDataDic = new Dictionary<int,double>();
    public static double GetOneEquipFight(EquipData equipData ,EquipData oldEquipData, bool isWear)
    {
        double hp = 0, atkValue = 0, bloodsucking = 0, mitigation = 0, recovery = 0f, JoukRate = 0f, HPMultiple = 0f, AtkMultiple = 0f, IgnoreDef = 0f, BattleFinalAttack =0f, PetAtk = 0f;
        float criticalStrike = 0, criticalInjury = 0, bossDamageAdd = 0, monsterDamageAdd = 0f, AtkHPRecovery = 0f;
        float skillDamage = 0f, magicTimes = 0f;
        double hp_add = 0f, atk_add = 0f, harmTypePhy_atkAdd = 0f, harmTypeMag_atkAdd = 0f, harmTypeSor_atkAdd = 0f, PhysicDefAdd = 0f, MagicDefAdd = 0f, SorceryDefAdd = 0f;
        double PhysicDef = 0f,MagicDef = 0f,SorceryDef = 0f,ParryRate = 0f,ParryValue = 0f,PhysicAtk= 0f,MagicAtk= 0f,SorceryAtk= 0f;
        double earth_atkAdd = 0f, water_atkAdd = 0f, fire_atkAdd = 0f, air_atkAdd = 0f;
        double orgAtk = 0f, orgAtkAdd = 0f, workAtk = 0f, orgWorkAtkAdd = 0f, orgVocationAtkAdd = 0f, harmType_atkAdd = 0f, vocation_atkAdd = 0f;
        
        double hpR=0, atkValueR=0,bloodsuckingR=0,mitigationR=0,recoveryR = 0f, JoukRateR = 0f, HPMultipleR = 0f, AtkMultipleR = 0f, IgnoreDefR = 0f, BattleFinalAttackR =0f, PetAtkR = 0f;
        float criticalStrikeR=0, criticalInjuryR=0, bossDamageAddR=0, monsterDamageAddR=0f,AtkHPRecoveryR = 0f;
        float skillDamageR = 0f, magicTimesR = 0f;
        double hp_addR = 0f, atk_addR = 0f, harmTypePhy_atkAddR = 0f, harmTypeMag_atkAddR = 0f, harmTypeSor_atkAddR = 0f, PhysicDefAddR = 0f, MagicDefAddR = 0f, SorceryDefAddR = 0f;
        double PhysicDefR = 0f,MagicDefR = 0f,SorceryDefR = 0f,ParryRateR = 0f,ParryValueR = 0f,PhysicAtkR= 0f,MagicAtkR= 0f,SorceryAtkR= 0f;
        double earth_atkAddR = 0f, water_atkAddR = 0f, fire_atkAddR = 0f, air_atkAddR = 0f;
        double orgAtkR = 0f, orgAtkAddR = 0f, workAtkR = 0f, orgWorkAtkAddR = 0f, orgVocationAtkAddR = 0f, harmType_atkAddR = 0f, vocation_atkAddR = 0f;
        
        for (int i = 0; i < equipData.lstAttrsID.Count; i++)
        {
            switch ((EN_BUFF_ADD_TYPE)equipData.lstAttrsID[i])
            {
                case EN_BUFF_ADD_TYPE.PhysicAtk:  // 1
                    PhysicAtk += equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.MagicAtk:  // 2
                    MagicAtk += equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.SorceryAtk:  // 3
                    SorceryAtk += equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.HP:  // 4
                    hp += equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.PhysicDef:  // 5
                    PhysicDef += equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.MagicDef:  // 6
                    MagicDef += equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.SorceryDef:  // 7
                    SorceryDef += equipData.lstAttrsValue[i];
                    break;
                // case EN_BUFF_ADD_TYPE.ATK_ADD:
                //     atk_add = equipData.lstAttrsValue[i] * DataManager.Instance.GetRoleData().FightAttrVo.Atk;
                //     break;
                case EN_BUFF_ADD_TYPE.Recovery:  // 8
                    recovery += equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.PetAtk:  // 9
                    PetAtk += equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.PhysicAtkADD: // 10
                    harmTypePhy_atkAdd = equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.MagicAtkADD: // 11
                    harmTypeMag_atkAdd = equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.SorceryAtkADD: // 12
                    // atk_add = equipData.lstAttrsValue[i];
                    harmTypeSor_atkAdd = equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.HP_ADD: // 13
                    hp_add += equipData.lstAttrsValue[i] * DataManager.Instance.GetRoleData().FightAttrVo.HP;
                    break;
                case EN_BUFF_ADD_TYPE.PhysicDefADD: // 14
                    PhysicDefAdd = equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.MagicDefADD: // 15
                    MagicDefAdd = equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.SorceryDefADD: // 16
                    SorceryDefAdd = equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.EarthAtkADD: // 17
                    earth_atkAdd = equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.WaterAtkADD: // 18
                    water_atkAdd = equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.FireAtkADD: // 19
                    fire_atkAdd = equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.AirAtkADD: // 20
                    // atk_add += equipData.lstAttrsValue[i];
                    air_atkAdd = equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.HPMultiple:  // 22
                    HPMultiple += (float)equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.AtkMultiple:  // 23
                    AtkMultiple += equipData.lstAttrsValue[i];
                    break;
                
                case EN_BUFF_ADD_TYPE.JoukRate:  // 24
                    JoukRate += equipData.lstAttrsValue[i];
                    break;
                
                case EN_BUFF_ADD_TYPE.ParryRate:  // 26
                    ParryRate += equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.ParryValue:  // 27
                    ParryValue += equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.IgnoreDef:  //28
                    IgnoreDef += equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.CriticalStrike:  // 29
                    criticalStrike += (float)equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.CriticalInjury: // 30
                    criticalInjury += (float)equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.BossDamageAdd:  // 31
                    bossDamageAdd += (float)equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.MonsterDamageAdd:   // 32
                    monsterDamageAdd += (float)equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.Mitigation:  // 33
                    mitigation += equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.Bloodsucking:  // 34
                    bloodsucking += equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.AtkHPRecovery:  // 35
                    AtkHPRecovery += (float)equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.SkillDamage:  // 36
                    skillDamage += (float)equipData.lstAttrsValue[i];
                    break;
                
                case EN_BUFF_ADD_TYPE.MagicTimes:  // 38
                    magicTimes += (float)equipData.lstAttrsValue[i];
                    break;
                
                case EN_BUFF_ADD_TYPE.BattleFinalAttack:  // 48
                    BattleFinalAttack += equipData.lstAttrsValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.ATK:   // 101 攻击
                    atkValue += equipData.lstAttrsValue[i];
                    break;
            }
        }
        for (int i = 0; i < equipData.lstEntrysID.Count; i++)
        {
            switch ((EN_BUFF_ADD_TYPE)equipData.lstEntrysID[i])
            {
                case EN_BUFF_ADD_TYPE.PhysicAtk:  // 1
                    PhysicAtk += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.MagicAtk:  // 2
                    MagicAtk += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.SorceryAtk:  // 3
                    SorceryAtk += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.HP:  // 4
                    hp += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.PhysicDef:  // 5
                    PhysicDef += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.MagicDef:  // 6
                    MagicDef += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.SorceryDef:  // 7
                    SorceryDef += equipData.lstEntrysValue[i];
                    break;
                // case EN_BUFF_ADD_TYPE.ATK_ADD:
                //     atk_add += equipData.lstEntrysValue[i];
                //     break;
                case EN_BUFF_ADD_TYPE.Recovery:  // 8
                    recovery += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.PetAtk:  // 9
                    PetAtk += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.PhysicAtkADD: // 10
                    harmTypePhy_atkAdd += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.MagicAtkADD: // 11
                    harmTypeMag_atkAdd += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.SorceryAtkADD: // 12
                    harmTypeSor_atkAdd += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.HP_ADD:  //13
                    hp_add += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.PhysicDefADD: // 14
                    PhysicDefAdd += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.MagicDefADD: // 15
                    MagicDefAdd += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.SorceryDefADD: // 16
                    SorceryDefAdd += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.EarthAtkADD: // 17
                    earth_atkAdd += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.WaterAtkADD: // 18
                    water_atkAdd += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.FireAtkADD: // 19
                    fire_atkAdd += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.AirAtkADD: // 20
                    air_atkAdd += equipData.lstEntrysValue[i];
                    break;
                
                case EN_BUFF_ADD_TYPE.HPMultiple:  // 22
                    HPMultiple += (float)equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.AtkMultiple:  // 23
                    AtkMultiple += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.JoukRate:  // 24
                    JoukRate += equipData.lstEntrysValue[i];
                    break;
                
                case EN_BUFF_ADD_TYPE.ParryRate:  // 26
                    ParryRate += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.ParryValue:  // 27
                    ParryValue += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.IgnoreDef:  // 28
                    IgnoreDef += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.CriticalStrike:  // 29
                    criticalStrike += (float)equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.CriticalInjury:  // 30
                    criticalInjury += (float)equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.BossDamageAdd:  // 31
                    bossDamageAdd += (float)equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.MonsterDamageAdd:  // 32
                    monsterDamageAdd += (float)equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.Mitigation:  // 33
                    mitigation += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.Bloodsucking:  // 34
                    bloodsucking += equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.AtkHPRecovery:  // 35
                    AtkHPRecovery += (float)equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.SkillDamage:  // 36
                    skillDamage += (float)equipData.lstEntrysValue[i];
                    break;
                case EN_BUFF_ADD_TYPE.MagicTimes:  // 38
                    magicTimes += (float)equipData.lstEntrysValue[i];
                    break;
                
                case EN_BUFF_ADD_TYPE.BattleFinalAttack:  // 48
                    BattleFinalAttack += equipData.lstEntrysValue[i];
                    break;
                
                case EN_BUFF_ADD_TYPE.ATK:  // 101
                    atkValue += equipData.lstEntrysValue[i];
                    break;
            }
        }
        
        FightAttrVo fightAttrVo = DataManager.Instance.GetRoleData().FightAttrVo;
        if (fightAttrVo.HarmType == (int)EN_HARM_TYPE.PhysicAtk)
        {
            workAtk = PhysicAtk; // 词条直接增加职业攻击力
            harmType_atkAdd = harmTypePhy_atkAdd; // 词条直接增加职业攻击力加成
            //orgAtkAdd = (fightAttrVo.Atk - fightAttrVo.PhysicAtk)/fightAttrVo.PhysicAtk;
            orgAtk = fightAttrVo.PhysicAtk; // 基础攻击力
            orgWorkAtkAdd = fightAttrVo.PhysicAtkADD; // 基础职业攻击力加成
        }
        else if (fightAttrVo.HarmType == (int)EN_HARM_TYPE.MagicAtk)
        {
            workAtk = MagicAtk;
            harmType_atkAdd = harmTypeMag_atkAdd;
            //orgAtkAdd = (fightAttrVo.Atk - fightAttrVo.MagicAtk)/fightAttrVo.MagicAtk;
            orgAtk = fightAttrVo.MagicAtk;
            orgWorkAtkAdd = fightAttrVo.MagicAtkADD;
        }
        else if (fightAttrVo.HarmType == (int)EN_HARM_TYPE.SorceryAtk)
        {
            workAtk = SorceryAtk;
            harmType_atkAdd = harmTypeSor_atkAdd;
            //orgAtkAdd = (fightAttrVo.Atk - fightAttrVo.SorceryAtk)/fightAttrVo.SorceryAtk;
            orgAtk = fightAttrVo.SorceryAtk;
            orgWorkAtkAdd = fightAttrVo.SorceryAtkADD;
        }

        if (fightAttrVo.HarmAttrAtkAdd == (int)EN_HARM_ATTR_TYPE.Earth)
        {
            vocation_atkAdd = earth_atkAdd;  // 词条直接增加 系 攻击力加成
            orgVocationAtkAdd = fightAttrVo.EarthAtkADD;  // 基础系攻击力加成
        }
        else if (fightAttrVo.HarmAttrAtkAdd == (int)EN_HARM_ATTR_TYPE.Water)
        {
            vocation_atkAdd = water_atkAdd;
            orgVocationAtkAdd = fightAttrVo.WaterAtkADD;
        }
        else if (fightAttrVo.HarmAttrAtkAdd == (int)EN_HARM_ATTR_TYPE.Fire)
        {
            vocation_atkAdd = fire_atkAdd;
            orgVocationAtkAdd = fightAttrVo.FireAtkADD;
        }
        else if(fightAttrVo.HarmAttrAtkAdd == (int)EN_HARM_ATTR_TYPE.Air)
        {
            vocation_atkAdd = air_atkAdd;
            orgVocationAtkAdd = fightAttrVo.AirAtkADD;
        }
        //atk_add = harmType_atkAdd + vocation_atkAdd;

        if (oldEquipData != null)
        {
            for (int i = 0; i < oldEquipData.lstAttrsID.Count; i++)
            {
                switch ((EN_BUFF_ADD_TYPE)oldEquipData.lstAttrsID[i])
                {
                    case EN_BUFF_ADD_TYPE.PhysicAtk:  // 1
                        PhysicAtkR += oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.MagicAtk:  // 2
                        MagicAtkR += oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.SorceryAtk:  // 3
                        SorceryAtkR += oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.HP:    // 4
                        hpR += oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.PhysicDef:  // 5
                        PhysicDefR += oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.MagicDef:  // 6
                        MagicDefR += oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.SorceryDef:  // 7
                        SorceryDefR += oldEquipData.lstAttrsValue[i];
                        break;
                    // case EN_BUFF_ADD_TYPE.ATK_ADD:
                    //     atk_addR = oldEquipData.lstAttrsValue[i] * DataManager.Instance.GetRoleData().FightAttrVo.Atk;
                    //     break;
                    case EN_BUFF_ADD_TYPE.Recovery:    // 8
                        recoveryR += oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.PetAtk:  // 9
                        PetAtkR += oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.PhysicAtkADD: // 10
                        harmTypePhy_atkAddR = oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.MagicAtkADD: // 11
                        harmTypeMag_atkAddR = oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.SorceryAtkADD: // 12
                        harmTypeSor_atkAddR = oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.HP_ADD: // 13
                        hp_addR += oldEquipData.lstAttrsValue[i] * DataManager.Instance.GetRoleData().FightAttrVo.HP;
                        break;
                    case EN_BUFF_ADD_TYPE.PhysicDefADD: // 14
                        PhysicDefAddR = oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.MagicDefADD: // 15
                        MagicDefAddR = oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.SorceryDefADD: // 16
                        SorceryDefAddR = oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.EarthAtkADD: // 17
                        earth_atkAddR = oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.WaterAtkADD: // 18
                        water_atkAddR = oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.FireAtkADD: // 19
                        fire_atkAddR = oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.AirAtkADD: // 20
                        air_atkAddR = oldEquipData.lstAttrsValue[i];
                        break;
                    
                    case EN_BUFF_ADD_TYPE.HPMultiple:  // 22
                        HPMultipleR += (float)oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.AtkMultiple:  // 23
                        AtkMultipleR += oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.JoukRate:  // 24
                        JoukRateR += oldEquipData.lstAttrsValue[i];
                        break;
                    
                    case EN_BUFF_ADD_TYPE.ParryRate:  // 26
                        ParryRateR += oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.ParryValue:  // 27
                        ParryValueR += oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.IgnoreDef:  // 28
                        IgnoreDefR += oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.CriticalStrike:    // 29
                        criticalStrikeR += (float)oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.CriticalInjury:    // 30
                        criticalInjuryR += (float)oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.BossDamageAdd:    // 31
                        bossDamageAddR += (float)oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.MonsterDamageAdd:    // 32
                        monsterDamageAddR += (float)oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.Mitigation:    // 33
                        mitigationR += oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.Bloodsucking:    // 34
                        bloodsuckingR += oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.AtkHPRecovery:  // 35
                        AtkHPRecoveryR += (float)oldEquipData.lstAttrsValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.SkillDamage:  // 36
                        skillDamageR += (float)oldEquipData.lstAttrsValue[i];
                        break;
                    
                    case EN_BUFF_ADD_TYPE.MagicTimes:    // 38
                        magicTimesR += (float)oldEquipData.lstAttrsValue[i];
                        break;
                    
                    case EN_BUFF_ADD_TYPE.BattleFinalAttack:  // 48
                        BattleFinalAttackR += oldEquipData.lstAttrsValue[i];
                        break;
                    
                    case EN_BUFF_ADD_TYPE.ATK:    // 101
                        atkValueR += oldEquipData.lstAttrsValue[i];
                        break;
                }
            }
            
            for (int i = 0; i < oldEquipData.lstEntrysID.Count; i++)
            {
                switch ((EN_BUFF_ADD_TYPE)oldEquipData.lstEntrysID[i])
                {
                    case EN_BUFF_ADD_TYPE.PhysicAtk:  // 1
                        PhysicAtkR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.MagicAtk:  // 2
                        MagicAtkR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.SorceryAtk:  // 3
                        SorceryAtkR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.HP:  // 4
                        hpR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.PhysicDef:  // 5
                        PhysicDefR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.MagicDef:  // 6
                        MagicDefR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.SorceryDef:  // 7
                        SorceryDefR += oldEquipData.lstEntrysValue[i];
                        break;
                    // case EN_BUFF_ADD_TYPE.ATK_ADD:
                    //     atk_addR += oldEquipData.lstEntrysValue[i];
                    //     break;
                    case EN_BUFF_ADD_TYPE.Recovery:  // 8
                        recoveryR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.PetAtk:  // 9
                        PetAtkR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.PhysicAtkADD: // 10
                        harmTypePhy_atkAddR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.MagicAtkADD: // 11
                        harmTypeMag_atkAddR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.SorceryAtkADD: // 12
                        harmTypeSor_atkAddR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.HP_ADD:  //13
                        hp_addR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.PhysicDefADD: // 14
                        PhysicDefAddR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.MagicDefADD: // 15
                        MagicDefAddR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.SorceryDefADD: // 16
                        SorceryDefAddR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.EarthAtkADD: // 17
                        earth_atkAddR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.WaterAtkADD: // 18
                        water_atkAddR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.FireAtkADD: // 19
                        fire_atkAddR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.AirAtkADD: // 20
                        air_atkAddR += oldEquipData.lstEntrysValue[i];
                        break;
                    
                    case EN_BUFF_ADD_TYPE.HPMultiple:  // 22
                        HPMultipleR += (float)oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.AtkMultiple:  // 23
                        AtkMultipleR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.JoukRate:  // 24
                        JoukRateR += oldEquipData.lstEntrysValue[i];
                        break;
                    
                    case EN_BUFF_ADD_TYPE.ParryRate:  // 26
                        ParryRateR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.ParryValue:  // 27
                        ParryValueR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.IgnoreDef:  // 28
                        IgnoreDefR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.CriticalStrike:  // 29
                        criticalStrikeR += (float) oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.CriticalInjury:  // 30
                        criticalInjuryR += (float) oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.BossDamageAdd:  // 31
                        bossDamageAddR += (float) oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.MonsterDamageAdd:  // 32
                        monsterDamageAddR += (float) oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.Mitigation:  // 33
                        mitigationR += (float) oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.Bloodsucking:  // 34
                        bloodsuckingR += oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.AtkHPRecovery:  // 35
                        AtkHPRecoveryR += (float)oldEquipData.lstEntrysValue[i];
                        break;
                    case EN_BUFF_ADD_TYPE.SkillDamage:  // 36
                        skillDamageR += (float) oldEquipData.lstEntrysValue[i];
                        break;
                    
                    case EN_BUFF_ADD_TYPE.MagicTimes:  // 38
                        magicTimesR += (float)oldEquipData.lstEntrysValue[i];
                        break;
                    
                    case EN_BUFF_ADD_TYPE.BattleFinalAttack:  // 48
                        BattleFinalAttackR += oldEquipData.lstEntrysValue[i];
                        break;
                    
                    case EN_BUFF_ADD_TYPE.ATK:  // 101
                        atkValueR += oldEquipData.lstEntrysValue[i];
                        break;
                }
            }
            
            if (fightAttrVo.HarmType == (int)EN_HARM_TYPE.PhysicAtk)
            {
                workAtkR = PhysicAtkR; 
                harmType_atkAddR = harmTypePhy_atkAddR;
                //orgAtkAddR = (fightAttrVo.Atk - fightAttrVo.PhysicAtk)/fightAttrVo.PhysicAtk;
            }
            else if (fightAttrVo.HarmType == (int)EN_HARM_TYPE.MagicAtk)
            {
                workAtkR = MagicAtkR;
                harmType_atkAddR = harmTypeMag_atkAddR;
                //orgAtkAddR = (fightAttrVo.Atk - fightAttrVo.MagicAtk)/fightAttrVo.MagicAtk;
            }
            else if (fightAttrVo.HarmType == (int)EN_HARM_TYPE.SorceryAtk)
            {
                workAtkR = SorceryAtkR;
                harmType_atkAddR = harmTypeSor_atkAddR;
                //orgAtkAddR = (fightAttrVo.Atk - fightAttrVo.SorceryAtk)/fightAttrVo.SorceryAtk;
            }
            if (fightAttrVo.HarmAttrAtkAdd == (int)EN_HARM_ATTR_TYPE.Earth)
            {
                vocation_atkAddR = earth_atkAdd;
            }
            else if (fightAttrVo.HarmAttrAtkAdd == (int)EN_HARM_ATTR_TYPE.Water)
            {
                vocation_atkAddR = water_atkAdd;
            }
            else if (fightAttrVo.HarmAttrAtkAdd == (int)EN_HARM_ATTR_TYPE.Fire)
            {
                vocation_atkAddR = fire_atkAdd;
            }
            else if(fightAttrVo.HarmAttrAtkAdd == (int)EN_HARM_ATTR_TYPE.Air)
            {
                vocation_atkAddR = air_atkAdd;
            }
            atk_addR = harmType_atkAddR + vocation_atkAddR;
        }


        double fHp,
            //orgAtk,
            orgHp,
            fAtk,
            fHpAdd,
            fAtkAdd,
            fBloodsucking,
            fMitigation,
            fRecovery,
            fCriticalStrike,
            fCriticalInjury,
            fSkillDamage,
            fMagicTimes,
            fBossDamageAdd,
            fMonsterDamageAdd,
            
            orgPhysicDef,
            orgMagicDef,
            orgSorceryDef,
            fJoukRate,    //新加
            fAtkHPRecovery,
            fAtkMultiple,
            fIgnoreDef,
            fPetAtk,
            fPhysicDef,
            fMagicDef,
            fSorceryDef,
            fParryRate,
            fParryValue,
            fBattleFinalAttack;
        double fTotalFight = 0;
        
        
        //第一步，先算出，除去穿戴那件装备的属性
        // orgHp = fightAttrVo.HP / (1 + fightAttrVo.HPADD + fightAttrVo.HPADD + HPMultipleR) - hpR ;
        orgHp = fightAttrVo.HP / ((1 + fightAttrVo.HPADD) * (1 + fightAttrVo.HPMultiple )) - hpR;
        fHp = orgHp * (1 + fightAttrVo.HPADD - hp_addR) * (1 + fightAttrVo.HPMultiple - HPMultipleR);
        fHpAdd = fightAttrVo.HPADD - hp_addR;
        // orgHp = fightAttrVo.HP / (1 + fightAttrVo.HPADD) - hpR;
        // fHp = orgHp * (1 + fightAttrVo.HPADD - hp_addR);
        // fHpAdd = fightAttrVo.HPADD - hp_addR;

        orgAtk = orgAtk - atkValueR -  workAtkR;  //初始攻击力 = 原始攻击力 - 攻击力词条增加的攻击力 - 职业增加的攻击力
        fAtk = orgAtk * (1 + orgWorkAtkAdd - harmTypePhy_atkAddR) * (1 + orgVocationAtkAdd - vocation_atkAddR); // 原始总攻击力 = 初始攻击力 * （1 + 攻击力加成）*(1+系攻击力加成)
        //fAtkAdd = orgAtkAdd - atk_addR;
        
        // orgAtk = fightAttrVo.Atk / (1 + fightAttrVo.ATKADD) - atkR;
        // fAtk = orgAtk * (1 + fightAttrVo.ATKADD - atk_addR);
        // fAtkAdd = fightAttrVo.ATKADD - atk_addR;
        
        fBloodsucking = fightAttrVo.Bloodsucking - bloodsuckingR;  //吸血比率
        fMitigation = fightAttrVo.Mitigation - mitigationR;   //减伤比率
        fRecovery = fightAttrVo.Recovery - recoveryR;    // 生命恢复
        fCriticalInjury = fightAttrVo.CriticalInjury - criticalInjuryR;   // 爆伤比率（爆伤的伤害倍数）  暴击伤害百分比
        fCriticalStrike = fightAttrVo.CriticalStrike - criticalStrikeR;  //暴击比率（概率）暴击率
        if (Mathf.Abs((float)fCriticalStrike) < 0.0000001f)
            fCriticalStrike = 0;
        fMagicTimes = fightAttrVo.MagicTimes - magicTimesR;  // 技能伤害次数(倍数)
        fSkillDamage = fightAttrVo.SkillDamage - skillDamageR;  // 技能伤害比率
        fBossDamageAdd = fightAttrVo.BossDamageAdd - bossDamageAddR;
        fMonsterDamageAdd = fightAttrVo.MonsterDamageAdd - monsterDamageAddR;
        //新加
        fJoukRate = fightAttrVo.JoukRate - JoukRateR;    // 英雄 闪避率
        fAtkHPRecovery = fightAttrVo.AtkHPRecovery - AtkHPRecoveryR;
        fAtkMultiple = fightAttrVo.AtkMultiple - AtkMultipleR;  // 伤害倍率
        if (fAtkMultiple < 0)
        {
            fAtkMultiple = 0;
        }
        fIgnoreDef = fightAttrVo.IgnoreDef - IgnoreDefR;  //无视防御
        fPetAtk = fightAttrVo.PetAtk - PetAtkR;
        
        // fPhysicDef = fightAttrVo.PhysicDef - PhysicDefR;
        // fMagicDef = fightAttrVo.MagicDef - MagicDefR;
        // fSorceryDef = fightAttrVo.SorceryDef - SorceryDefR;
        orgPhysicDef = fightAttrVo.PhysicDef/ (1 + PhysicDefAdd) - PhysicDefR;
        fPhysicDef = orgPhysicDef * (1 + PhysicDefAdd - PhysicDefAddR);
        orgMagicDef = fightAttrVo.MagicDef/ (1 + MagicDefAdd) - MagicDefR;
        fMagicDef = orgMagicDef * (1 + MagicDefAdd - MagicDefAddR);
        orgSorceryDef = fightAttrVo.SorceryDef/ (1 + SorceryDefAdd) - SorceryDefR;
        fSorceryDef = orgSorceryDef * (1 + SorceryDefAdd - SorceryDefAddR);
        
        fParryRate = fightAttrVo.ParryRate - ParryRateR;  // 格挡率
        fParryValue = fightAttrVo.ParryValue - ParryValueR;  // 格挡值
        fBattleFinalAttack = fightAttrVo.BattleFinalAttack - BattleFinalAttackR;

        fTotalFight = GetTotalFight(fHp, fBloodsucking, fMitigation, fRecovery, fAtk, fCriticalStrike, fCriticalInjury, fSkillDamage, fMagicTimes,
            fBossDamageAdd, fMonsterDamageAdd, fAtkHPRecovery, fJoukRate, fAtkMultiple, fIgnoreDef, fBattleFinalAttack, fPetAtk, fPhysicDef,
            fMagicDef, fSorceryDef, fParryRate, fParryValue);
        
        //Debug.Log($"======****************=======fTotalFight==={fTotalFight}");
        // Debug.LogWarningFormat("hp={0}  atkValue={1}  Bloodsucking={2}  Mitigation={3}  Recovery={4} CriticalInjury={5} CriticalStrike={6} BossDamageAdd={7} MonsterDamageAdd={8} fTotalFight={9}",fHp,fAtk,fBloodsucking,fMitigation,fRecovery,fCriticalInjury,fCriticalStrike,fBossDamageAdd,fMonsterDamageAdd, fTotalFight);
        //第二步， 算出穿上那件装备的属性
        double oldFight = 0;
        if (isWear)
        {
            oldFight = GetTotalFight();
            Debug.Log("oldFight="+oldFight);
        }
        
        //第三步，算出穿上新装备的属性
        double nHp,
            nAtk,
            nHpAdd,
            nAtkAdd,
            nBloodsucking,
            nMitigation,
            nRecovery,
            nCriticalStrike,
            nCriticalInjury,
            nSkillDamage,
            nMagicTimes,
            nBossDamageAdd,
            nMonsterDamageAdd,
            
            nJoukRate,   //新加
            nAtkHPRecovery,
            nAtkMultiple,
            nIgnoreDef,
            nPetAtk,
            nPhysicDef,
            nMagicDef,
            nSorceryDef,
            nParryRate,
            nParryValue,
            nBattleFinalAttack;
        double nTotalFight = 0;
        
        nHpAdd = fHpAdd + hp_add;
        nHp = (orgHp + hp) * (1 + nHpAdd)* (1 + fightAttrVo.HPMultiple - HPMultipleR + HPMultiple);

        //nAtkAdd = fAtkAdd + atk_add;
        // nAtk = (orgAtk + atkValue + workAtk) * (1+nAtkAdd);
        nAtk = (orgAtk + atkValue + workAtk) * (1 + orgWorkAtkAdd - harmTypePhy_atkAddR + harmType_atkAdd) * (1 + orgVocationAtkAdd - vocation_atkAddR + vocation_atkAdd);

        nBloodsucking = fBloodsucking + bloodsucking;
        nMitigation = fMitigation + mitigation;
        nRecovery = fRecovery + recovery;
        nCriticalStrike = fCriticalStrike + criticalStrike;
        nCriticalInjury = fCriticalInjury + criticalInjury;
        nSkillDamage = fSkillDamage + skillDamage;
        nMagicTimes = fMagicTimes + magicTimes;
        nBossDamageAdd = fBossDamageAdd + bossDamageAdd;
        nMonsterDamageAdd = fMonsterDamageAdd + monsterDamageAdd;
        //新加
        nJoukRate = fJoukRate + JoukRate;   
        nAtkHPRecovery = fAtkHPRecovery + AtkHPRecovery;
        nAtkMultiple = fAtkMultiple + AtkMultiple;
        nIgnoreDef = fIgnoreDef + IgnoreDef;
        nPetAtk = fPetAtk + PetAtk;
        nPhysicDef = (fPhysicDef + PhysicDef) * (1 + PhysicDefAdd);
        nMagicDef = (fMagicDef + MagicDef) * (1+MagicDefAdd);
        nSorceryDef = (fSorceryDef + SorceryDef) * (1+SorceryDefAdd);
        nParryRate = fParryRate + ParryRate;
        nParryValue = fParryValue + ParryValue;
        nBattleFinalAttack = fBattleFinalAttack + BattleFinalAttack;

        nTotalFight = GetTotalFight(nHp, nBloodsucking, nMitigation, nRecovery, nAtk, nCriticalStrike, nCriticalInjury, nSkillDamage, nMagicTimes,
            nBossDamageAdd, nMonsterDamageAdd, nAtkHPRecovery, nJoukRate, nAtkMultiple, nIgnoreDef, nBattleFinalAttack, nPetAtk, nPhysicDef,
            nMagicDef, nSorceryDef, nParryRate, nParryValue);

        // Debug.LogWarningFormat("nhp={0}  nAtk={1}  nBloodsucking={2}  nMitigation={3}  nRecovery={4} nCriticalInjury={5} nCriticalStrike={6} nBossDamageAdd={7} nMonsterDamageAdd={8} nTotalFight={9}",nHp,nAtk,nBloodsucking,nMitigation,nRecovery,nCriticalInjury,nCriticalStrike,nBossDamageAdd,nMonsterDamageAdd, nTotalFight);

        if (isWear)
        {
            return Math.Ceiling(Math.Abs(fTotalFight - oldFight));
        }
        return Math.Ceiling(Math.Abs(nTotalFight - fTotalFight));
    }

    static Dictionary<int, int> magicTimeDict = new Dictionary<int, int>();
    private static List<EquipData> loreEquipList = new List<EquipData>();
    /// <summary>
    /// 抽卡技能关联符石技能的次数
    /// </summary>
    /// <returns></returns>
    public static int GetUseRuneSkillTimes()
    {
        loreEquipList.Clear();
        int maxMagicTimes = 0;
        loreEquipList = EquipManager.Instance.GetInstallLoreEquipsList();
        foreach (var equipData in loreEquipList)
        {
            if (equipData.skillMultiplesList != null && equipData.skillMultiplesList.Count > 0)
            {
                foreach (var skillMultipleData in equipData.skillMultiplesList)
                {
                    foreach (var rune in SkillInfoManager.Instance.GetBattleSkillList())
                    {
                        if (rune.SkillId == skillMultipleData.skillId)
                        {
                            maxMagicTimes += skillMultipleData.times;
                        }
                    }
                }
            }
        }
        return maxMagicTimes;
        /*
         magicTimeDict.Clear();
         foreach (var rune in SkillInfoManager.Instance.GetBattleSkillList())
         {
            //TODO 传承装备携带符石技能，符石技能关联 装备的抽卡技能？  
             int magicTimes = RuneInfoManager.Instance.GetSkillTimes(rune.SkillId);
             if (magicTimeDict.ContainsKey(rune.SkillId))
             {
                 magicTimeDict[rune.SkillId] += magicTimes;
             }
             else
             {
                 magicTimeDict.Add(rune.SkillId, magicTimes);
             }
         }
         int maxMagicTimes = 0;
         foreach (var item in magicTimeDict)
         {
             if (item.Value > maxMagicTimes)
                 maxMagicTimes = item.Value;
         }
         return maxMagicTimes;
         */
    }
    
    private static double GetRuneFight()
    {
        double total = 0;
        FightAttrVo fightAttrVo = DataManager.Instance.GetRoleData().FightAttrVo;
        // double atkFight = fightAttrVo.Atk * (1 + fightAttrVo.CriticalStrike) * fightAttrVo.CriticalInjury * (1 + fightAttrVo.SkillDamage) * (1 + fightAttrVo.MagicTimes) * (1 + (fightAttrVo.BossDamageAdd + fightAttrVo.MonsterDamageAdd) / 2f);
        double atkFight = fightAttrVo.Atk * (1 + fightAttrVo.AtkMultiple) * (1 + (float)fightAttrVo.CriticalStrike) * fightAttrVo.CriticalInjury *
            (1 + fightAttrVo.SkillDamage) * (1 + useRuneSkillNum) * (1 + (fightAttrVo.BossDamageAdd + fightAttrVo.MonsterDamageAdd) / 2f) *
            (1 + fightAttrVo.IgnoreDef) * fightAttrVo.BattleFinalAttack + fightAttrVo.PetAtk;
        
        total = atkFight* GetUseRuneSkillTimes();
        
        return total;
    }


    public static double GetTotalFight()
    {
        double total = 0;
        total += GetHeroFight(); //+ GetRuneFight();
        return total;
    }

    //主线任务类型
    // enum eMainTaskType
    // {
    //     eMainTaskType_PassStage	= 1;	//  通关，
    //     eMainTaskType_KillMonsters	= 2;	// 击杀怪物数，
    //     eMainTaskType_GenEquipByBox	= 3;	// 开宝箱产生装备，
    //     eMainTaskType_HeroLevel	= 4;	// 英雄等级，
    //     eMainTaskType_PetLotteryTimes	= 5;	// 宠物抽奖，
    //     eMainTaskType_PetLevelUp	= 6;	// 强化宠物，，
    //     eMainTaskType_SellEquips	= 7;	// 出售装备，
    //     eMainTaskType_EquipBoxLevelUp	= 8;	// 箱子升级
    //     eMainTaskType_Max	= 9;	// -1种类数
    // }

    // 修改后的每日任务跳转
    public static void SkipToCompleteTask(int taskType)
    {
        switch ((TaskType) taskType)
        {
            // case TaskType.eMainTaskType_PetExtract:
            //     LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
            //     lobbyView?.OpenBottomPanel(4, 0);
            //     break;
            // case TaskType.eMainTaskType_SkillExtract:
            //     lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
            //     lobbyView?.OpenBottomPanel(4, 0);
            //     break;
            // case TaskType.eMainTaskType_EquipMake:
            //     lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
            //     lobbyView?.OpenBottomPanelToEquipMake();
            //     break;
            // case TaskType.eMainTaskType_HeroCPUp:
            // case TaskType.eMainTaskType_LifeLvUp:
            // case TaskType.eMainTaskType_AttackLvUp:
            // case TaskType.eMainTaskType_RecoverLvUp:
            // case TaskType.eMainTaskType_CriticalHitLvUp:
            // case TaskType.eMainTaskType_BlastInjuryLvUp:
            // case TaskType.eMainTaskType_GetQualityEquip:
            //     lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
            //     lobbyView?.OpenBottomPanel(3, 0);
            //     break;
            // case TaskType.eMainTaskType_PassStage:
            // case TaskType.eMainTaskType_HeroExtract:
            //     lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
            //     lobbyView?.OpenBottomPanel(4, 0);
            //     break;
            // case TaskType.eMainTaskType_KillMonsters:
            // case TaskType.eMainTaskType_OnlineTime:
            // case TaskType.eMainTaskType_MakeLvUp:
            //     UIManager.Instance.ShowUIPanel("TreasureLevelup");
            //     break;
        }
    }
    
    /// <summary>
    /// 英雄属性
    /// </summary>
    public static FightAttrVo GetFightAttrVo(IList<msg.BattleAttr> battleAttrs,FightAttrVo fightAttrVo)
    {
        // FightAttrVo fightAttrVo = new FightAttrVo();
        foreach (var item in battleAttrs)
        {
            double attrValue = double.Parse(item.AttrValue.ToString("f4"));
            SetBattleAttr(fightAttrVo, item.AttrId, attrValue);
        }
        
        //区分职业
        HeroInfo myHero = HeroInfoManager.Instance.GetMyHero();
        // if (myHero.HeroUnit.HarmType == (int)EN_HARM_TYPE.PhysicAtk)
        // {
        //     fightAttrVo.HarmType = (int)EN_HARM_TYPE.PhysicAtk;
        // }else if (myHero.HeroUnit.HarmType == (int)EN_HARM_TYPE.MagicAtk)
        // {
        //     fightAttrVo.HarmType = (int)EN_HARM_TYPE.MagicAtk;
        // }else if (myHero.HeroUnit.HarmType == (int)EN_HARM_TYPE.SorceryAtk)
        // {
        //     fightAttrVo.HarmType = (int)EN_HARM_TYPE.SorceryAtk;
        // }
        fightAttrVo.HarmType = myHero.HeroUnit.HarmType;
        fightAttrVo.HarmAttrAtkAdd = myHero.HeroUnit.VocationAttr;
        
        return fightAttrVo;
    }
    
    public static double atkNum = 0;
    /// <summary>
    /// 宠物属性
    /// </summary>
    public static FightAttrVo GetPetFightAttrVo(Dictionary<int,Engine.PetBattleAttr> battleAttrs)
    {
        FightAttrVo fightAttrVo = new FightAttrVo();
        foreach (var item in battleAttrs)
        {
            double attrValue = double.Parse(item.Value.AttrVal.ToString("f4"));
            SetBattleAttr(fightAttrVo, (msg.eBattleAttr)item.Value.AttrId, attrValue);
        }

        atkNum = 0;
        //区分职业
        if (fightAttrVo.PhysicAtk > 0)
        {
            atkNum = fightAttrVo.PhysicAtk;
            fightAttrVo.HarmType = (int)EN_HARM_TYPE.PhysicAtk;
        }
        if (fightAttrVo.MagicAtk > 0 && fightAttrVo.MagicAtk > atkNum)
        {
            atkNum = fightAttrVo.MagicAtk;
            fightAttrVo.HarmType = (int)EN_HARM_TYPE.MagicAtk;
        }
        if (fightAttrVo.SorceryAtk > 0 && fightAttrVo.SorceryAtk > atkNum)
        {
            atkNum = fightAttrVo.SorceryAtk;
            fightAttrVo.HarmType = (int)EN_HARM_TYPE.SorceryAtk;
        }
        fightAttrVo.Atk += atkNum;
        return fightAttrVo;
    }

    public static void SetBattleAttr(FightAttrVo fightAttrVo, eBattleAttr AttrId, double attrValue)
    {
        switch (AttrId)
        {
            // case msg.eBattleAttr.eBattleAttr_Extra_Atk_Rate:  //攻击加成比率  细分用其它的
            //     fightAttrVo.ATKADD = (double) attrValue * ConstDefine.CONFIG_PLACE_EX;
            //     break;
            case msg.eBattleAttr.eBattleAttr_Begin:  // 0 开始
                fightAttrVo.Begin = Math.Ceiling(attrValue);
                break;
            //以下三种伤害客户端无视，需取服务器端下发的eBattleAttr_FinalAttack = 101 用作Attack伤害  
            case msg.eBattleAttr.eBattleAttr_PhysicAttack:  // 1 物理伤害
                fightAttrVo.PhysicAtk = Math.Ceiling(attrValue);
                break;
            case msg.eBattleAttr.eBattleAttr_MagicAttack:  // 2 魔法伤害
                fightAttrVo.MagicAtk = Math.Ceiling(attrValue);
                break;
            case msg.eBattleAttr.eBattleAttr_SorceryAttack:  // 3 道术伤害
                fightAttrVo.SorceryAtk = Math.Ceiling(attrValue);
                break;
            case msg.eBattleAttr.eBattleAttr_HP:       // 4 生命
                fightAttrVo.HP = Math.Ceiling(attrValue);
                break;
            //以下三种伤害客户端无视，需取服务器端下发的eBattleAttr_FinalDefence = 102 用作防御值 
            case msg.eBattleAttr.eBattleAttr_PhysicDefence:  // 5 物理防御
                fightAttrVo.PhysicDef = Math.Ceiling(attrValue);
                break;
            case msg.eBattleAttr.eBattleAttr_MagicDefence:  // 6 魔法防御
                fightAttrVo.MagicDef = Math.Ceiling(attrValue);
                break;
            case msg.eBattleAttr.eBattleAttr_SorceryDefence:  // 7 道术防御
                fightAttrVo.SorceryDef = Math.Ceiling(attrValue);
                break;
            case msg.eBattleAttr.eBattleAttr_HP_Recovery:  // 8 生命恢复
                fightAttrVo.Recovery = attrValue;
                break;
            case msg.eBattleAttr.eBattleAttr_PetAtk:  // 9 附加的宠物伤害（角色各功能加给宠物身上生效的伤害）
                fightAttrVo.PetAtk = Math.Ceiling(attrValue);
                break;
            case msg.eBattleAttr.eBattleAttr_Extra_PhysicAttack:  // 10 物理伤害加成
                fightAttrVo.PhysicAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Extra_MagicAttack:  // 11 魔法伤害加成
                fightAttrVo.MagicAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Extra_SorceryAttack:  // 12 道术伤害加成
                fightAttrVo.SorceryAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Extra_HP_Rate:     // 13 生命加成比率
                fightAttrVo.HPADD = (double)attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Extra_PhysicDefence:  // 14 物理防御加成
                fightAttrVo.PhysicDefADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Extra_MagicDefence:  // 15 魔法防御加成
                fightAttrVo.MagicDefADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Extra_SorceryDefence:  // 16 道术防御加成
                fightAttrVo.SorceryDefADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Extra_EarthAtk:  // 17 地系伤害加成
                fightAttrVo.EarthAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Extra_WaterAtk:  // 18 水系伤害加成
                fightAttrVo.WaterAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Extra_FireAtk:  // 19 火系伤害加成
                fightAttrVo.FireAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Extra_AirAtk:  // 20 气系伤害加成
                fightAttrVo.AirAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Extra_PetAtk:  // 21 // 宠物伤害加成（角色各功能加给宠物身上生效的伤害）
                fightAttrVo.PetAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Multiple_HP_Rate:  // 22 生命倍率
                fightAttrVo.HPMultiple = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Multiple_Atk_Rate:  // 23 伤害倍率
                fightAttrVo.AtkMultiple = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Jouk_Rate:  // 24 闪避率
                fightAttrVo.JoukRate = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_AtkHit_Rate:  // 25 普通攻击命中率
                fightAttrVo.AtkHitRate = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Parry_Rate:  // 26 格挡率
                fightAttrVo.ParryRate = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Parry_Value:  // 27 格挡值
                fightAttrVo.ParryValue = Math.Ceiling(attrValue);
                break;
            case msg.eBattleAttr.eBattleAttr_Ignore_Defence_Rate:  // 28 无视防御
                fightAttrVo.IgnoreDef = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_CriticalStrike_Rate:  // 29 暴击比率
                fightAttrVo.CriticalStrike =  attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_CriticalInjury_Rate:    // 30 爆伤比率
                fightAttrVo.CriticalInjury = attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Extra_InjuryBoss_Rate:   // 31 BOSS伤害加成比率
                fightAttrVo.BossDamageAdd =  (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Extra_InjuryMonster_Rate:   // 32 小怪伤害加成比率
                fightAttrVo.MonsterDamageAdd =  (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_ReduceHurt_Rate:    // 33 减伤比率
                fightAttrVo.Mitigation =  (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_AbsorptionEnemyHP_Rate:   // 34 吸血比率
                fightAttrVo.Bloodsucking =  (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_Hurt_HPRecovery:   // 35 普攻回复的生命具体值
                fightAttrVo.AtkHPRecovery =  (float) attrValue;
                break;
            case msg.eBattleAttr.eBattleAttr_SkillAtk_Rate:    // 36 技能伤害比率
                fightAttrVo.SkillDamage = attrValue * ConstDefine.CONFIG_PLACE_EX;;
                break;
            case msg.eBattleAttr.eBattleAttr_SkillAtkMultiple_Rate:   // 37 技能伤害次数(倍数)加成比率
                fightAttrVo.MagicTimesAdd = (int) attrValue * ConstDefine.CONFIG_PLACE_EX;;
                break;
            case msg.eBattleAttr.eBattleAttr_SkillAtkMultiple:    // 38 技能伤害次数(倍数)
                fightAttrVo.MagicTimes = (float) attrValue;
                break;
            case msg.eBattleAttr.eBattleAttr_DropGold_Rate:    // 39 掉落金币加成比率
                fightAttrVo.GoldAdd = (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_SkillCD_Rate:    // 40 技能冷却比率
                fightAttrVo.SkillCd =  (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_AtkSpeed_Rate:   // 41 攻速比率
                fightAttrVo.AtkSpeed =  (float)attrValue*ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_ComboAtk_Rate:   // 42 连击比率
                fightAttrVo.ComboAtk =  (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_CounterAtk_Rate:   // 43 反击比率
                fightAttrVo.CounterAtk =  (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_RareLoreEquipGen_Rate:    // 44 稀有传承概率
                fightAttrVo.LoreEquipRate =  (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_DropLoreEquip_Rate:   // 45 传承掉落概率
                fightAttrVo.DropLoreEquipRate =  (float)attrValue*ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_OnlineStageAwaardMultiple:   // 46 关卡挂机奖励次数
                fightAttrVo.OnlineAwardTimes =  (float)attrValue;
                break;
            case msg.eBattleAttr.eBattleAttr_HomeTownProduce_ExtraLimit:   // 47 家园挂机时间上限
                fightAttrVo.HomeLimitMaxTime =  (float)attrValue;
                break;
            case msg.eBattleAttr.eBattleAttr_BattleFinalAttack:   // 48 战斗最终伤害 ---- 还未投放，暂时为0
                fightAttrVo.BattleFinalAttack =  (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case msg.eBattleAttr.eBattleAttr_KilledRecovery:   // 49 消灭对象后HP回复
                fightAttrVo.KilledRecovery =  (float)attrValue;
                break;
            case msg.eBattleAttr.eBattleAttr_FinalAttack:  // 101 攻击
                fightAttrVo.Atk = Math.Ceiling(attrValue);
                break;
            case msg.eBattleAttr.eBattleAttr_FinalDefence:  // 102 防御
                fightAttrVo.Def = Math.Ceiling(attrValue);
                break;
        }
    }
    
    /// <summary>
    /// 怪物属性设置   加减 attrValue
    /// </summary>
    public static void SetMonsterAttrBySkillAchieve(MapObjectAttr attribute,int attrId, double attrValue)
    {
        switch (attrId)
        {
            //以下三种伤害客户端无视，需取服务器端下发的eBattleAttr_FinalAttack = 101 用作Attack伤害  
            // case (int)EN_BUFF_ADD_TYPE.PhysicAtk:  // 1 物理伤害
            //     attribute.PhysicAtk += Math.Ceiling(attrValue);
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.MagicAtk:  // 2 魔法伤害
            //     attribute.MagicAtk += Math.Ceiling(attrValue);
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.SorceryAtk:  // 3 道术伤害
            //     attribute.SorceryAtk += Math.Ceiling(attrValue);
            //     break;
            case (int)EN_BUFF_ADD_TYPE.HP:       // 4 生命
                attribute.HP += Math.Ceiling(attrValue);;
                break;
            //以下三种伤害客户端无视，需取服务器端下发的eBattleAttr_FinalDefence = 102 用作防御值 
            // case (int)EN_BUFF_ADD_TYPE.PhysicDef:  // 5 物理防御
            //     attribute.PhysicDef += Math.Ceiling(attrValue);
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.MagicDef:  // 6 魔法防御
            //     attribute.MagicDef += Math.Ceiling(attrValue);
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.SorceryDef:  // 7 道术防御
            //     attribute.SorceryDef += Math.Ceiling(attrValue);
            //     break;
            case (int)EN_BUFF_ADD_TYPE.Recovery:  // 8 生命恢复
                attribute.Recovery += attrValue;
                break;
            // case (int)EN_BUFF_ADD_TYPE.PetAtkADD:  // 9 附加的宠物伤害（角色各功能加给宠物身上生效的伤害）
            //     attribute.PetAtkADD += Math.Ceiling(attrValue);
            //     break;
            /**/
            // case (int)EN_BUFF_ADD_TYPE.PhysicAtkADD:  // 10 物理伤害加成
            //     attribute.PhysicAtkADD += attrValue * ConstDefine.CONFIG_PLACE_EX;
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.MagicAtkADD:  // 11 魔法伤害加成
            //     attribute.MagicAtkADD += attrValue * ConstDefine.CONFIG_PLACE_EX;
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.SorceryAtkADD:  // 12 道术伤害加成
            //     attribute.SorceryAtkADD += attrValue * ConstDefine.CONFIG_PLACE_EX;
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.HPADD:     // 13 生命加成比率
            //     attribute.HPADD += (double)attrValue * ConstDefine.CONFIG_PLACE_EX;
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.PhysicDefADD:  // 14 物理防御加成
            //     attribute.PhysicDefADD += attrValue * ConstDefine.CONFIG_PLACE_EX;
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.MagicDefADD:  // 15 魔法防御加成
            //     attribute.MagicDefADD += attrValue * ConstDefine.CONFIG_PLACE_EX;
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.SorceryDefADD:  // 16 道术防御加成
            //     attribute.SorceryDefADD += attrValue * ConstDefine.CONFIG_PLACE_EX;
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.EarthAtkADD:  // 17 地系伤害加成
            //     attribute.EarthAtkADD += attrValue * ConstDefine.CONFIG_PLACE_EX;
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.WaterAtkADD:  // 18 水系伤害加成
            //     attribute.WaterAtkADD += attrValue * ConstDefine.CONFIG_PLACE_EX;
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.FireAtkADD:  // 19 火系伤害加成
            //     attribute.FireAtkADD += attrValue * ConstDefine.CONFIG_PLACE_EX;
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.AirAtkADD:  // 20 气系伤害加成
            //     attribute.AirAtkADD += attrValue * ConstDefine.CONFIG_PLACE_EX;
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.PetAtkADD:  // 21 附加的宠物伤害（角色各功能加给宠物身上生效的伤害）
            //     attribute.PetAtkADD = Math.Ceiling(attrValue);
            //     break;
            case (int)EN_BUFF_ADD_TYPE.HPMultiple:  // 22 生命倍率
                attribute.HPMultiple += attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.AtkMultiple:  // 23 伤害倍率
                attribute.AtkMultiple += attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.JoukRate:  // 24 闪避率
                attribute.JoukRate += attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.AtkHitRate:  // 25 普通攻击命中率
                attribute.AtkHitRate += attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.ParryRate:  // 26 格挡率
                attribute.ParryRate += attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.ParryValue:  // 27 格挡值
                attribute.ParryValue += Math.Ceiling(attrValue);
                break;
            case (int)EN_BUFF_ADD_TYPE.IgnoreDef:  // 28 无视防御
                attribute.IgnoreDef += attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.CriticalStrike:  // 29 暴击比率
                attribute.CriticalStrike +=  attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.CriticalInjury:    // 30 爆伤比率
                attribute.CriticalInjury += attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.BossDamageAdd:   // 31 BOSS伤害加成比率
                attribute.BossDamageAdd += (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.MonsterDamageAdd:   // 32 小怪伤害加成比率
                attribute.MonsterDamageAdd += (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.Mitigation:    // 33 减伤比率
                attribute.Mitigation += (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.Bloodsucking:   // 34 吸血比率
                attribute.Bloodsucking += (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            // case (int)EN_BUFF_ADD_TYPE.AtkHPRecovery:   // 35 普攻回复的生命具体值
            //     attribute.AtkHPRecovery +=  (float) attrValue;
            //     break;
            case (int)EN_BUFF_ADD_TYPE.SkillDamage:    // 36 技能伤害比率
                attribute.SkillDamage += attrValue * ConstDefine.CONFIG_PLACE_EX;;
                break;
            case (int)EN_BUFF_ADD_TYPE.MagicTimesAdd:   // 37 技能伤害次数(倍数)加成比率
                attribute.MagicTimesAdd += (int) attrValue * ConstDefine.CONFIG_PLACE_EX;;
                break;
            case (int)EN_BUFF_ADD_TYPE.MagicTimes:    // 38 技能伤害次数(倍数)
                attribute.MagicTimes += (float) attrValue;
                break;
            case (int)EN_BUFF_ADD_TYPE.GoldAdd:    // 39 掉落金币加成比率
                attribute.GoldAdd += (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.SkillCd:    // 40 技能冷却比率
                attribute.SkillCd += (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.AtkSpeed:   // 41 攻速比率
                attribute.AtkSpeed += (float)attrValue*ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.ComboAtk:   // 42 连击比率
                attribute.ComboAtk += (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.CounterAtk:   // 43 反击比率
                attribute.CounterAtk += (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            // case (int)EN_BUFF_ADD_TYPE.LoreEquipRate:    // 44 稀有传承概率
            //     attribute.LoreEquipRate += (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.DropLoreEquipRate:   // 45 传承掉落概率
            //     attribute.DropLoreEquipRate += (float)attrValue*ConstDefine.CONFIG_PLACE_EX;
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.OnlineAwardTimes:   // 46 关卡挂机奖励次数
            //     attribute.OnlineAwardTimes += (float)attrValue;
            //     break;
            // case (int)EN_BUFF_ADD_TYPE.HomeLimitMaxTime:   // 47 家园挂机时间上限
            //     attribute.HomeLimitMaxTime += (float)attrValue;
            //     break;
            case (int)EN_BUFF_ADD_TYPE.BattleFinalAttack:   // 48 战斗最终伤害 ---- 还未投放，暂时为0
                attribute.BattleFinalAttack += (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                break;
            // case (int)EN_BUFF_ADD_TYPE.KilledRecovery:   // 49 消灭对象后HP回复
            //     attribute.KilledRecovery +=  (float)attrValue;
            //     break;
            case (int)EN_BUFF_ADD_TYPE.ATK:  // 101 攻击
                attribute.Atk += attribute.Atk * Math.Ceiling(attrValue) * ConstDefine.CONFIG_PLACE_EX;
                break;
            case (int)EN_BUFF_ADD_TYPE.Def:  // 102 防御
                attribute.Def += attribute.Def * Math.Ceiling(attrValue) * ConstDefine.CONFIG_PLACE_EX;
                break;
        }
    }
}
