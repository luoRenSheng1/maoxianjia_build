/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_AthleticsItem : GComponent
    {
        public Controller status;
        public GLoader bg;
        public GButton goBtn;
        public GButton restBtn;
        public GTextField txdy;
        public GComponent reddot;
        public GTextField timeLb;
        public const string URL = "ui://zoxecbv2u5mt14";

        public static UI_AthleticsItem CreateInstance()
        {
            return (UI_AthleticsItem)UIPackage.CreateObject("PVPMap", "AthleticsItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            bg = (GLoader)GetChild("bg");
            goBtn = (GButton)GetChild("goBtn");
            restBtn = (GButton)GetChild("restBtn");
            txdy = (GTextField)GetChild("txdy");
            reddot = (GComponent)GetChild("reddot");
            timeLb = (GTextField)GetChild("timeLb");
        }
    }
}