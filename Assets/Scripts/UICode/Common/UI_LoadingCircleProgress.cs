/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_LoadingCircleProgress : GLabel
    {
        public Transition t0;
        public const string URL = "ui://0anhreylkwfmxxne";

        public static UI_LoadingCircleProgress CreateInstance()
        {
            return (UI_LoadingCircleProgress)UIPackage.CreateObject("Common", "LoadingCircleProgress");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            t0 = GetTransition("t0");
        }
    }
}