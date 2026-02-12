/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_BarHp : GComponent
    {
        public Controller bossType;
        public UI_BarHpComponent hpBar;
        public const string URL = "ui://5moj1x39gwxtdxy3m";

        public static UI_BarHp CreateInstance()
        {
            return (UI_BarHp)UIPackage.CreateObject("CommonEx", "BarHp");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bossType = GetController("bossType");
            hpBar = (UI_BarHpComponent)GetChild("hpBar");
        }
    }
}