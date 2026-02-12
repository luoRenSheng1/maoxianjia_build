/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Achievement
{
    public partial class UI_AchievementMain : GComponent
    {
        public GComponent frame;
        public GList list;
        public GList tabList;
        public GButton closeBtn;
        public const string URL = "ui://b8bvql0ir1ahh";

        public static UI_AchievementMain CreateInstance()
        {
            return (UI_AchievementMain)UIPackage.CreateObject("Achievement", "AchievementMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            list = (GList)GetChild("list");
            tabList = (GList)GetChild("tabList");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}