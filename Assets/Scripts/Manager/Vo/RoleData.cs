using System.Collections.Generic;
using Config;
using UnityEngine;

namespace Engine
{
    public class FightAttrVo
    {
        public FightAttrVo DeepCopy()
        {
            return new FightAttrVo()
            {
                Begin = this.Begin,
                // ATKADD = this.ATKADD,
                Atk = this.Atk,
                Def = this.Def,
                
                PhysicAtk = this.PhysicAtk,
                MagicAtk = this.MagicAtk,
                SorceryAtk = this.SorceryAtk,
                HP = this.HP,
                PhysicDef = this.PhysicDef,
                MagicDef = this.MagicDef,
                SorceryDef = this.SorceryDef,
                Recovery = this.Recovery,
                PetAtk = this.PetAtk,
                PhysicAtkADD = this.PhysicAtkADD,
                MagicAtkADD = this.MagicAtkADD,
                SorceryAtkADD = this.SorceryAtkADD,
                HPADD = this.HPADD,
                PhysicDefADD = this.PhysicDefADD,
                MagicDefADD = this.MagicDefADD,
                SorceryDefADD = this.SorceryDefADD,
                EarthAtkADD = this.EarthAtkADD,
                WaterAtkADD = this.WaterAtkADD,
                FireAtkADD = this.FireAtkADD,
                AirAtkADD = this.AirAtkADD,
                PetAtkADD = this.PetAtkADD,
                HPMultiple = this.HPMultiple,
                AtkMultiple = this.AtkMultiple,
                JoukRate = this.JoukRate,
                AtkHitRate = this.AtkHitRate,
                ParryRate = this.ParryRate,
                ParryValue = this.ParryValue,
                IgnoreDef = this.IgnoreDef,
                CriticalStrike = this.CriticalStrike,
                CriticalInjury = this.CriticalInjury,
                BossDamageAdd = this.BossDamageAdd,
                MonsterDamageAdd = this.MonsterDamageAdd,
                Mitigation = this.Mitigation,
                Bloodsucking = this.Bloodsucking,
                AtkHPRecovery = this.AtkHPRecovery,
                SkillDamage = this.SkillDamage,
                MagicTimes = this.MagicTimes,
                MagicTimesAdd = this.MagicTimesAdd,
                GoldAdd = this.GoldAdd,
                SkillCd = this.SkillCd,
                AtkSpeed = this.AtkSpeed,
                ComboAtk = this.ComboAtk,
                CounterAtk = this.CounterAtk,
                LoreEquipRate = this.LoreEquipRate,
                DropLoreEquipRate  = this.DropLoreEquipRate,
                OnlineAwardTimes = OnlineAwardTimes,
                HomeLimitMaxTime = this.HomeLimitMaxTime,
                BattleFinalAttack = this.BattleFinalAttack,
                KilledRecovery = this.KilledRecovery,
            };
        }

        #region 属性
        
        /// <summary>
        /// 初始值
        /// </summary>
        public double Begin{ get; set; }
        
        /// <summary>
        /// 攻击  总攻击力  eBattleAttr_FinalAttack = 101 用作Attack伤害  
        /// </summary>
        public double Atk { get; set; }
        // public double ATKADD { get; set; }  //新的不用这个  有需要找服务器 再加一个类似  攻击力 的加成值
        
        /// <summary>
        /// 防御值  总防御力   需取服务器端下发的eBattleAttr_FinalDefence = 102 用作防御值
        /// </summary>
        public double Def { get; set; }
        
        //以下三种伤害客户端无视，需取服务器端下发的eBattleAttr_FinalAttack = 101 用作Attack伤害  
        /// <summary>
        /// 物理伤害
        /// </summary>
        public double PhysicAtk { get; set; }
        /// <summary>
        /// 魔法伤害
        /// </summary>
        public double MagicAtk { get; set; }
        /// <summary>
        /// 道术伤害
        /// </summary>
        public double SorceryAtk { get; set; }
        
        /// <summary>
        /// 生命
        /// </summary>
        public double HP { get; set; }//原有的
        
        /// <summary>
        /// 物理防御   以下三种伤害客户端无视，需取服务器端下发的eBattleAttr_FinalDefence = 102 用作防御值
        /// </summary>
        public double PhysicDef { get; set; }
        /// <summary>
        /// 魔法防御
        /// </summary>
        public double MagicDef { get; set; }
        /// <summary>
        /// 道术防御
        /// </summary>
        public double SorceryDef { get; set; }
        
        /// <summary>
        /// 生命恢复
        /// </summary>
        public double Recovery { get; set; }//原有的
        
        /// <summary>
        /// 附加的宠物伤害（宠物会获得的额外普通攻击伤害）  0的时候不生效，
        /// </summary>
        public double PetAtk { get; set; }
        
        /// <summary>
        /// 物理伤害加成
        /// </summary>
        public double PhysicAtkADD { get; set; }
        /// <summary>
        /// 魔法伤害加成
        /// </summary>
        public double MagicAtkADD { get; set; }
        /// <summary>
        /// 道术伤害加成
        /// </summary>
        public double SorceryAtkADD { get; set; }
        
        /// <summary>
        /// 生命加成比率
        /// </summary>
        public double HPADD { get; set; }//原有的
        
        /// <summary>
        /// 物理防御加成
        /// </summary>
        public double PhysicDefADD { get; set; }
        /// <summary>
        /// 魔法防御加成
        /// </summary>
        public double MagicDefADD { get; set; }
        /// <summary>
        /// 道术防御加成
        /// </summary>
        public double SorceryDefADD { get; set; }
        
        /// <summary>
        /// 地系伤害加成
        /// </summary>
        public double EarthAtkADD { get; set; }
        /// <summary>
        /// 水系伤害加成
        /// </summary>
        public double WaterAtkADD { get; set; }
        /// <summary>
        /// 火系伤害加成
        /// </summary>
        public double FireAtkADD { get; set; }
        /// <summary>
        /// 气系伤害加成
        /// </summary>
        public double AirAtkADD { get; set; }
        /// <summary>
        /// 附加的宠物伤害加成（宠物会获得的额外普通攻击伤害的加成万分比）   PetAtk 附加的宠物伤害  为0的时候不生效，
        /// </summary>
        public double PetAtkADD { get; set; }
        
        /// <summary>
        /// 生命倍率
        /// </summary>
        public double HPMultiple { get; set; }
        /// <summary>
        /// 伤害倍率
        /// </summary>
        public double AtkMultiple { get; set; }
        
        /// <summary>
        /// 英雄 闪避率
        /// </summary>
        public double JoukRate { get; set; }
        /// <summary>
        /// 英雄 普通攻击命中率
        /// </summary>
        public double AtkHitRate { get; set; }
        /// <summary>
        /// 格挡率
        /// </summary>
        public double ParryRate { get; set; }
        /// <summary>
        /// 格挡值
        /// </summary>
        public double ParryValue { get; set; }
        /// <summary>
        /// 无视防御   英雄无视防御百分比
        /// </summary>
        public double IgnoreDef { get; set; }
        
        /// <summary>
        /// 暴击比率（概率）暴击率
        /// </summary>
        public double CriticalStrike { get; set; }  //原有的
        /// <summary>
        /// 爆伤比率（爆伤的伤害倍数）  暴击伤害百分比
        /// </summary>
        public double CriticalInjury { get; set; }//原有的
        /// <summary>
        /// BOSS伤害加成比率
        /// </summary>
        public float BossDamageAdd { get; set; }//原有的
        /// <summary>
        /// 小怪伤害加成比率
        /// </summary>
        public float MonsterDamageAdd { get; set; }//原有的
        /// <summary>
        /// 减伤比率
        /// </summary>
        public float Mitigation { get; set; }//原有的
        /// <summary>
        /// 吸血比率
        /// </summary>
        public float Bloodsucking { get; set; }//原有的
        
        /// <summary>
        /// 攻击回复   普攻回复的生命具体值
        /// </summary>
        public float AtkHPRecovery { get; set; }
        
        /// <summary>
        /// 技能伤害比率
        /// </summary>
        public double SkillDamage { get; set; }//原有的
        
        /// <summary>
        /// 技能伤害次数(倍数)加成比率 客户端在技能生效且 eBattleAttr_SkillAtkMultiple 匹配时，才有用   （改成直接读取传承装备关联的技能里面配的次数）
        /// </summary>
        public float MagicTimesAdd { get; set; }//原有的
        /// <summary>
        /// 技能伤害次数(倍数),服务器端固定为0下发 （针对所有技能攻击加成服务器已经一起算到攻击力里面，这里只给属性面板显示用） 客户端需要根据装备的传承装备获取生效的技能ID与上阵的技能ID匹配，且技能触发时这个倍数才生效
        /// </summary>
        public float MagicTimes { get; set; }//原有的
        /// <summary>
        /// 金币加成比率
        /// </summary>
        public float GoldAdd { get; set; }//原有的
        /// <summary>
        /// 技能冷却比率
        /// </summary>
        public float SkillCd;//原有的
        /// <summary>
        /// 攻速比率
        /// </summary>
        public float AtkSpeed {get;set; }//原有的

        /// <summary>
        /// 连击比率
        /// </summary>
        public float ComboAtk { get; set; }//原有的

        /// <summary>
        /// 反击比率
        /// </summary>
        public float CounterAtk { get; set; }//原有的
        
        /// <summary>
        /// 稀有传承概率
        /// </summary>
        public float LoreEquipRate { get; set; }
        /// <summary>
        /// 传承掉落概率
        /// </summary>
        public float DropLoreEquipRate { get; set; }
        
        /// <summary>
        /// 关卡挂机奖励次数
        /// </summary>
        public float OnlineAwardTimes { get; set; }
        
        /// <summary>
        /// 家园挂机时间上限
        /// </summary>
        public float HomeLimitMaxTime { get; set; }
        
        /// <summary>
        /// 战斗最终伤害 ---- 还未投放，暂时为0
        /// </summary>
        public float BattleFinalAttack { get; set; }
        
        /// <summary>
        /// 消灭对象后HP回复
        /// </summary>
        public float KilledRecovery { get; set; }
        
        /// <summary>
        /// 角色伤害类型
        /// </summary>
        public int HarmType { get; set; }
        
        /// <summary>
        /// 角色 水、火、地风系伤害加成
        /// </summary>
        public int HarmAttrAtkAdd { get; set; }
        #endregion
    }
    public class RoleData
    {
        public string userID;
        public string userName;
        public double gold;
        public long dia;
        public long dia2;
        public long exp;
        public double fight => FightUtils.GetHeroFight();
        
        public int talentPoints;//天赋点
        public int shield;//盾牌
        public int emblem;//勋章
        public int rune;//符石
        public int sigil;//职印

        public int lv;
        public int vipLv;
        public int skillLevel;
        public int vipExp;
        public int avatarID;
        public int chapterId;       // 最后选择挑战关卡所在章节 ---
        public int stageId;         // 最后选择挑战的关卡 ---
        public int subStageId;     // 最后挑战关卡时失败的节点 --有记录，挂机状态时客户端用
        public int latestPassedStageId;  //当前通过的最后关卡
        public int battleStatus;   // 关卡状态 --参见 eBattleStatus 枚举 为0表示正常战斗过关中，为1表示失败、胜利后进入循环 -1 没有战斗  2--最大值
        public int heroId;
        public int petLotteryLv;
        public int petLotteryExp;
        public int equipBoxLv;
        public int equipBoxExp;
        public ulong equipBoxLvUpTime;
        public ulong runeRecycleNum;
        //在线累计时长
        public ulong OnlineAwardCdTime;
        public ulong OfflineTotalSecond;
        public OfflineRewardData OfflineRewardData;
        
        private Dictionary<int, int> _dungeonStageDict = new Dictionary<int, int>();

        public uint ChangeNameCounter;
        
        private Dictionary<int, int> _stageMaxDict = new Dictionary<int, int>();

        public FightAttrVo FightAttrVo = new FightAttrVo();
        
        public int SkillLotteryLv;
        public int SkillLotteryExp;
        public int HeroFreeLottery;

        public bool IsFirstCharge;

        /// <summary>
        /// 本章节解锁新关卡id，为0表示没有
        /// </summary>
        public int lockStageId;
        
        /// <summary>
        /// 今日完成随机任务数
        /// </summary>
        public int DoneEventTaskNum;

        /// <summary>
        /// 最后所在地图对应的章节ID
        /// </summary>
        public int lastStayMapChapterId;
        
        /// <summary>
        /// npc任务积分 
        /// </summary>
        public int npcTaskPoints;

        public RoleData()
        {
            userID = "";
            userName = "默认名字娃哈哈";
            gold = 0;
            dia = 0;
            dia2 = 0;
            talentPoints = 0;
            lv = 1;
            vipLv = 0;
            skillLevel = 1;
            avatarID = 0;
            stageId = 1001;
            chapterId = 1000;
            heroId = HeroInfoManager.Instance.GetHeroFirstID();
            _dungeonStageDict.Add((int)DungeonType.Diamond, 1);
            _dungeonStageDict.Add((int)DungeonType.Gold, 1);
            _dungeonStageDict.Add((int)DungeonType.Zhuzhao, 1);
            _dungeonStageDict.Add((int)DungeonType.Exp, 1);
            _dungeonStageDict.Add((int)DungeonType.PetMaterial, 1);
            _dungeonStageDict.Add((int)DungeonType.PetSkillBook, 1);
            _dungeonStageDict.Add((int)DungeonType.GodEquip, 1);
            _dungeonStageDict.Add((int)DungeonType.Holy, 1);
            DoneEventTaskNum = 0;
            shield = 0;
            emblem = 0;
            rune = 0;
            sigil = 0;
            npcTaskPoints = 0;
        }

        // public string GetAvatarUrl()
        // {
        //     return "ui://Common/tx";
        // }
        public string GetAvatarUrl()
        {
            avatarID = DataManager.Instance.mRoleData.avatarID;
            if (avatarID == 0)
            {
                return "1030011";// 宝藏猎人头像
            }
            else
            {
                ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(avatarID);
                return itemTypeUnit.Icon;
            }
        }

        public int GetDungeonStageId(DungeonType type)
        {
            if (_dungeonStageDict.TryGetValue((int) type, out int dungeonStageId))
            {
                return dungeonStageId;
            }

            return 1;
        }

        public void SetDungeonStage(int type, int stageId)
        {
            if (!_stageMaxDict.TryGetValue(type, out int maxLv))
            {
                maxLv = ConfigUtils.GetDungeonStageMaxByType(type);
                _stageMaxDict.Add(type, maxLv);
            }
            if (_dungeonStageDict.TryGetValue(type, out int curStageId))
            {
                if (curStageId < (stageId+1))
                {
                    _dungeonStageDict[type] = Mathf.Min(maxLv, stageId+1);
                }
            }
            else
            {
                _dungeonStageDict.Add(type, Mathf.Min(maxLv, stageId));
            }
        }
    }
}