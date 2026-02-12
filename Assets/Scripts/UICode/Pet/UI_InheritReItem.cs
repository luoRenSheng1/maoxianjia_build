/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_InheritReItem : GComponent
    {
        public Controller quality;
        public Controller isCheck;
        public GLoader qualityIcon;
        public GTextField desc;
        public GTextField num;
        public GButton btnCheck;
        public const string URL = "ui://lxs2h4ifhz5cdxy7i";

        public static UI_InheritReItem CreateInstance()
        {
            return (UI_InheritReItem)UIPackage.CreateObject("Pet", "InheritReItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            quality = GetController("quality");
            isCheck = GetController("isCheck");
            qualityIcon = (GLoader)GetChild("qualityIcon");
            desc = (GTextField)GetChild("desc");
            num = (GTextField)GetChild("num");
            btnCheck = (GButton)GetChild("btnCheck");
        }
    }
}