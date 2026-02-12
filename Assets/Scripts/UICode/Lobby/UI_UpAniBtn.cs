/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_UpAniBtn : GComponent
    {
        public UI_UpBtn upBtn;
        public Transition t0;
        public const string URL = "ui://s7x7ku0nfr3pdxycq";

        public static UI_UpAniBtn CreateInstance()
        {
            return (UI_UpAniBtn)UIPackage.CreateObject("Lobby", "UpAniBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            upBtn = (UI_UpBtn)GetChild("upBtn");
            t0 = GetTransition("t0");
        }
    }
}