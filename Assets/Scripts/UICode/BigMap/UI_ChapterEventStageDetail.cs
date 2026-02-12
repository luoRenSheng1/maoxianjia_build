/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_ChapterEventStageDetail : GComponent
    {
        public Controller type;
        public GComponent frame;
        public GButton Btn3;
        public GButton Btn1;
        public GButton Btn2;
        public GGroup twoBtns;
        public GLoader icon;
        public GLoader petIcon;
        public GTextField name;
        public GTextField desc;
        public GButton btnFight;
        public GTextField txtBtn;
        public GGroup oneBtns;
        public const string URL = "ui://pdufy3kep9n01p";

        public static UI_ChapterEventStageDetail CreateInstance()
        {
            return (UI_ChapterEventStageDetail)UIPackage.CreateObject("BigMap", "ChapterEventStageDetail");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            type = GetController("type");
            frame = (GComponent)GetChild("frame");
            Btn3 = (GButton)GetChild("Btn3");
            Btn1 = (GButton)GetChild("Btn1");
            Btn2 = (GButton)GetChild("Btn2");
            twoBtns = (GGroup)GetChild("twoBtns");
            icon = (GLoader)GetChild("icon");
            petIcon = (GLoader)GetChild("petIcon");
            name = (GTextField)GetChild("name");
            desc = (GTextField)GetChild("desc");
            btnFight = (GButton)GetChild("btnFight");
            txtBtn = (GTextField)GetChild("txtBtn");
            oneBtns = (GGroup)GetChild("oneBtns");
        }
    }
}