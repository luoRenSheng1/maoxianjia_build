/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_SubmitGoldAndDiaMain : GComponent
    {
        public Controller typeCtrl;
        public GComponent frame;
        public GTextField desc;
        public GButton giveUpBtn;
        public GButton submitBtn;
        public GButton closeBtn;
        public const string URL = "ui://pdufy3keob1sid5";

        public static UI_SubmitGoldAndDiaMain CreateInstance()
        {
            return (UI_SubmitGoldAndDiaMain)UIPackage.CreateObject("BigMap", "SubmitGoldAndDiaMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeCtrl = GetController("typeCtrl");
            frame = (GComponent)GetChild("frame");
            desc = (GTextField)GetChild("desc");
            giveUpBtn = (GButton)GetChild("giveUpBtn");
            submitBtn = (GButton)GetChild("submitBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}