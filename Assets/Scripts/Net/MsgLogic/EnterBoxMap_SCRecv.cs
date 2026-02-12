using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    /// <summary>
    /// 进入深埋宝藏小地图（接收回包）
    /// </summary>
    public class EnterBoxMap_SCRecv : IReceiver
    {
        public EnterBoxMap_SC msg;
        public int MsgID()
        {
            return (int)eMsgID.eMsg_EnterBoxMap_SC;
        }

        public void Process()
        {
            /*
            	optional eErrCode result = 1;// [default = eSucceed];	 //处理结果
                optional uint64 event_guid = 2;  //事件guid
                repeated BatchStuff batch_stuff = 3;  //服务器端生成的结果
            */
            if (msg != null)
            {
                Debug.Log($"进入推箱子副本 Result = {msg.Result}");
            }
            if (msg == null || msg.Result != eErrCode.eErrCode_Success) { return; }
            //是否处于本地数据，如果不是就不继续
            int type = MapChapterManager.Instance.GetEventTypeByGuid(msg.EventGuid);
            if(type == (int)eRandomEventType.eRandomEventType_RandomBox)
            {
                //更新事件内容
                List<BatchStuff> temp = new List<BatchStuff>();
                for(int i = 0; i < msg.BatchStuffCount; i++)
                {
                    temp.Add(new BatchStuff(msg.BatchStuffList[i]));
                }
                MapChapterManager.Instance.UpdateBatchStuffList(msg.EventGuid, temp);

                //进入小地图副本
                var view = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
                view?.ShowSokobanMapView(msg.EventGuid);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("EnterBoxMap_SC_Recv Read " + mRecv.Length);
            msg = EnterBoxMap_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}