using CommonEx;
using Engine;
using FairyGUI;
using msg;

namespace Lobby
{
    public partial class UI_ComMessage : GButton
    {
        public void InitChatInfo()
        {
            this.btnChat.onClick.Add(OnClickChat);
            this.chatBtn.onClick.Add(OnClickChat);
            EngineBase.EventDispatcher.GameWorld.Regist<ChatVo>(EventDefine.EVENT_CHAT_UPDATE_Lobby, this.OnUpdateChat);
            EngineBase.EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CHAT_UPDATE_Lobby_Red, this.UpdateChatRedDot);
            UpdateChatRedDot();
            this.pGroup.visible = false;
        }
        
        private void OnClickChat()
        {
            //LogUtils.LogWarning("OnClickChat");
            UIManager.Instance.ShowUIPanel("Chat");
        }

        private void OnUpdateChat(ChatVo chatVo)
        {
            UpdateChatRedDot();
            this.pGroup.visible = true;
            if (chatVo.Channel == (int) eChatChannel.eChatChannel_World)
            {
                this.type.selectedIndex = 1;
                this.pNameLb.SetVar("pName", chatVo.SenderName).FlushVars();
                this.contentLb.text = chatVo.Content;
            }
            else
            {
                this.type.selectedIndex = 0;
                this.pNameLb.SetVar("pName", chatVo.SenderName).FlushVars();
                this.contentLb.text = chatVo.Content;
            }

        }

        private void UpdateChatRedDot()
        {
            int noReadCnt = 0;
            if (ChatManager.Instance.Channel == (int) eChatChannel.eChatChannel_World)
            {
                noReadCnt = ChatManager.Instance.HasNoReadChatList();
            }else if (ChatManager.Instance.Channel == (int) eChatChannel.eChatChannel_System)
            {
                noReadCnt = ChatManager.Instance.HasNoReadSysChatList();
            }

            this.redDot.visible = noReadCnt > 0;
            if (noReadCnt == 1)
            {
                ((UI_RedDotComponent) this.redDot).countType.selectedIndex = 0;
            }
            else
            {
                ((UI_RedDotComponent) this.redDot).countType.selectedIndex = 1;
                ((UI_RedDotComponent) this.redDot).countText.text = noReadCnt.ToString();
            }
        }
    }
}