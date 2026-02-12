/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_PetBar : GProgressBar
    {
        public Controller maxCtrl;
        public GTextField maxLevel;
        public const string URL = "ui://5moj1x39ucd4dxycm";

        public static UI_PetBar CreateInstance()
        {
            return (UI_PetBar)UIPackage.CreateObject("CommonEx", "PetBar");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            maxCtrl = GetController("maxCtrl");
            maxLevel = (GTextField)GetChild("maxLevel");
        }
    }
}