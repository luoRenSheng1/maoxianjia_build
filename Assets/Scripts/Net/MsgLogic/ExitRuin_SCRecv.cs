using msg;

namespace Engine
{
    using EngineBase;
    
    public class ExitRuin_SCRecv : IReceiver
    {
        public ExitRuin_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ExitRuin_SC;
        }

        public void Process()
        {   // 退出废墟返回
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                // 关闭遗迹地图
                UIManager.Instance.CloseUIPanel("RuinMap");
                
                MapChapterManager.Instance.ResetRuinState();//重置遗迹地图状态
                
                //通知刷新
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ENTER_AND_EXIT_RUIN);//标签栏显示
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ExitRuin_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}