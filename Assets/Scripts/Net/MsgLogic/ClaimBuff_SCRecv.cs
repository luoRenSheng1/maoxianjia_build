using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ClaimBuff_SCRecv : IReceiver
    {
        public ClaimBuff_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimBuff_SC;
        }

        public void Process()
        {   //todo 获取buff
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                //FightAttrVo fightAttrVo = new FightAttrVo();//DataManager.Instance.GetRoleData().FightAttrVo;
                
                ClaimBuff claimBuff = new ClaimBuff();
                // claimBuff.buffId = (int)msg.BuffId;   // buffId 暂时没什么用
                // int buffId = (int)msg.BuffId;  // buffId 暂时没什么用
                
                // TODO buff列表  添加buff  根据这个 有没有值  做表现
                // foreach (var value in msg.BuffsList)
                // {
                //     BuffInfo buffInfo = new BuffInfo();
                //     /* buff 有值 获得这个buff  只会有一个值 */
                //     foreach (var item in value.BattleAttrList)
                //     {
                //         double attrValue = double.Parse(item.AttrValue.ToString("f4"));
                //         BuffData buffData = new BuffData();
                //         buffData.battleAttr = item.AttrId;
                //         buffData.attrValue = attrValue;
                //         buffInfo.battleAttrList.Add(buffData);
                //     }
                //     buffInfo.startTime = value.StartTime;
                //     buffInfo.endTime = value.EndTime;
                //     buffInfo.cfgId = (int)value.CfgId;
                //     claimBuff.buffInfo = buffInfo;
                // }
                // DataManager.Instance.claimBuff = claimBuff;
                
                // TODO 通知做表现
                // UIManager.Instance.ShowUIPanel("GetBuffReward", true);
                
                
                // 修改版：大地图遗迹buff
                foreach (var value in msg.BuffsList)
                {
                    BuffInfo buffInfo = new BuffInfo();
                    /* buff 有值 获得这个buff  只会有一个值 */
                    foreach (var item in value.BattleAttrList)
                    {
                        double attrValue = double.Parse(item.AttrValue.ToString("f4"));
                        BuffData buffData = new BuffData();
                        buffData.battleAttr = item.AttrId;
                        buffData.attrValue = attrValue;
                        buffInfo.battleAttrList.Add(buffData);
                    }
                    buffInfo.startTime = value.StartTime;
                    buffInfo.endTime = value.EndTime;
                    buffInfo.cfgId = (int)value.CfgId;
                    
                    DataManager.Instance.AddBuffInfos(buffInfo);
                }
                
                // 领取完buff后，清理buff信息
                MapChapterManager.Instance._ruinBuffInfo = null;
                
                // 设置可以挑战的boss的index
                MapChapterManager.Instance._canFightMonsterIndex = (int)msg.MonsterIndex + 1;
                
                // 领取完最后一个遗迹奖励后，2秒后自动传送出副本
                if (msg.MonsterIndex == 2)
                {
                    MapChapterManager.Instance._canExitRuinMap = true;
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_AUTOEXIT_RUIN);
                }
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CLAIM_BUFF_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RUIN_CLAIMBUFF_UPDATE, msg.MonsterIndex);
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_FIGHT_INFO);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ClaimBuff_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
