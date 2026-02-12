/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_MineSelect : GComponent
    {
        public GImage select;
        public Transition t0;
        public const string URL = "ui://pdufy3keozcvih6";

        public static UI_MineSelect CreateInstance()
        {
            return (UI_MineSelect)UIPackage.CreateObject("BigMap", "MineSelect");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            select = (GImage)GetChild("select");
            t0 = GetTransition("t0");
        }
    }
}