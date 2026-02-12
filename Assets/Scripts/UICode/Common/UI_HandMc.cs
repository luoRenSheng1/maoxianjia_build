/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_HandMc : GComponent
    {
        public Transition t0;
        public const string URL = "ui://0anhreyluq40dxy4e";

        public static UI_HandMc CreateInstance()
        {
            return (UI_HandMc)UIPackage.CreateObject("Common", "HandMc");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            t0 = GetTransition("t0");
        }
    }
}