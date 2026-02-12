
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using BigMap;
    using CommonEx;
    using Config;
    using Engine;
    using FairyGUI;
    using msg;
    using UnityEngine;
    using BatchStuff = Engine.BatchStuff;

    public enum FishingBossType
    {
        /// <summary>
        /// 普通boss
        /// </summary>
        CommomBoss = 0,
        /// <summary>
        /// 传承boss
        /// </summary>
        InheritBoss = 1,
    }

    public class FishingBossStageDetailView : UIViewBase
    {
        private UI_FishingBossStageDetail _fishingBossStageDetail => this.main as UI_FishingBossStageDetail;

        private int bossId;//垂钓结果是 普通boss，则是stage表的id；  垂钓结果是传承boss，则是 EventStage表中的id
        private FishingBossType bossType;
        
        private Coroutine _coTimeFlow;
        
        private List<int> traitArr; // 属性配置列表
        private string[] rewardArr; // 掉落奖励配置列表
        private List<string> showRewardArr = new List<string>(); // 掉落奖励展示列表
        
        private List<ConfigMonsterEntryUnit> monsterEntryList = new List<ConfigMonsterEntryUnit>(); // 属性配置列表
        
        // 普通boss
        // private ConfigStageUnit stageUnit;
        
        // 传承boss
        

        public FishingBossStageDetailView()
        {
            this.package = "BigMap";
            this.name = "FishingBossStageDetail";
            this.component = "FishingBossStageDetail";
            this.removePackage = true;
            this.safeAreaInset = true;
        }
        
        public override void BindAll()
        {
            base.BindAll();
            BigMapBinder.BindAll();
        }
        
        protected override void OnInit()
        {
            base.OnInit();
            this._fishingBossStageDetail.btnFight.onClick.Add(this.OnClickFightBtn);

            // this._fishingBossStageDetail.attrList.itemRenderer = MonsterAttributeRender;
            this._fishingBossStageDetail.rewardList.itemRenderer = ShowDropRewardRenfer;
        }

        protected override void OnHide()
        {
            base.OnHide();
            Utils.ClearSpineModelOnFGUI(this._fishingBossStageDetail.monsterIcon);
        }

        protected override void OnUpdateParams(params object[] values)
        {
            base.OnUpdateParams(values);
            bossId = (int)values[0];
            bossType = (FishingBossType)values[1];
        }

        protected override void OnShow()
        {
            base.OnShow();
            showRewardArr.Clear();
            this._fishingBossStageDetail.bossType.selectedIndex = (int)bossType;
            
            // 普通boss
            if (bossType == FishingBossType.CommomBoss)
            {
                UpdateCommonBossInfo();
            }

            // 传承boss
            if (bossType == FishingBossType.InheritBoss)
            {
                UpdateInheritBossInfo();
            }
            
            if (null != _coTimeFlow)
                GameManager.Instance.StopCoroutine(_coTimeFlow);
            _coTimeFlow = GameManager.Instance.StartCoroutine(OnTimeDown());
        }
        
        IEnumerator OnTimeDown()
        {
            int time = 5;
            while (time >= 0)
            {
                this._fishingBossStageDetail.txtTitleTime.SetVar("value", time.ToString()).FlushVars();
                yield return new WaitForSeconds(1);
                time--;
            }
            OnClickFightBtn();
        }

        /// <summary>
        /// 普通boss
        /// </summary>
        private void UpdateCommonBossInfo()
        {
            ConfigStageUnit stageUnit = ConfigUtils.GetStageUnitByIndexId(bossId);
            
            int groupId = stageUnit.MonsterData;
            var monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
            ConfigMonsterGroupUnit data = monsterGroupArr[0];
            ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(data.MonsterId);
            var monsterModelPath = monsterUnit.Model;
            Utils.SetSpineModelOnFGUI(this._fishingBossStageDetail.monsterIcon, monsterModelPath, monsterUnit.BossSize, "idle", null, true);

            this._fishingBossStageDetail.txtDesc.text = ConfigUtils.GetTextById(monsterUnit.Name);
            
            showRewardArr.Add(((int)DropItemType.GOLD).ToString());//金币
            showRewardArr.Add(((int)DropItemType.INHERIT).ToString());//传承装备
            
            // 掉落组
            List<string> dropItems2 = new List<string>();//处理完之后的物品列表
            dropItems2 = HanlderDorpShowItem(ConfigUtils.GetDropUnitByDropGroup(int.Parse(stageUnit.Drop)));
            showRewardArr.AddRange(dropItems2);
            this._fishingBossStageDetail.rewardList.numItems = showRewardArr.Count;
            
            // 特性：暂时不需要
            this._fishingBossStageDetail.isShowTx.selectedIndex = 0;

        }

        /// <summary>
        /// 传承boss
        /// </summary>
        private void UpdateInheritBossInfo()
        {
            ConfigEventStageUnit eventStageUnit = ConfigUtils.GetEventStageUnitById(bossId);
            
            int groupId = eventStageUnit.MonsterData;
            var monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
            ConfigMonsterGroupUnit data = monsterGroupArr[0];
            ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(data.MonsterId);
            var monsterModelPath = monsterUnit.Model;
            Utils.SetSpineModelOnFGUI(this._fishingBossStageDetail.monsterIcon, monsterModelPath, monsterUnit.BossSize, "idle", null, true);

            this._fishingBossStageDetail.txtDesc.text = ConfigUtils.GetTextById(monsterUnit.Name);
            
            int inheritId = (int)DropItemType.INHERIT;
            showRewardArr.Add(inheritId.ToString());//传承装备
            
            // 掉落组
            List<string> dropItems2 = new List<string>();//处理完之后的物品列表
            dropItems2 = HanlderDorpShowItem(ConfigUtils.GetDropUnitByDropGroup(eventStageUnit.Drop));
            showRewardArr.AddRange(dropItems2);
            this._fishingBossStageDetail.rewardList.numItems = showRewardArr.Count;
            
            // 特性：暂时不需要
            this._fishingBossStageDetail.isShowTx.selectedIndex = 0;
            
        }

        private List<string> HanlderDorpShowItem(List<ConfigDropUnit> dropUnitList)
        {
            //规则：首掉落位：金币    第2掉落位置：秘典，展示通用秘典图标  第3掉落位置：圣物，展示通用圣物图标  第4掉落位置：传承装备，展示通用传承装备图标  第5掉落位置：常规读取
            List<string> dropItemList = new List<string>();
            foreach (var dropUnit in dropUnitList)
            {
                ConfigItemTypeUnit itemUnit = ConfigUtils.GetConfigItemTypeUnitById(dropUnit.Item);
                if (itemUnit.Type == 11)
                {
                    dropItemList.Add(((int)DropItemType.CLASSIC).ToString() +",1");
                }
                else if (itemUnit.Type == 16)
                {
                    dropItemList.Add(((int)DropItemType.HOLY).ToString() + ",1");
                }
                else
                {
                    dropItemList.Add(dropUnit.Item.ToString() + ",1");
                }
            }
            
            dropItemList = dropItemList.Distinct().ToList();//去除重复奖励
            
            dropItemList.Add(((int)DropItemType.INHERIT).ToString() + ",1");//传承
            
            //排序
            dropItemList = dropItemList.OrderBy(item => 
                {
                    // 优先级顺序
                    if (item == ((int)DropItemType.CLASSIC).ToString() +",1") return 0;
                    if (item == ((int)DropItemType.HOLY).ToString() + ",1") return 1;
                    if (item == ((int)DropItemType.INHERIT).ToString() + ",1") return 2;
                    return 3; // 其他类型
                })
                .ToList();
            
            return dropItemList;
            
        }
        
        private void ShowDropRewardRenfer(int index, GObject item)
        {
            string[] items = showRewardArr[index].Split(",");
            ((UI_ItemCom)item).itemSpineEff.visible = false;
        
            // ((UI_ItemCom) item).txtLv.text = items[1].ToString();
            ((UI_ItemCom) item).txtLv.visible = false;
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(items[0]));
            ((UI_ItemCom) item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
            ((UI_ItemCom) item).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
        
            ((UI_ItemCom)item).data = itemTypeUnit.Id;
            ((UI_ItemCom)item).onClick.Set(this.OnClickDropReward);
        
            // ItemData itemData = new ItemData()
            // {
            //     id = int.Parse(items[0]),
            //     count = 0,//int.Parse(items[1])
            // };
            // ((UI_ItemCom) item).SetItemDataWithGuid(itemData, true);  //提示
        }
        
        private void OnClickDropReward(EventContext context)
        {
            int itemId = (int)(context.sender as UI_ItemCom).data;
            TipsManger.Instance.ShowPopupTip((UI_ItemCom)context.sender, Tipstype.None, itemId);
        }
        
        private void OnClickFightBtn()
        {
            if (null != _coTimeFlow)
                GameManager.Instance.StopCoroutine(_coTimeFlow);
            
            // 制造假随机事件数据，调用通用随机事件副本接口
            RandomEventData randomEventData = new RandomEventData();
            randomEventData.eventType = (int)StageEventType.Fishing;
            BatchStuff batchStuff = new BatchStuff();
            batchStuff.id = (int)bossType;//普通为0，传承为1
            batchStuff.cfgId = bossId;//普通boss，则是stage表的id      传承boss，则是 EventStage表中的id
            randomEventData.batchStuffList.Add(batchStuff);
            
            DungeonMapManager.Instance.mapEventData = randomEventData;
            DungeonMapManager.Instance.batchStuffId = (int)bossType;
            
            MapChapterManager.Instance.HideMapObject();

            MapChapterManager.Instance.SendAttackFishingBossBegin();
            
            UIManager.Instance.CloseUIPanel("FishingBossStageDetail");

        }

    }
