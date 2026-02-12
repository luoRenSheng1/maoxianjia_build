using BestHTTP.Extensions;
using Common;
using Config;
using EngineBase;
using FairyGUI;
using Spine.Unity;
using System;
using System.Collections.Generic;
using Vector2 = UnityEngine.Vector2;

namespace Engine
{
    public enum PushType
    {
        FunctionPush = 1,//功能类推送
        PlayingMethodPush = 2,//玩法类推送
        RewardPush = 3,//奖励类推送
    }

    /// <summary>
    /// 推送提示
    /// </summary>
    public class PushManager : TSingleton<PushManager>
    {
        /// <summary>
        /// 小窗口
        /// </summary>
        private UI_PushTips _pushTipsNode;
        /// <summary>
        /// npc形象
        /// </summary>
        private SkeletonAnimation _npcSpine;

        /// <summary>
        /// 所有的推送数据
        /// </summary>
        private Dictionary<int,List<ConfigPushUnit>> _data = new Dictionary<int,List<ConfigPushUnit>>();

        //玩家登录的时间

        /// <summary>
        /// id对应函数名字
        /// </summary>
        private Dictionary<int, Func<ConfigPushUnit, (int,int)>> IdToFunc = new Dictionary<int, Func<ConfigPushUnit, (int, int)>>();
        /// <summary>
        /// 当前的推松的数据
        /// </summary>
        private ConfigPushUnit _pushData = null;
        /// <summary>
        /// 角色升级所需材料数量参数
        /// </summary>
        private int _common100002_value1 = 0;
        /// <summary>
        /// 宠物天赋令数量
        /// </summary>
        private int _common300010_value1 = 0;

        public void OnInit()
        {
            _pushTipsNode = UIPackage.CreateObject("Common", "PushTips") as UI_PushTips;//推送提示
            _pushTipsNode.touchable = false;
            _pushTipsNode.visible = false;
            GRoot.inst.AddChildAt(_pushTipsNode, GRoot.inst.numChildren);

            _pushTipsNode.GoTo.onClick.Add(OnClickGoTo);
            _pushTipsNode.GoTo.touchable = false;

            //角色形象
            Utils.SetSpineModelOnFGUI(_pushTipsNode.npc, "Pet_10060", 60f, HeroState.idle.ToString(), (o) =>
            {
                if (o is SkeletonAnimation animation)
                {
                    _npcSpine = animation;
                }
            });

            //数据
            var list = ConfigUtils.GetAllConfigPushUnit();
            foreach(var item in list)
            {
                if(item.Type <= 3)//目前支持前三种
                {
                    if(!_data.ContainsKey(item.Type))
                    {
                        _data[item.Type] = new List<ConfigPushUnit>();
                    }
                    _data[item.Type].Add(item);
                }
            }
            //排序优先级
            foreach(var item in _data)
            {
                item.Value.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            }

            IdToFunc.Add(1001, UpgradeEquipmentStore);//1001    升级装备商店
            IdToFunc.Add(1002, UpgradeSecretGuide);//1002    升级秘典
            IdToFunc.Add(1003, RoleBreakthrough);//1003    角色突破
            IdToFunc.Add(1004, RoleTalent);//1004    角色天赋
            IdToFunc.Add(1005, PetTalent);//1005    宠物天赋
            IdToFunc.Add(1006, PetWritingBooks);//1006    宠物打书
            IdToFunc.Add(1007, UpgradeTool);//1007    升级神器
            IdToFunc.Add(1008, UpgradeSacred);//1008    升级圣物
            IdToFunc.Add(1009, ReplaceInheritedEquipment);//1009    更换传承装备

            IdToFunc.Add(2001, DigEquipmentTreasure);//2001    挖装备宝箱
            IdToFunc.Add(2002, GoldPile);//2002    金币堆
            IdToFunc.Add(2003, DiamondMine);//2003    钻石矿
            IdToFunc.Add(2004, SokobanTreasure);//2004    深埋宝藏
            IdToFunc.Add(2005, HuntingMission);//2005    狩猎任务
            IdToFunc.Add(2006, RelicBuff);//2006    遗迹BUFF
            IdToFunc.Add(2007, AdventureCave);//2007    奇遇山洞
            IdToFunc.Add(2008, GoFishing);//2008    钓鱼
            IdToFunc.Add(2009, CatchPets);//2009    抓捕宠物
            IdToFunc.Add(2010, InheritingBOSS);//2010    传承BOSS

            IdToFunc.Add(3001, DailyTaskRewards);//3001    日常任务奖励
            IdToFunc.Add(3002, SevenDaySignIn);//3002    七日签到奖励
            IdToFunc.Add(1001, HeroRecruitment);//3003    英雄招募免费次数奖励
            IdToFunc.Add(3004, OfflineReward);//3004    挂机奖励
            IdToFunc.Add(3005, AchievementReward);//3005    成就奖励
            IdToFunc.Add(3006, Home_GrainWorkshop);//3006    家园 - 粮食工坊奖励
            IdToFunc.Add(3007, Home_ProcessingPlant);//3007    家园 - 加工厂奖励
            IdToFunc.Add(3008, Home_ExplorationCamp);//3008    家园 - 探索营地奖励
            IdToFunc.Add(3009, Home_Shack);//3009    家园 - 窝棚奖励
            IdToFunc.Add(3010, Home_Training);//3010    家园 - 训练场奖励
            IdToFunc.Add(3011, Home_StoneMine);//3011    石头矿区奖励
            IdToFunc.Add(3012, EmailReward);//3012    邮件奖励
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// 点击前往
        /// </summary>
        private void OnClickGoTo(EventContext context)
        {
            //触发跳转逻辑
            //JumpManager
        }

        /// <summary>
        /// 出现动画
        /// </summary>
        public void ShowTips()
        {
            if (!_pushTipsNode.visible)
                _pushTipsNode.visible = true;
            _pushTipsNode.touchable = true;
            _pushTipsNode.GoTo.touchable = true;

            //显示的位置
            int type = 0;

            //是在大地图上
            if(UIManager.Instance.IsTopController("ChapterMap")) { type = 0; }
            //是在营地和战斗中
            else if(UIManager.Instance.IsTopController("Lobby")) { type = 1; }

            Vector2 pos = Vector2.zero;
            switch (type)
            {
                case 0://大地图中
                    var chapterMap = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
                    pos = chapterMap.GetPushPos();
                    break;
                case 1://营地或关卡战斗中
                    var lobby = UIManager.Instance.FindByName("Lobby") as LobbyView;
                    pos = lobby.GetPushPos();
                    break;
            }
            pos.x = GRoot.inst.width - _pushTipsNode.width;
            pos.y -= _pushTipsNode.height;
            _pushTipsNode.xy = pos;
            //显示的文字
            _pushTipsNode.txt.text = _pushData.DocNote;

            //入场动画
            _pushTipsNode.In.Play();
        }
        /// <summary>
        /// 隐藏动画
        /// </summary>
        public void HideTips()
        {
            _pushTipsNode.touchable = false;
            _pushTipsNode.GoTo.touchable = false;
            //离场动画
            _pushTipsNode.Out.Play();
        }

        /// <summary>
        /// 检测推送
        /// </summary>
        public void ChkPush()
        {
            //功能类推送，对应的效果触发就可以激活
            //玩法类推送，数量和时间内没有操作就激活
            //奖励类推送，有可以领取，并且时间倒计时到达就激活
            
            int b = (int)PushType.FunctionPush;
            int e = (int)PushType.RewardPush;
            for (int i = b; i <= e; i++)
            {
                if(ChkFunctionPush(i))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 检测功能推送
        /// </summary>
        public bool ChkFunctionPush(int t)
        {
            if (!_data.ContainsKey(t)) { return false; }

            List<ConfigPushUnit> list = new List<ConfigPushUnit>(_data[t]);
            bool isChange = false;
            (int,int) r = (-1,-1);
            //根据顺序检测是否可以触发
            foreach (var d in _data[t])
            {
                if(IdToFunc.ContainsKey(d.Id))
                {
                    if(d.SystemId > 0 && FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType)d.SystemId).Item1 == false)
                    {//解锁条件不允许
                        continue;
                    }
                    //从Dictionary中取出并调用方法
                    if (IdToFunc.TryGetValue(d.Id, out var cb))
                    {
                        r = cb(d);
                        if (r.Item1 != 0)//调用方法，获取返回值
                        {
                            //记录数据
                            _pushData = d;
                            //显示
                            ShowTips();
                            //重新排序
                            list.Remove(d);
                            list.Add(d);

                            isChange = true;
                            break;
                        }
                    }
                }
            }
            if (isChange)
            {//将排序后的重新塞进去
                _data[t] = list;
                return true;
            }
            //定时逻辑，玩家登录开始计算
            return false;
        }
        
        /// <summary>
        /// 升级装备商店
        /// </summary>
        private (int, int) UpgradeEquipmentStore(ConfigPushUnit d)
        {
            //触发条件：玩家拥有的当前金币足够提升1个装备商店等级（当前商店购买次数所需的金币之和）
            var treasureChestUnit = ConfigUtils.GetTreasureChestUnitById(DataManager.Instance.GetTreasureData().id);
            if(treasureChestUnit == null || treasureChestUnit.GoldCoins == "") { return (-1, -1); }

            ulong need = 0;
            try { need = ulong.Parse(treasureChestUnit.GoldCoins); } catch { return (-1, -1); }

            if (DataManager.Instance.GetRoleData().gold < need)
            {
                return (-1, -1);
            }

            return (1, -1);
        }
        /// <summary>
        /// 升级秘典
        /// </summary>
        private (int, int) UpgradeSecretGuide(ConfigPushUnit d)
        {
            //触发条件：当前金币足够升级d % 次当前最高等级的秘典（d % 为Parameter字段值）
            var data = GrimoireManager.Instance.GetGrimoireDict();
            if(data.Count <= 0) { return (-1, -1); }//没有秘典
            //获取最高等级的秘典数据(如果有阶位区分，取最高阶优先)
            //ClassicInfo ci = null;
            int lvl = 0;
            long costGold = 0;
            int classicId = 0;
            foreach (var item in data)
            {
                if(item.Value.nextLevel != 0)
                {
                    if(costGold < item.Value.nextLevelCostGold)
                    {
                        classicId = item.Value.classicId;
                        lvl = item.Value.level;
                        costGold = item.Value.nextLevelCostGold;
                    }
                    //同水平中
                    else if(costGold == item.Value.nextLevelCostGold)
                    {
                        //等级最高
                        if(lvl < item.Value.level)
                        {
                            classicId = item.Value.classicId;
                            lvl = item.Value.level;
                            costGold = item.Value.nextLevelCostGold;
                        }
                    }
                }
            }
            if(costGold == 0) { return (-1, -1); }
            ulong num = 0;
            try { num = ulong.Parse(d.Parameter); } catch { return (-1, -1); }

            ulong need = (ulong)costGold * num;

            if (DataManager.Instance.GetRoleData().gold < need)
            {
                return (-1, -1);
            }
            return (1, classicId);
        }
        /// <summary>
        /// 角色突破
        /// </summary>
        private (int, int) RoleBreakthrough(ConfigPushUnit d)
        {
            //触发条件：
            //1、功能已开放
            //2、有角色处于可升级状态等级可突破，未突破不算可升级状态
            //3、已有的升级材料，可以使已拥有角色等级成长至下一个属性赋予等级。例：材料足够英雄升5，升25等
            if(_common100002_value1 == 0)
            {
                try
                {
                    _common100002_value1 = int.Parse(ConfigDataGroup.GetInstance<ConfigCommon>().Get(100002).Param1);
                }
                catch
                {
                    return (-1, -1);
                }
            }

            //检测所有的角色，满级到达可突破等级
            var list = HeroInfoManager.Instance.GetAllHeroUnits();
            foreach (var h in list)
            {
                HeroInfo heroInfo = HeroInfoManager.Instance.GetThisHero(h.Id);
                if (!heroInfo.IsGet)//非满级的
                { 
                    ConfigHeroLevelUnit breakUnit = ConfigUtils.GetHeroLevel(heroInfo.HeroUnit.Id, heroInfo.Level);
                    //判断是突破还是升级？ 
                    if (breakUnit != null && breakUnit.Item != "0" && breakUnit.Level > heroInfo.BreakLevel)//突破
                    {
                        if(FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroTupo).Item1)
                        {
                            string[] itemStr = breakUnit.Item.Split('|');
                            bool isEnough = true;
                            for (int i = 0; i < itemStr.Length; i++)
                            {
                                string[] itemArr = itemStr[i].Split(',');
                                if (ItemInfoManager.Instance.GetItemCount(int.Parse(itemArr[0])) < int.Parse(itemArr[1]))
                                {
                                    isEnough = false;
                                    break;
                                }
                            }
                            if(isEnough) return (2, h.Id);
                        }
                        //continue;//下一个
                    }
                    else//升级
                    {
                        if(FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroLevelUp).Item1)
                        {
                            int cnt = ItemInfoManager.Instance.GetItemCount(_common100002_value1);

                            if(cnt >= 1)
                            {
                                return (1, h.Id);
                            }
                        }
                    }
                }
            }
            return (-1, -1);
        }
        /// <summary>
        /// 角色天赋
        /// </summary>
        private (int, int) RoleTalent(ConfigPushUnit d)
        {
            //触发条件：
            //2、当天赋点大于当前最高等级天赋的升级消耗时
            var unlockTalentDict = TalentInfoManager.Instance.GetTalentDict();
            int maxLvl = 0; int maxLvlId = 0;
            foreach(var talent in unlockTalentDict)
            {
                if (unlockTalentDict[talent.Key] >= ConfigUtils.GetMaxLvAptitudeUnitByAptitUdeId(talent.Key).Lv)
                {//满级的无视
                    continue;
                }
                //取最高等级的天赋
                if(maxLvlId == 0)
                {
                    maxLvlId = talent.Key;
                    maxLvl = talent.Value;
                }
                else if(maxLvl < talent.Value)
                {
                    maxLvlId = talent.Key;
                    maxLvl = talent.Value;
                }
            }
            if(maxLvlId == 0) { return (-1, -1); }
            //检测材料
            int talentPoints = TalentInfoManager.Instance.GetTalentPoints();
            ConfigAptitudeUnit nextUnit = ConfigUtils.GetNextAptitudeUnitByAptitudeIdAndLv(maxLvlId, maxLvl);//下一等级天赋的unit
            int costTalentPoints = nextUnit == null ? 0 : nextUnit.Count.ToInt32();
            if (talentPoints >= costTalentPoints)
            {//可升级
                return (nextUnit.Id, -1);//外部使用，检测ID来定位到对应的标签页和对应的天赋
            }
            return (-1, -1);
        }
        /// <summary>
        /// 宠物天赋
        /// </summary>
        private (int,int) PetTalent(ConfigPushUnit d)
        {
            //触发条件//2、宠物天赋令1010010数大于d %（d % 为Parameter字段值）
            int num = 0;
            try { num = int.Parse(d.Parameter); } catch { return (-1, -1); }
            if(_common300010_value1 == -1)
            {
                string[] datas = ConfigDataGroup.GetInstance<ConfigCommon>().Get(300010).Param1.Split(',');
                try { 
                    _common300010_value1 = int.Parse(datas[0]);
                } catch { return (-1, -1); }
            }
            //令牌材料足够
            if (ItemInfoManager.Instance.GetItemCount(_common300010_value1) >= num)
            {
                return (1, -1);
            }

            return (-1, -1);
        }
        /// <summary>
        /// 宠物打书
        /// </summary>
        private (int, int) PetWritingBooks(ConfigPushUnit d)
        {
            //触发条件//2、包裹内存在的技能书品质高于宠物已携带的技能书品质
            //三只已经上阵的宠物中检测
            Dictionary<int, PetItemInfo> petList = PetInfoManager.Instance.GetUpLoadPet();
            foreach(var pet in petList)
            {
                if(pet.Value.BattleIndex != -1)//说明已经上阵
                {//所有的已经拥有的技能书
                    foreach(var skillBook in pet.Value.bookSlotsList)
                    {
                        if(skillBook.BookId != 0)//0表示没有
                        {
                            ConfigPetSkillBookUnit petBook = ConfigUtils.GetPetSkillBookUnitById(skillBook.BookId);
                            if(petBook != null)//没有品质，取这里的
                            {
                                //遍历所有背包内的书
                                List<PetSkillBook> bookList = PetInfoManager.Instance.GetPetSkillBookList();
                                foreach(var book in bookList)
                                {
                                    if(book.Quality > petBook.Quality)
                                    {
                                        return (pet.Value.BattleIndex, book.BookId);//返回上阵ID
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return (-1,-1);
        }
        private (int, int) ChkUpgradeTool(int type, int num)
        {
            var curUnit = ConfigUtils.GetArtifactEquipUnitByTypeAndLv(type, EquipManager.Instance.GetArtifactDict()[type]);
            ulong needDia = 0;//所需消耗钻石
            int needItemId = 0;//所需消耗道具Id
            ulong needItemNum = 0;//所需消耗道具数量
            for(int i = 0; i < num; i++)
            {
                ConfigArtifactEquipUnit nextArtifactEquip = ConfigUtils.GetNextArtifactEquipUnitByTypeAndLv(curUnit.Type, curUnit.ArtifactLevel + i);//下一等级
                if (nextArtifactEquip == null)
                {//满级
                    break;
                }
                needDia += (ulong)nextArtifactEquip.DiamondsCost;
                var costItems = nextArtifactEquip.ItemCost.Split(',');
                try
                {
                    needItemId = int.Parse(costItems[0]);
                    needItemNum += (ulong)int.Parse(costItems[1]);
                }
                catch
                {
                    break;
                }
            }

            if (needDia > 0 && needItemNum > 0 && needDia <= (ulong)DataManager.Instance.mRoleData.dia && needItemNum <= (ulong)ItemInfoManager.Instance.GetItemCount(needItemId))
            {
                return (1, type);//条件达成,返回哪个神器的类型
            }

            return (-1,-1);
        }
        /// <summary>
        /// 升级神器
        /// </summary>
        private (int, int) UpgradeTool(ConfigPushUnit d)
        {
            var _artifactDict = EquipManager.Instance.GetArtifactDict();
            int count = 0;

            try
            {
                count = int.Parse(d.Parameter);
                
                ArtifactType[] types = new ArtifactType[4] { ArtifactType.SHIELD, ArtifactType.MEDAL, ArtifactType.STONE, ArtifactType.JOB };
                foreach (var t in types)
                {
                    var r = ChkUpgradeTool((int)t, count);
                    if (r.Item1 > 0)
                    {//满足条件返回 哪个神器的类型
                        return r;
                    }
                }
            } catch { return (-1, -1); }
            
            return (-1, -1);
        }
        /// <summary>
        /// 升级圣物
        /// </summary>
        private (int, int) UpgradeSacred(ConfigPushUnit d)
        {
            //当前材料足够最高等级圣物升级到d%次
            int count = 0;
            try
            {
                count = int.Parse(d.Parameter);
                ulong costSP = 0;
                ulong costDia = 0;
                //已佩戴圣物类型中的
                var list = HolyManager.Instance.GetHolyItemBattleList();
                //HolyItemInfo holy = null;
                int holyId = -1;
                int lv = 0;
                foreach (var item in list)
                {
                    if(lv < item.Level)
                    {
                        holyId = item.HolyId;
                        lv = item.Level;
                    }
                }
                if(holyId <= 0) { return (-1, -1); }
                for(int i = 0; i < count; i++)
                {
                    // 获取下一级圣物信息
                    var nextHoly = ConfigUtils.GetNextHolyUnitByHolyIdAndLevel(holyId, lv + i);
                    costSP += (ulong)nextHoly.Consume1;
                    costDia += (ulong)nextHoly.Consume2;
                }
                // 判断材料是否充足 碎片 钻石
                if (costSP > 0 && costDia > 0 && (ulong)ItemInfoManager.Instance.GetItemCount(1010015) >= costSP && (ulong)DataManager.Instance.mRoleData.dia >= costDia)
                {
                    return (1, -1);
                }
            }
            catch { return (-1, -1); }

            return (-1, -1);
        }

        /// <summary>
        /// 更换传承装备
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private (int, int) ReplaceInheritedEquipment(ConfigPushUnit d)
        {
            //玩家传承装备栏未穿戴装备，包裹栏中有对应类型传承装备，或玩家传承装备栏存在品质高于穿戴栏的同类型传承装备（或的关系，两者满足其中一种就生效）

            //所有的背包中未穿戴的传承装备
            List<EquipData> el = EquipManager.Instance.GetNoWearLoreEquip();

            //四个槽位
            for (int i = 0; i < 4; i++)
            {
                int partIndex = i + 5;
                var partEquip = DataManager.Instance.FindEquip((EN_EQUIP_PARTS)partIndex);
                if (partEquip == null || partEquip.id == 0)
                {//没有装备，如果背包中有装备，选一件品质最高的
                    //EquipData me = null;
                    int guid = 0;//ID（如果直接使用类赋值，会影响原先的数据）
                    int quality = 0;//品质
                    int lv = 0;//等级
                    foreach (var e in el)
                    {
                        if(e.partType == partIndex)
                        {//选 品质》等级
                            if(quality < e.quality || (quality == e.quality && lv < e.lv))
                            {
                                guid = (int)e.guid;
                                quality = e.quality;
                                lv = e.lv;
                            }
                        }
                    }
                    if(guid != 0)
                    {
                        return (1, guid);
                    }
                }
                else//找个比当前装备品质更高，等级更高的
                {
                    //EquipData me = null;
                    int guid = (int)partEquip.guid;//ID（如果直接使用类赋值，会影响原先的数据）
                    int quality = partEquip.quality;//品质
                    int lv = partEquip.lv;//等级
                    foreach (var e in el)
                    {
                        if (e.partType == partIndex)
                        {//选 品质》等级
                            if (quality < e.quality || (quality == e.quality && lv < e.lv))
                            {
                                guid = (int)e.guid;
                                quality = e.quality;
                                lv = e.lv;
                            }
                        }
                    }
                    if (guid != 0)
                    {
                        return (1, guid);
                    }
                }
            }
            return (-1, -1);
        }
        /// <summary>
        /// 挖装备宝箱
        /// </summary>
        private (int, int) DigEquipmentTreasure(ConfigPushUnit d)
        {
            

            return (-1, -1);
        }
        /// <summary>
        /// 金币堆
        /// </summary>
        private (int, int) GoldPile(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 钻石矿
        /// </summary>
        private (int, int) DiamondMine(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 深埋宝藏
        /// </summary>
        private (int, int) SokobanTreasure(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 狩猎任务
        /// </summary>
        private (int, int) HuntingMission(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 遗迹BUFF
        /// </summary>
        private (int, int) RelicBuff(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 奇遇山洞
        /// </summary>
        private (int, int) AdventureCave(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 钓鱼
        /// </summary>
        private (int, int) GoFishing(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 抓捕宠物
        /// </summary>
        private (int, int) CatchPets(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 传承BOSS
        /// </summary>
        private (int, int) InheritingBOSS(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 日常任务奖励
        /// </summary>
        private (int, int) DailyTaskRewards(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 七日签到奖励
        /// </summary>
        private (int, int) SevenDaySignIn(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 英雄招募免费次数奖励
        /// </summary>
        private (int, int) HeroRecruitment(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 挂机奖励
        /// </summary>
        private (int, int) OfflineReward(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 成就奖励
        /// </summary>
        private (int, int) AchievementReward(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 家园-粮食工坊奖励
        /// </summary>
        private (int, int) Home_GrainWorkshop(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 家园-加工厂奖励
        /// </summary>
        private (int, int) Home_ProcessingPlant(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 家园-探索营地奖励
        /// </summary>
        private (int, int) Home_ExplorationCamp(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 家园-窝棚奖励
        /// </summary>
        private (int, int) Home_Shack(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 家园-训练场奖励
        /// </summary>
        private (int, int) Home_Training(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 石头矿区奖励
        /// </summary>
        private (int, int) Home_StoneMine(ConfigPushUnit d)
        {
            return (-1, -1);
        }
        /// <summary>
        /// 邮件奖励
        /// </summary>
        private (int, int) EmailReward(ConfigPushUnit d)
        {
            return (-1, -1);
        }

    }
}