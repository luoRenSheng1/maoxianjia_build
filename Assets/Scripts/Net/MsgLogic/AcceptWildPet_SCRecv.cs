using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class AcceptWildPet_SCRecv : IReceiver
    {
        public AcceptWildPet_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AcceptWildPet_SC;
        }

        public void Process()
        {
            // 逃跑的宠物 开始攻击
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                RandomEventData eventData = MapChapterManager.Instance.GetRandomEventDataByGuid(msg.EventGuid);
                eventData.eventStatus = (int)msg.EventStatus;
                MapChapterManager.Instance.UpdateRandomEventList(eventData);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = AcceptWildPet_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
