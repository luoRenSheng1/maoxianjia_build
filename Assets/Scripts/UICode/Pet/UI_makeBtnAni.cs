/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_makeBtnAni : GComponent
    {
        public UI_MakeBtn makeBtn;
        public Transition t0;
        public const string URL = "ui://lxs2h4ifllb1dxyc4";

        public static UI_makeBtnAni CreateInstance()
        {
            return (UI_makeBtnAni)UIPackage.CreateObject("Pet", "makeBtnAni");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            makeBtn = (UI_MakeBtn)GetChild("makeBtn");
            t0 = GetTransition("t0");
        }
    }
}