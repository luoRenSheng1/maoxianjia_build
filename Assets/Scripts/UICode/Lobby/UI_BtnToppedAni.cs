/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_BtnToppedAni : GComponent
    {
        public UI_BtnTopped btnTopped;
        public Transition t0;
        public const string URL = "ui://s7x7ku0nllb1dxycr";

        public static UI_BtnToppedAni CreateInstance()
        {
            return (UI_BtnToppedAni)UIPackage.CreateObject("Lobby", "BtnToppedAni");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            btnTopped = (UI_BtnTopped)GetChild("btnTopped");
            t0 = GetTransition("t0");
        }
    }
}