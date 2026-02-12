/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Setting
{
    public partial class UI_HeadIconChange : GComponent
    {
        public GList iconList;
        public GButton changeBtn;
        public const string URL = "ui://zs0w02qtbaxfx";

        public static UI_HeadIconChange CreateInstance()
        {
            return (UI_HeadIconChange)UIPackage.CreateObject("Setting", "HeadIconChange");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            iconList = (GList)GetChild("iconList");
            changeBtn = (GButton)GetChild("changeBtn");
        }
    }
}