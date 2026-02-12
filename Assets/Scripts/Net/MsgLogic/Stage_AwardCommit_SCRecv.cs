using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using CommonEx;
using Config;
using EngineBase;
using FairyGUI;
using msg;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;
using Random = UnityEngine.Random;

namespace Engine
{
    public class Stage_AwardCommit_SCRecv : IReceiver
    {
        public Stage_AwardCommit_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Stage_AwardCommit_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                // Debug.LogFormat("GroupId:{0}", msg.GroupId);
                // Debug.LogFormat("MonsterGold:{0}", msg.MonsterGold);
                // Debug.LogFormat("MonsterId:{0}", msg.MonsterId);
                double gold = msg.MonsterGold;
                float x = msg.PosX, y = msg.PosY;
                Vector2 pos = new Vector2(x, y);
                if(gold > 0)
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.FIGHT_DATA_MONSTER_DEAD, pos, gold);
                if (UIManager.Instance.IsTopController("Lobby"))
                {
                    if (msg.Rune.Guid > 0) //符石表现
                    {
                        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById((int) msg.Rune.ItemId);
                        LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
                        if (lobbyView != null)
                        {
                            UIItemsGain itemsGain = UIGainBasePool.CreateUIGainBase(); //new UIItemsGain();
                            itemsGain.ApplyItemSourceToDestination(lobbyView.GetUserHeadIcon(),
                                itemTypeUnit.Icon); //type=4 符石
                            itemsGain.StartItemFly(pos, 1);
                        }
                    }

                    List<ItemData> ret = new List<ItemData>();
                    PlayerAttrUtils.GetItemData(msg.DropItem.ItemsList.ToList(), ref ret);
                    for (int i = 0; i < ret.Count; i++)
                    {
                        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(ret[i].id);
                        LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
                        UIItemsGain itemsGain = UIGainBasePool.CreateUIGainBase(); //new UIItemsGain();
                        itemsGain.ApplyItemSourceToDestination(lobbyView?.GetUserHeadIcon(),
                            itemTypeUnit.Icon); //type=3 通关拿到钥匙
                        itemsGain.StartItemFly(pos, Random.Range(1, 2));
                    }
                }

                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
                PlayerAttrUtils.UpdateFinance(msg.Finance);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = Stage_AwardCommit_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
