/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_HitCom : GComponent
    {
        public GTextField desc;
        public GButton yesBtn;
        public GButton noBtn;
        public const string URL = "ui://5moj1x39l8pndxy7r";

        public static UI_HitCom CreateInstance()
        {
            return (UI_HitCom)UIPackage.CreateObject("CommonEx", "HitCom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            desc = (GTextField)GetChild("desc");
            yesBtn = (GButton)GetChild("yesBtn");
            noBtn = (GButton)GetChild("noBtn");
        }
    }
}