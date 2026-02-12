/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Chat
{
    public partial class UI_ChatLeftItem : GComponent
    {
        public GLoader otherHeadIcon;
        public GTextField content;
        public GTextField playerName;
        public const string URL = "ui://x1rxl2idkvszk";

        public static UI_ChatLeftItem CreateInstance()
        {
            return (UI_ChatLeftItem)UIPackage.CreateObject("Chat", "ChatLeftItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            otherHeadIcon = (GLoader)GetChild("otherHeadIcon");
            content = (GTextField)GetChild("content");
            playerName = (GTextField)GetChild("playerName");
        }
    }
}