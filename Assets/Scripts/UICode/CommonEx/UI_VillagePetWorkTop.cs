/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_VillagePetWorkTop : GComponent
    {
        public GProgressBar workBar;
        public const string URL = "ui://5moj1x39hvyadxy4g";

        public static UI_VillagePetWorkTop CreateInstance()
        {
            return (UI_VillagePetWorkTop)UIPackage.CreateObject("CommonEx", "VillagePetWorkTop");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            workBar = (GProgressBar)GetChild("workBar");
        }
    }
}