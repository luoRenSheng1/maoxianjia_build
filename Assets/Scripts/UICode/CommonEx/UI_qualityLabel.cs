/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_qualityLabel : GLabel
    {
        public Controller qualityCtrl;
        public const string URL = "ui://5moj1x39kn0fdxy3p";

        public static UI_qualityLabel CreateInstance()
        {
            return (UI_qualityLabel)UIPackage.CreateObject("CommonEx", "qualityLabel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            qualityCtrl = GetController("qualityCtrl");
        }
    }
}