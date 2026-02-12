using System;
using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerBattleAttrDelBuff_PC_Recv : IReceiver
    {
        public PlayerBattleAttrDelBuff_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerBattleAttrDelBuff_PC;
        }

        public void Process()
        {   //登录后，主动下发战斗总属性
            FightAttrVo fightAttrVo = DataManager.Instance.GetRoleData().FightAttrVo;
            RoleManager.Instance.UpdateFightInfo(FightUtils.GetFightAttrVo(msg.BattleAttrList,fightAttrVo));
            
            FightAttrVo fightAttrVo1 = FightUtils.GetFightAttrVo(msg.BattleAttrList,fightAttrVo);
            LogUtils.LogWarning((int)eMsgID.eMsg_PlayerBattleAttrDelBuff_PC +":服务器删除-伤害:" + fightAttrVo1.Atk + "-----服务器删除-宠物伤害:" + fightAttrVo1.PetAtk + "-----服务器删除-防御:" + fightAttrVo1.Def + "-----服务器删除-生命:" + fightAttrVo1.HP);

            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_HERO_Attr);
            //遗迹buff  移除客户端有倒计时，不用处理
            if (msg.BattleBuffList.Count > 0)
            {
                //buff列表  添加buff  根据这个 有没有值  做表现
                // foreach (var value in msg.BattleBuffList)
                // {
                // }
            }
            
            //战斗内触发的buff  技能、天赋、宠物等触发的buff  做表现用，现在没有  值已经加到 BattleAttrList 里面
            if (msg.BattleBuf4StageList.Count > 0)
            {
                //buff列表  添加buff  根据这个 有没有值  做表现
                // foreach (var value in msg.BattleBuf4StageList)
                // {
                // }
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerBattleAttrDelBuff_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
