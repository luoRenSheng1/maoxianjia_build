/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_PassiveBtnSkill : GButton
    {
        public Controller hasSkill;
        public UI_passiveSkillIcon skillIcon;
        public GImage mask;
        public GComponent redDot;
        public const string URL = "ui://s7x7ku0n9jppdxy4m";

        public static UI_PassiveBtnSkill CreateInstance()
        {
            return (UI_PassiveBtnSkill)UIPackage.CreateObject("Lobby", "PassiveBtnSkill");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            hasSkill = GetController("hasSkill");
            skillIcon = (UI_passiveSkillIcon)GetChild("skillIcon");
            mask = (GImage)GetChild("mask");
            redDot = (GComponent)GetChild("redDot");
        }
    }
}