/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_BuyFightTimes : GComponent
    {
        public GComponent frame;
        public GTextInput cntLb;
        public GButton reduceBtn;
        public GButton addBtn;
        public GTextField itemNum;
        public GButton moneyBtn;
        public GButton closeBtn;
        public const string URL = "ui://zoxecbv2eh4f30";

        public static UI_BuyFightTimes CreateInstance()
        {
            return (UI_BuyFightTimes)UIPackage.CreateObject("PVPMap", "BuyFightTimes");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            cntLb = (GTextInput)GetChild("cntLb");
            reduceBtn = (GButton)GetChild("reduceBtn");
            addBtn = (GButton)GetChild("addBtn");
            itemNum = (GTextField)GetChild("itemNum");
            moneyBtn = (GButton)GetChild("moneyBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}