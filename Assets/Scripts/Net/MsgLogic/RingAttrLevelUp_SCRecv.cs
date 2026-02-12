using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class RingAttrLevelUp_SCRecv : IReceiver
    {
        public RingAttrLevelUp_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RingAttrLevelUp_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                ShuxingInfo shuxingInfo = new ShuxingInfo();
                switch (msg.RingAttr.AttrId)
                {
                    // case eBattleAttr.eBattleAttr_Atk:
                    case eBattleAttr.eBattleAttr_FinalAttack:
                        shuxingInfo.Type = ShuxingType.ATK;
                        shuxingInfo.Lv = (int) msg.RingAttr.Level;
                        shuxingInfo.Val = msg.RingAttr.AttrValue;
                        break;
                    case eBattleAttr.eBattleAttr_HP:
                        shuxingInfo.Type = ShuxingType.HP;
                        shuxingInfo.Lv = (int) msg.RingAttr.Level;
                        shuxingInfo.Val = msg.RingAttr.AttrValue;
                        break;
                    case eBattleAttr.eBattleAttr_HP_Recovery:
                        shuxingInfo.Type = ShuxingType.Recovery;
                        shuxingInfo.Lv = (int) msg.RingAttr.Level;
                        shuxingInfo.Val = msg.RingAttr.AttrValue;
                        break;
                    case eBattleAttr.eBattleAttr_CriticalStrike_Rate:
                        shuxingInfo.Type = ShuxingType.CriticalStrike;
                        shuxingInfo.Lv = (int) msg.RingAttr.Level;
                        shuxingInfo.Val = msg.RingAttr.AttrValue;
                        break;
                    case eBattleAttr.eBattleAttr_CriticalInjury_Rate:
                        shuxingInfo.Type = ShuxingType.CriticalInjury;
                        shuxingInfo.Lv = (int) msg.RingAttr.Level;
                        shuxingInfo.Val = msg.RingAttr.AttrValue;
                        break;
                }
                RoleManager.Instance.UpdateShuxingInfo(shuxingInfo);
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_RingInfo);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = RingAttrLevelUp_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
