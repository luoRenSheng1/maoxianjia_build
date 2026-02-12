/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lottery
{
    public partial class UI_boxItem : GButton
    {
        public Controller getCtrl;
        public GImage boxIcon;
        public GTextField valueLb;
        public Transition t0;
        public const string URL = "ui://6izp804wju03v";

        public static UI_boxItem CreateInstance()
        {
            return (UI_boxItem)UIPackage.CreateObject("Lottery", "boxItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            getCtrl = GetController("getCtrl");
            boxIcon = (GImage)GetChild("boxIcon");
            valueLb = (GTextField)GetChild("valueLb");
            t0 = GetTransition("t0");
        }
    }
}