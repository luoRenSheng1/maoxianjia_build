using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class Chat_PC_Recv : IReceiver
    {
        public Chat_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Chat_PC;
        }

        public void Process()
        {
            ChatVo chatVo = new ChatVo();
            chatVo.Guid = msg.Guid;
            chatVo.Channel = (int) msg.Channel;
            chatVo.SenderUserId = msg.SenderUserid;
            chatVo.SenderName = msg.SenderName;
            chatVo.Content = msg.Content;
            chatVo.SendTime = (ulong)msg.TimeStamp;
            chatVo.SenderIcon = msg.SenderIcon;
            chatVo.SenderLv = msg.SenderLevel;
            chatVo.SendPlayerGuid = msg.SenderGuid;
            chatVo.SendVipLv = msg.SenderRealVipLv;
            chatVo.ReceiverGuid = msg.ReceiverGuid;
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CHAT_UPDATE, chatVo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = Chat_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
