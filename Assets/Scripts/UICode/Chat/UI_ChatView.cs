/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Chat
{
    public partial class UI_ChatView : GComponent
    {
        public Controller typeCtrl;
        public Controller tabCtrl;
        public GComponent frame;
        public GList chatBtnList;
        public GList chatList;
        public GButton closeBtn;
        public GButton settingBtn;
        public GTextInput ipt;
        public GButton sendChatBtn;
        public const string URL = "ui://x1rxl2idr13r0";

        public static UI_ChatView CreateInstance()
        {
            return (UI_ChatView)UIPackage.CreateObject("Chat", "ChatView");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeCtrl = GetController("typeCtrl");
            tabCtrl = GetController("tabCtrl");
            frame = (GComponent)GetChild("frame");
            chatBtnList = (GList)GetChild("chatBtnList");
            chatList = (GList)GetChild("chatList");
            closeBtn = (GButton)GetChild("closeBtn");
            settingBtn = (GButton)GetChild("settingBtn");
            ipt = (GTextInput)GetChild("ipt");
            sendChatBtn = (GButton)GetChild("sendChatBtn");
        }
    }
}