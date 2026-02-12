using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerRingUpdate_PC_Recv : IReceiver
    {
        public PlayerRingUpdate_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerRingUpdate_PC;
        }

        public void Process()
        {
            foreach (var item in msg.RingAttrList)
            {
                ShuxingInfo shuxingInfo = new ShuxingInfo();
                switch (item.AttrId)
                {
                    // case eBattleAttr.eBattleAttr_Atk:
                    case eBattleAttr.eBattleAttr_FinalAttack:
                        shuxingInfo.Type = ShuxingType.ATK;
                        shuxingInfo.Lv = (int) item.Level;
                        break;
                    case eBattleAttr.eBattleAttr_HP:
                        shuxingInfo.Type = ShuxingType.HP;
                        shuxingInfo.Lv = (int) item.Level;
                        break;
                    case eBattleAttr.eBattleAttr_HP_Recovery:
                        shuxingInfo.Type = ShuxingType.Recovery;
                        shuxingInfo.Lv = (int) item.Level;
                        break;
                    case eBattleAttr.eBattleAttr_CriticalStrike_Rate:
                        shuxingInfo.Type = ShuxingType.CriticalStrike;
                        shuxingInfo.Lv = (int) item.Level;
                        break;
                    case eBattleAttr.eBattleAttr_CriticalInjury_Rate:
                        shuxingInfo.Type = ShuxingType.CriticalInjury;
                        shuxingInfo.Lv = (int) item.Level;
                        break;
                }
                RoleManager.Instance.UpdateShuxingInfo(shuxingInfo);
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_RingInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerRingUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
