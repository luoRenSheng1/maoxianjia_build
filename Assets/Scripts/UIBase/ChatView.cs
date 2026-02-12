
using System.Collections.Generic;
using Chat;
using CommonEx;
using Engine;
using FairyGUI;
using msg;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;
using UI_TabBtn = Chat.UI_TabBtn;

public class ChatView : UIViewBase
{
    private UI_ChatView Chat => this.main as UI_ChatView;

    private List<ChatVo> _chatVos;
    public ChatView()
    {
        this.name = "Chat";
        this.package = "Chat";
        this.component = "ChatView";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        ChatBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.Chat.closeBtn.onClick.Add(this.Hide);
        this.Chat.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.Chat.chatList.itemProvider = ItemProvider;
        this.Chat.chatList.itemRenderer = ChatItemRender;
        this.Chat.tabCtrl.onChanged.Add(this.OnTabCtrlChange);
        this.Chat.sendChatBtn.onClick.Add(this.OnClickSendChat);
        
        EventDispatcher.GameWorld.Regist<ChatVo>(EventDefine.EVENT_CHAT_UPDATE_Lobby, this.OnUpdateChatList);
    }

    private string ItemProvider(int index)
    {
        if (_chatVos[index].Channel == (int) eChatChannel.eChatChannel_System)
        {
            return "ui://x1rxl2idkvszn";
        }
        if (_chatVos[index].SenderName == DataManager.Instance.GetRoleData().userName)
        {
            return "ui://x1rxl2idkvszl";
        }
        return "ui://x1rxl2idkvszk";
    }

    private void ChatItemRender(int index, GObject item)
    {
        bool isMy = _chatVos[index].SenderUserId == ulong.Parse(DataManager.Instance.GetRoleData().userID);
        _chatVos[index].isRead = true;
        if (_chatVos[index].Channel == (int) eChatChannel.eChatChannel_System)
        {
            ((UI_ChatSysItem) item).content.text = _chatVos[index].Content;
            ((UI_ChatSysItem) item).playerName.text = _chatVos[index].SenderName;
        }else if (_chatVos[index].Channel == (int) eChatChannel.eChatChannel_World)
        {
            if (isMy)
            {
                ((UI_ChatRightItem) item).content.text = _chatVos[index].Content;
                ((UI_ChatRightItem) item).playerName.text = _chatVos[index].SenderName;
                ((UI_ChatRightItem) item).myHeadIcon.url = UIResource.GetItemUrl(DataManager.Instance.GetRoleData().GetAvatarUrl());
            }
            else
            {
                ((UI_ChatLeftItem) item).content.text = _chatVos[index].Content;
                ((UI_ChatLeftItem) item).playerName.text = _chatVos[index].SenderName;
                ((UI_ChatLeftItem) item).otherHeadIcon.url = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(_chatVos[index].SenderIcon).Icon);
            } 
        }

    }

    private void OnTabCtrlChange()
    {
        int index = this.Chat.tabCtrl.selectedIndex;
        if (index == 0)
        {
            _chatVos = ChatManager.Instance.GetWoldChatList();
            this.Chat.chatList.numItems = _chatVos.Count;
        }else if (index == 1)
        {
            _chatVos = ChatManager.Instance.GetSysChatList();
            this.Chat.chatList.numItems = _chatVos.Count;
        }
        this.Chat.chatList.scrollPane.ScrollBottom(true);
        UpdateRedDot();
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CHAT_UPDATE_Lobby_Red);
    }

    protected override void OnShow()
    {
        base.OnShow();
        if (ChatManager.Instance.Channel == (int) eChatChannel.eChatChannel_World)
        {
            this.Chat.tabCtrl.selectedIndex = 0;
        }
        else if(ChatManager.Instance.Channel == (int) eChatChannel.eChatChannel_System)
        {
            this.Chat.tabCtrl.selectedIndex = 1;
        }

        this.Chat.typeCtrl.selectedIndex = 0;
        OnTabCtrlChange();
    }

    private void OnUpdateChatList(ChatVo chatVo)
    {
        if (IsOnStage() && IsShow())
        {
            if (chatVo.Channel == (int) eChatChannel.eChatChannel_World && this.Chat.tabCtrl.selectedIndex == 0)
            {
                this.Chat.ipt.text = "";
                _chatVos = ChatManager.Instance.GetWoldChatList();
                this.Chat.chatList.numItems = _chatVos.Count;
            }
            else if(chatVo.Channel == (int) eChatChannel.eChatChannel_System && this.Chat.tabCtrl.selectedIndex == 1)
            {
                _chatVos = ChatManager.Instance.GetSysChatList();
                this.Chat.chatList.numItems = _chatVos.Count;
            }
            this.Chat.chatList.scrollPane.ScrollBottom(true);
            UpdateRedDot();
        }
        
    }

    private void UpdateRedDot()
    {
        int worldNotRead = ChatManager.Instance.HasNoReadChatList();
        ((UI_TabBtn) this.Chat.chatBtnList.GetChildAt(0)).redDot.visible = worldNotRead > 0;
        if (worldNotRead > 1)
        {
            ((UI_RedDotComponent) ((UI_TabBtn) this.Chat.chatBtnList.GetChildAt(0)).redDot).countType.selectedIndex = 1;
            ((UI_RedDotComponent) ((UI_TabBtn) this.Chat.chatBtnList.GetChildAt(0)).redDot).countText.text = worldNotRead.ToString();
        }
        else
        {
            ((UI_RedDotComponent) ((UI_TabBtn) this.Chat.chatBtnList.GetChildAt(0)).redDot).countType.selectedIndex = 0;
        }
        
        int sysNotRead = ChatManager.Instance.HasNoReadSysChatList();
        ((UI_TabBtn) this.Chat.chatBtnList.GetChildAt(1)).redDot.visible = sysNotRead > 0;
        if (sysNotRead > 1)
        {
            ((UI_RedDotComponent) ((UI_TabBtn) this.Chat.chatBtnList.GetChildAt(1)).redDot).countType.selectedIndex = 1;
            ((UI_RedDotComponent) ((UI_TabBtn) this.Chat.chatBtnList.GetChildAt(1)).redDot).countText.text = worldNotRead.ToString();
        }
        else
        {
            ((UI_RedDotComponent) ((UI_TabBtn) this.Chat.chatBtnList.GetChildAt(1)).redDot).countType.selectedIndex = 0;
        }
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CHAT_UPDATE_Lobby_Red);
    }

    private void OnClickSendChat()
    {
        // if (DataManager.Instance.GetRoleData().lv < 3)
        // {
        //     this.Chat.ipt.text = "";
        //     UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(10158, 3));
        //     return;
        // }
        
        if(string.IsNullOrEmpty(this.Chat.ipt.text))
        {
            UIManager.Instance.ToastByKey(10157);
            return;
        }

        if (IllegalWordDetection.DetectIllegalWords(this.Chat.ipt.text).Count > 0)
        {
            UIManager.Instance.ToastByKey(10159);
            return;
        }

        var builder = Chat_CS.CreateBuilder();
        builder.Channel = (uint) eChatChannel.eChatChannel_World;
        builder.Content = this.Chat.ipt.text;
        builder.ReceiverGuid = 0;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_Chat_CS, builder.Build());
    }
}
