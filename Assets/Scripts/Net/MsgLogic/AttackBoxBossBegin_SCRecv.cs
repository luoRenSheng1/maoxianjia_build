using msg;

namespace Engine
{
    using EngineBase;
    /// <summary>
    /// 深埋宝藏 宝箱结果是传承boss,则发起战斗
    /// </summary>
    public class AttackBoxBossBegin_SCRecv : IReceiver
    {
        public AttackBoxBossBegin_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AttackBoxBossBegin_SC;
        }

        public void Process()
        {
            // 野外boss 开始攻击
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                MapChapterManager.Instance.EnterRandomEventCopyBattle();
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ENTER_RANDOM_BOX_RESULT);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000 + (int)msg.Result);
                DungeonMapManager.Instance.mapEventData = null;
                // 事件时间到期，也要刷新怪物状态
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = AttackBoxBossBegin_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
