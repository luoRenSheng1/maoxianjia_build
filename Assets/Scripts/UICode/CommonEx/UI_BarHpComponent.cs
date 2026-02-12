/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_BarHpComponent : GComponent
    {
        public Controller bossType;
        public UI_BarHpTween tweenBar;
        public UI_BarHpTween tweenBar2;
        public UI_BarHpTween hpBar;
        public GTextField HPText;
        public GTextField ATKText;
        public const string URL = "ui://5moj1x39hargdxy3l";

        public static UI_BarHpComponent CreateInstance()
        {
            return (UI_BarHpComponent)UIPackage.CreateObject("CommonEx", "BarHpComponent");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bossType = GetController("bossType");
            tweenBar = (UI_BarHpTween)GetChild("tweenBar");
            tweenBar2 = (UI_BarHpTween)GetChild("tweenBar2");
            hpBar = (UI_BarHpTween)GetChild("hpBar");
            HPText = (GTextField)GetChild("HPText");
            ATKText = (GTextField)GetChild("ATKText");
        }
    }
}