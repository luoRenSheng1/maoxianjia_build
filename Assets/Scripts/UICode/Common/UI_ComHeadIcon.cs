/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_ComHeadIcon : GLabel
    {
        public Controller black;
        public GLoader bg;
        public GLoader txk;
        public GButton selectBtn;
        public const string URL = "ui://0anhreylg1nmxxrm";

        public static UI_ComHeadIcon CreateInstance()
        {
            return (UI_ComHeadIcon)UIPackage.CreateObject("Common", "ComHeadIcon");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            black = GetController("black");
            bg = (GLoader)GetChild("bg");
            txk = (GLoader)GetChild("txk");
            selectBtn = (GButton)GetChild("selectBtn");
        }
    }
}