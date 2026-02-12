using System.Collections.Generic;
using System.Linq;
using EngineBase;
using msg;
using UnityEngine;

namespace Engine
{
    public class PlayerItemsInfo4Login_PC_Recv : IReceiver
    {
        public PlayerItemsInfo4Login_PC msg;
        public int MsgID()
        {
            return (int) eMsgID.eMsg_PlayerItemsInfo4Login_PC;
        }

        public void Process()
        {
            LogUtils.LogWarningFormat("PlayerItemsInfo4Login_PC_Recv Process");
            List<ItemData> ret = new List<ItemData>();
            PlayerAttrUtils.GetItemData(msg.ItemsList.ToList(), ref ret);

            if (msg.SplitInfo.IsEnd)//最后一条数据了
            {
                //结束Item数据接收，可以更新UI
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("PlayerItemsInfo4Login_PC_Recv Read " + mRecv.Length);
            msg = PlayerItemsInfo4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}