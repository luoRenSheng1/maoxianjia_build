/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Achievement
{
    public partial class UI_AchievementItem : GComponent
    {
        public Controller getBtnStatus;
        public Controller isOk;
        public GLoader icon;
        public GTextField desc;
        public UI_ItemBtn itemBtn;
        public UI_GetBtn getBtn;
        public UI_Bar bar;
        public const string URL = "ui://b8bvql0ir1ahi";

        public static UI_AchievementItem CreateInstance()
        {
            return (UI_AchievementItem)UIPackage.CreateObject("Achievement", "AchievementItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            getBtnStatus = GetController("getBtnStatus");
            isOk = GetController("isOk");
            icon = (GLoader)GetChild("icon");
            desc = (GTextField)GetChild("desc");
            itemBtn = (UI_ItemBtn)GetChild("itemBtn");
            getBtn = (UI_GetBtn)GetChild("getBtn");
            bar = (UI_Bar)GetChild("bar");
        }
    }
}