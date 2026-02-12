using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GenEquips_SCRecv : IReceiver
    {
        public GenEquips_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GenEquips_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                int newEquipItemId = 0;
                
                EquipData tmpEquipData = null;
                foreach (var item in msg.EquipsList)
                {
                    EquipData equipData = new EquipData();
                    equipData.guid = item.Guid;
                    equipData.id = (int)item.BaseItemId;
                    newEquipItemId = (int)item.BaseItemId;

                    foreach (var item2 in item.EntryInfoList)
                    {
                        Debug.Log(item2.EquipAttrList[0].AttrId);
                    }
                    
                    (List<int>, List<double>) basicRet = PlayerAttrUtils.GetAttrID_Value(item.EquipAttrList.ToList());
                    equipData.lstAttrsID = basicRet.Item1;
                    equipData.lstAttrsValue = basicRet.Item2;
                    (List<int>,List<int>, List<double>) entryRet = PlayerAttrUtils.GetEntryAttrID_Value(item.EntryInfoList.ToList());
                    equipData.lstEntryCfgsID = entryRet.Item1;
                    equipData.lstEntrysID = entryRet.Item2;
                    equipData.lstEntrysValue = entryRet.Item3;
                    equipData.partType = (int)item.EquipType;
                    equipData.quality = (int)item.Quality;
                    equipData.lv = item.Level;
                    equipData.recallGold = (int)item.RecallGold;
                    foreach (var data in item.EntrySkillsList.ToList())
                    {
                        SkillMultipleData multipleData = new SkillMultipleData();
                        multipleData.skillId = (int)data.SkillId;
                        multipleData.times = (int)data.SkillMultiple_;
                        equipData.skillMultiplesList.Add(multipleData);
                    
                        equipData.lstEntryCfgsID.Add((int)data.SkillId);
                        equipData.lstEntrysID.Add((int)data.SkillId);
                        equipData.lstEntrysValue.Add((int)data.SkillMultiple_);
                    }
                    EquipManager.Instance.AddEquipItem(equipData);
                    tmpEquipData = equipData;
                }
                
                //为事件时
                // msg.BatchStuffId;//树id
                // msg.BatchStuffEventStatus;//树状态
                // msg.EventGuid;//事件guid
                if (msg.EventGuid > 0)
                {
                    RandomEventData randomEventData = new RandomEventData();
                    randomEventData = MapChapterManager.Instance.GetRandomEventDataByGuid(msg.EventGuid);
                    foreach (var batchStuff in randomEventData.batchStuffList)
                    {
                        if (batchStuff.id == msg.BatchStuffId)
                        {
                            batchStuff.id = (int)msg.BatchStuffId;
                            batchStuff.eventStatus = (int)msg.BatchStuffEventStatus;
                            break;
                        }
                    }
                    //MapChapterManager.Instance.UpdateRandomEventList(randomEventData);  //刷新数据，会更新全部关卡数据，需要刷新可以单独写一个
                }
                
                //扣除钥匙
                EquipManager.Instance.ItemConsume(1);
                if (tmpEquipData != null)
                {
                    bool isNew = EquipManager.Instance.IsNewPartEquip((int)tmpEquipData.partType);

                    // int deltaTime = 0;
                    int deltaTime = 2;
                    int type = 2;
                    if (msg.EventGuid != 0)
                    {
                        deltaTime = 2;
                        type = 1;
                    }
                    EquipManager.Instance.newEquipItemId = newEquipItemId;
                    
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_USE_TREASURE_CHES_RES, tmpEquipData, isNew, deltaTime, type);
                    if (msg.EventGuid != 0)
                    {
                        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RANDOM_TREASURE_UPDATE);
                    }
                    
                }

                ItemInfoManager.Instance.GetItemData((int) msg.CosumedKeys.Id).count -= (int) msg.CosumedKeys.Num;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GenEquips_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
