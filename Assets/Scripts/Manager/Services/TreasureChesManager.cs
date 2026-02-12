using System;
using Config;
using EngineBase;
using UnityEngine;
using System.Collections;
using System.Linq;
using msg;
using EventDispatcher = EngineBase.EventDispatcher;
using PlayerPrefsEx = EngineBase.PlayerPrefs;

namespace Engine
{
    public enum TreasureBoxOpenStatus
    {
        None,
        ProgressBarFull,
        LevelUp
    }
    public class TreasureChesManager : TSingleton<TreasureChesManager>
    {
        private float _timer = 0;
        private Coroutine _coTimeFlow;
        public int TreasureBoxCurNum
        {
            get
            {
                return DataManager.Instance.GetRoleData().equipBoxExp;
            }
        }

        public TreasureBoxOpenStatus TreasureBoxOpenStatus { get; set; }
        

        public void StartTreasureCd()
        {
            var equipMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Zhuzhao);
            if(!equipMap.Item1) return;
            if (null != _coTimeFlow)
                GameManager.Instance.StopCoroutine(_coTimeFlow);

            int lv = DataManager.Instance.GetTreasureData().id;
            var cfgCurLv = ConfigUtils.GetTreasureChestUnitById(lv);
            if (null != cfgCurLv && lv < TreasureChesManager.Instance.GetTreasureMaxLv())
            {
                int totalSecond = (int)(DataManager.Instance.GetTreasureData().lastTargetTime - ServerTimeManager.Instance.CurServerTime);
                if (totalSecond > 0)
                {
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TREASURE_CD_UPDATE, StringUtils.GetTimeString((int)totalSecond));
                    _coTimeFlow = GameManager.Instance.StartCoroutine(TimeFlow());
                }
            }
            
        }

        IEnumerator TimeFlow()
        {
            while (true)
            {
                int totalSecond = (int)(DataManager.Instance.GetTreasureData().lastTargetTime - ServerTimeManager.Instance.CurServerTime);
                if (totalSecond > 0)
                {
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TREASURE_CD_UPDATE, StringUtils.GetTimeString((int)totalSecond));
                }

                if (totalSecond <= 0)
                {
                    GameManager.Instance.TimerManager.SetTimer(1, () =>
                    {
                        //发送协议 宝箱倒计时完成
                        var builder = EquipBox_Levelup_CS.CreateBuilder();
                        EquipBox_Levelup_CS levelup = builder.Build();
                        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_EquipBox_Levelup_CS, levelup);
                    });
                    break;
                }

                yield return GameManager.Instance.waitSec1;
            }
        }
        
        private bool CheckTreasureChes(int lv)
        {
            var configUnit = ConfigUtils.GetTreasureChestUnitById(lv);
            if (null != configUnit)
            {
                var count = DataManager.Instance.GetMagicKeys();
                if (count >= configUnit.Consume)
                {
                    return true;
                }
            }

            return false;
        }

        public void UseTreasureChes(int lv, int batchStuffId, ulong eventGuid)
        {
            if (CheckTreasureChes(lv))
            {
                var configUnit = ConfigUtils.GetTreasureChestUnitById(lv);
                if (null != configUnit)
                {
                    var builder = GenEquips_CS.CreateBuilder();
                    builder.GenTimes = 1;
                    builder.BatchStuffId = (uint)batchStuffId;
                    builder.EventGuid = eventGuid;
                    GenEquips_CS genEquip = builder.Build();
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_GenEquips_CS, genEquip);
                }

            }
            else
            {
                ConfigGiftUnit giftUnit = ConfigUtils.GetGiftUnitsById((int) Gift_Bury.Gift_zzc);
                int zzcBuyNum = ShopInfoManager.Instance.GetPackNum(giftUnit.Id).BuyCounter;
                if (zzcBuyNum < giftUnit.BuyNumber)
                {
                    //打开铸造锤礼包
                    // UIManager.Instance.ShowUIPanel("BuryGiftPack", Gift_Bury.Gift_zzc);// 关闭商业化
                }
                else
                {
                    UIManager.Instance.ToastByKey(10134);
                }

            }
        }

        public int GetTreasureMaxLv()
        {
            int maxLv = 0;
            var data = ConfigUtils.GetConfigTreasureChestUnits();
            if (null != data && data.Values.ToList().Count >= 1)
            {
                maxLv = data.Values.ToList()[data.Count - 1].Id;
            }

            return maxLv;
        }

        /// <summary>
        /// 是否宝箱最大等级
        /// </summary>
        /// <returns></returns>
        public (int, int) CheckTreasureChes()
        {
            //上限
            int maxLv = GetTreasureMaxLv();
            int currLv = DataManager.Instance.GetTreasureData().id;
            if (currLv >= maxLv)
            {
                return (10014, 0);
            }

            int lv = DataManager.Instance.GetTreasureData().id;
            var cfgCurLv = ConfigUtils.GetTreasureChestUnitById(lv);
            if (null != cfgCurLv)
            {
                if (lv > 1)
                {
                    //cd
                    if (DataManager.Instance.GetTreasureData().lastTargetTime > 0)
                    {
                        return (0, 10011);
                    }
                }

                return (0, 0);
            }

            return (1004, 0);
        }

        /// <summary>
        /// 宝箱升级
        /// </summary>
        /// <param name="lv"></param>
        /// <returns></returns>
        public void TreasureChesLevelUp()
        {
            var data = CheckTreasureChes();
            int code = data.Item1;
            switch (code)
            {
                case 0:
                    var builder = EquipBox_StartLevelup_CS.CreateBuilder();
                    EquipBox_StartLevelup_CS startLevelup = builder.Build();
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_EquipBox_StartLevelup_CS, startLevelup);
                    break;
                case 1000:
                case 1001:
                case 1002:
                case 1003:
                    PushLevelUpFailed(code);
                    break;
                case 1004:
                    break;
                default:
                    UIManager.Instance.ToastByKey(code);
                    break;
            }
        }

        private void PushLevelUpFailed(int errorCode)
        {
            JsonObject jsObj2 = new JsonObject();
            jsObj2["errCode"] = errorCode;
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TREASURE_LEVELUP_RES, MsgCode.FAIL, jsObj2.ToJson());
        }

        public void PushLevelUpSuccess()
        {
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TREASURE_LEVELUP_RES, MsgCode.SUCCESS, DataManager.Instance.mTreasureData.ToJson());
        }

        public void AutoTreasure()
        {
            if (EquipManager.Instance.AutoUnpack && !EquipManager.Instance.HasNewEquipToStopAutopack)
            {
                if(ItemInfoManager.Instance.GetItemCount(ConstDefine.CONST_MAGIC_KEY) > 0)
                    UseTreasureChes(DataManager.Instance.GetTreasureData().id, 0, 0);
                else
                {
                    _timer = 0;
                    EquipManager.Instance.AutoUnpack = false;
                    EquipManager.Instance.HasNewEquipToStopAutopack = false;
                }
            }
        }

        public void BuyTreasureBoxNum()
        {
            double curGold = double.Parse(ConfigUtils.GetTreasureChestUnitById(DataManager.Instance.GetTreasureData().id).GoldCoins);
            if (DataManager.Instance.GetRoleData().gold > curGold)
            {
                LogUtils.Log("能升级");
                var builder = EquipBox_LevelupMilestone_CS.CreateBuilder();
                EquipBox_LevelupMilestone_CS boxLevelUp = builder.Build();
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_EquipBox_LevelupMilestone_CS, boxLevelUp);
            }
            else
            {
                Utils.OpenBuyGoldView();
            }
            
        }

        public void Tick(float deltaSeconds)
        {
            if (!EquipManager.Instance.AutoUnpack)
            {
                _timer = 0;
                return;
            }
            _timer += deltaSeconds;
            if (_timer >= 1.5f)
            {
                AutoTreasure();
                _timer = 0;
            }
        }

        public bool TreasureBoxRedPoint()
        {
            var zhuzhaiMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Zhuzhao);
            if (!zhuzhaiMap.Item1)
            {
                return false;
            }
            
            bool canUpLv = false;
            int currLv = DataManager.Instance.GetTreasureData().id;
            int maxLv = TreasureChesManager.Instance.GetTreasureMaxLv();
            
            if (currLv < maxLv)
            {
                if (TreasureChesManager.Instance.TreasureBoxOpenStatus != TreasureBoxOpenStatus.LevelUp)
                {
                    var treasureChestUnit = ConfigUtils.GetTreasureChestUnitById(DataManager.Instance.GetTreasureData().id);
                    if (treasureChestUnit != null)
                    {

                        // canUpLv = DataManager.Instance.GetRoleData().gold >= double.Parse(treasureChestUnit.GoldCoins);
                        canUpLv = DataManager.Instance.GetRoleData().gold >= double.Parse(treasureChestUnit.GoldCoins) * treasureChestUnit.GoldCoinsNum;
                    }

                }
            }
            int totalSecond = (int)(DataManager.Instance.GetTreasureData().lastTargetTime - ServerTimeManager.Instance.CurServerTime);
            bool isPro = totalSecond > 0;
            var data = TreasureChesManager.Instance.CheckTreasureChes();
            return canUpLv && data.Item1 == 0 && !isPro;
        }
    }
}