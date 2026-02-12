/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_BtnSkill : GButton
    {
        public Controller hasSkill;
        public Controller qualityCtrl;
        public GLoader qualityIcon;
        public UI_skillIcon skillIcon;
        public GImage mask2;
        public GImage mask;
        public GComponent redDot;
        public GLoader3D tapSpine;
        public const string URL = "ui://s7x7ku0npdpadxxz3";

        public static UI_BtnSkill CreateInstance()
        {
            return (UI_BtnSkill)UIPackage.CreateObject("Lobby", "BtnSkill");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            hasSkill = GetController("hasSkill");
            qualityCtrl = GetController("qualityCtrl");
            qualityIcon = (GLoader)GetChild("qualityIcon");
            skillIcon = (UI_skillIcon)GetChild("skillIcon");
            mask2 = (GImage)GetChild("mask2");
            mask = (GImage)GetChild("mask");
            redDot = (GComponent)GetChild("redDot");
            tapSpine = (GLoader3D)GetChild("tapSpine");
        }
    }
}