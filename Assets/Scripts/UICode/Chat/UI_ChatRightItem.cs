/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Chat
{
    public partial class UI_ChatRightItem : GComponent
    {
        public GLoader myHeadIcon;
        public GTextField content;
        public GTextField playerName;
        public const string URL = "ui://x1rxl2idkvszl";

        public static UI_ChatRightItem CreateInstance()
        {
            return (UI_ChatRightItem)UIPackage.CreateObject("Chat", "ChatRightItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            myHeadIcon = (GLoader)GetChild("myHeadIcon");
            content = (GTextField)GetChild("content");
            playerName = (GTextField)GetChild("playerName");
        }
    }
}