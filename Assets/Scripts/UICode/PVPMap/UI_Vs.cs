/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_Vs : GComponent
    {
        public GTextField timeLb;
        public const string URL = "ui://zoxecbv2auqc22";

        public static UI_Vs CreateInstance()
        {
            return (UI_Vs)UIPackage.CreateObject("PVPMap", "Vs");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            timeLb = (GTextField)GetChild("timeLb");
        }
    }
}