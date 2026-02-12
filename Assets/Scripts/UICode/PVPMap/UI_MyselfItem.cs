/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_MyselfItem : GComponent
    {
        public Controller status;
        public Controller rankCtrl;
        public Controller numCtrl;
        public GLoader bg2;
        public GTextField num;
        public GLabel headIcon;
        public GTextField pName;
        public GTextField fightingCapacity;
        public GList rewardList;
        public GButton getBtn;
        public const string URL = "ui://zoxecbv2u5mt1u";

        public static UI_MyselfItem CreateInstance()
        {
            return (UI_MyselfItem)UIPackage.CreateObject("PVPMap", "MyselfItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            rankCtrl = GetController("rankCtrl");
            numCtrl = GetController("numCtrl");
            bg2 = (GLoader)GetChild("bg2");
            num = (GTextField)GetChild("num");
            headIcon = (GLabel)GetChild("headIcon");
            pName = (GTextField)GetChild("pName");
            fightingCapacity = (GTextField)GetChild("fightingCapacity");
            rewardList = (GList)GetChild("rewardList");
            getBtn = (GButton)GetChild("getBtn");
        }
    }
}