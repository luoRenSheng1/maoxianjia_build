/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Achievement
{
    public partial class UI_AchievementSelectBtn : GButton
    {
        public GComponent redPoint;
        public const string URL = "ui://b8bvql0irwnpm";

        public static UI_AchievementSelectBtn CreateInstance()
        {
            return (UI_AchievementSelectBtn)UIPackage.CreateObject("Achievement", "AchievementSelectBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            redPoint = (GComponent)GetChild("redPoint");
        }
    }
}