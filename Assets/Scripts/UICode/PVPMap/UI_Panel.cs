/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_Panel : GComponent
    {
        public UI_battleRoot battleRoot;
        public Transition t0;
        public const string URL = "ui://zoxecbv2qc4n1";

        public static UI_Panel CreateInstance()
        {
            return (UI_Panel)UIPackage.CreateObject("PVPMap", "Panel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            battleRoot = (UI_battleRoot)GetChild("battleRoot");
            t0 = GetTransition("t0");
        }
    }
}