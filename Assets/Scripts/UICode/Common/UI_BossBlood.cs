/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_BossBlood : GComponent
    {
        public GProgressBar tweenBar;
        public GProgressBar tweenBar2;
        public GProgressBar hpBar;
        public GLoader icon;
        public GTextField title;
        public const string URL = "ui://0anhreyliv53dxyh9";

        public static UI_BossBlood CreateInstance()
        {
            return (UI_BossBlood)UIPackage.CreateObject("Common", "BossBlood");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            tweenBar = (GProgressBar)GetChild("tweenBar");
            tweenBar2 = (GProgressBar)GetChild("tweenBar2");
            hpBar = (GProgressBar)GetChild("hpBar");
            icon = (GLoader)GetChild("icon");
            title = (GTextField)GetChild("title");
        }
    }
}