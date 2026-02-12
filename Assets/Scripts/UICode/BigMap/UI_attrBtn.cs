/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_attrBtn : GButton
    {
        public Controller qualityCtrl;
        public GTextField name;
        public const string URL = "ui://pdufy3keipa310";

        public static UI_attrBtn CreateInstance()
        {
            return (UI_attrBtn)UIPackage.CreateObject("BigMap", "attrBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            qualityCtrl = GetController("qualityCtrl");
            name = (GTextField)GetChild("name");
        }
    }
}