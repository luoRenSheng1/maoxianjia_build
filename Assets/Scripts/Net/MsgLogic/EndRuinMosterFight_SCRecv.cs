using System.Collections.Generic;
using Lobby;
using msg;

namespace Engine
{
    using EngineBase;
    
    public class EndRuinMosterFight_SCRecv : IReceiver
    {
        public EndRuinMosterFight_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_EndRuinMosterFight_SC;
        }

        public void Process()
        {   // 遗迹结束打怪返回
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                if (msg.IsWin)
                {
                    
                    // 遗迹buff
                    List<int> buffIdList = new List<int>();
                    foreach (var buffId in msg.BuffIdList)
                    {
                        buffIdList.Add((int)buffId);
                    }
                    
                    RuinBuffInfo ruinBuffInfo = new RuinBuffInfo();
                    ruinBuffInfo.guid = msg.EventGuid;
                    ruinBuffInfo.monsterIndex = msg.MonsterIndex;
                    ruinBuffInfo.buffIds = buffIdList;
                    
                    MapChapterManager.Instance.AddRuinBuffInfo(ruinBuffInfo);
                    MapChapterManager.Instance._defeatMonsterIndex = (int)msg.MonsterIndex;

                }
                
                var view = UIManager.Instance.FindByName("Lobby") as LobbyView;
                if (view != null)
                {
                    (view?.GetBottomList().GetChildAt(2) as UI_BtnBottom).title = ConfigUtils.GetStringByKey(5171);
                }

                //通知刷新
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_END_RUINBOSS_FIGHT);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = EndRuinMosterFight_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}