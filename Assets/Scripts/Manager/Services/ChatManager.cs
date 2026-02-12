using System;
using System.Collections.Generic;
using EngineBase;
using msg;
using UnityEngine;

namespace Engine
{
    public class ChatVo
    {
        public ulong Guid;
        public int Channel;
        public ulong SenderUserId;
        public string SenderName;
        public string Content;
        public ulong SendTime;
        public int SenderIcon;
        public int SenderLv;
        public ulong SendPlayerGuid;
        public int SendVipLv;
        public ulong ReceiverGuid;

        public bool isRead = false;
    }
    public class ChatManager : TSingleton<ChatManager>
    {
        public ulong ForbidEndTime { get; set; }
        public int Channel { get; set; } = (int) eChatChannel.eChatChannel_World;
        
        private List<ChatVo> _chatList = new List<ChatVo>();
        private List<ChatVo> _sysChatList = new List<ChatVo>();

        public void OnInit()
        {
            EventDispatcher.GameWorld.Regist<ChatVo>(EventDefine.EVENT_CHAT_UPDATE, this.OnUpdateChat);
        }

        public override void Dispose()
        {
            EventDispatcher.GameWorld.UnRegist<ChatVo>(EventDefine.EVENT_CHAT_UPDATE, this.OnUpdateChat);
            base.Dispose();
        }

        private void OnUpdateChat(ChatVo chatVo)
        {
            chatVo.isRead = false;
            chatVo.Content = IllegalWordDetection.Filter(chatVo.Content);
            if (chatVo.Channel == (int) eChatChannel.eChatChannel_World)
            {
                _chatList.Add(chatVo);
            }
            else if(chatVo.Channel == (int) eChatChannel.eChatChannel_System)
            {
                _sysChatList.Add(chatVo);
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CHAT_UPDATE_Lobby, chatVo);
        }

        public List<ChatVo> GetWoldChatList()
        {
            return _chatList;
        }
        
        public List<ChatVo> GetSysChatList()
        {
            return _sysChatList;
        }

        public int HasNoReadChatList()
        {
            int noReadCnt = 0;
            foreach (var item in _chatList)
            {
                if (!item.isRead)
                {
                    noReadCnt++;
                }
            }

            return Mathf.Min(99, noReadCnt);
        }
        
        public int HasNoReadSysChatList()
        {
            int noReadCnt = 0;
            foreach (var item in _sysChatList)
            {
                if (!item.isRead)
                {
                    noReadCnt++;
                }
            }

            return Mathf.Min(99, noReadCnt);
        }
    }
}