using System.Collections.Generic;
using System.Linq;
using EngineBase;
using msg;
using UnityEngine;

namespace Engine
{
    /// <summary>
    /// 进入奇遇山洞小地图（接收回包）
    /// </summary>
    public class EnterCave_SCRecv : IReceiver
    {
        public EnterCave_SC msg;
        public int MsgID()
        {
            return (int)eMsgID.eMsg_EnterCave_SC;
        }

        public void Process()
        {
            if(msg == null || msg.Result != eErrCode.eErrCode_Success) { return; }

            //是否处于本地数据，如果不是就不继续
            int type = MapChapterManager.Instance.GetEventTypeByGuid(msg.EventGuid);
            if(type != -1 && type == (int)eRandomEventType.eRandomEventType_AdventureBusinessMan)
            {
                var view = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
                view?.ShowAdventureCaveMap(msg.EventGuid);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("EnterCave_SC_Recv Read " + mRecv.Length);
            msg = EnterCave_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}