/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_PMWindow : GComponent
    {
        public UI_FrameMask_Tips frame;
        public GButton closeBtn;
        public GTextInput pmIpt1;
        public GButton sendBtn1;
        public GTextInput pmIpt2;
        public GButton sendBtn2;
        public GTextInput pmIpt3;
        public GButton sendBtn3;
        public GTextInput pmIpt4;
        public GButton sendBtn4;
        public GTextInput pmIpt5;
        public GButton sendBtn5;
        public GTextInput pmIpt6;
        public GButton sendBtn6;
        public GTextInput pmIpt7;
        public GComboBox itemPop;
        public GButton sendBtn7;
        public GComboBox petPop;
        public GButton sendBtn8;
        public GTextInput pmIpt8;
        public GTextInput pmIptQ;
        public GComboBox itemEquip;
        public GButton sendBtn10;
        public GComboBox itemQuality;
        public GComboBox skillpop;
        public GButton sendBtn11;
        public GTextInput pmIpt11;
        public GTextInput pmIpt;
        public GButton sendBtn9;
        public GComboBox heroPop;
        public GButton sendBtn15;
        public GButton sendBtn12;
        public GButton sendBtn13;
        public GButton sendBtn14;
        public GButton sendBtn16;
        public GButton sendBtn17;
        public GButton sendBtn18;
        public const string URL = "ui://0anhreylot6fdxy2f";

        public static UI_PMWindow CreateInstance()
        {
            return (UI_PMWindow)UIPackage.CreateObject("Common", "PMWindow");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (UI_FrameMask_Tips)GetChild("frame");
            closeBtn = (GButton)GetChild("closeBtn");
            pmIpt1 = (GTextInput)GetChild("pmIpt1");
            sendBtn1 = (GButton)GetChild("sendBtn1");
            pmIpt2 = (GTextInput)GetChild("pmIpt2");
            sendBtn2 = (GButton)GetChild("sendBtn2");
            pmIpt3 = (GTextInput)GetChild("pmIpt3");
            sendBtn3 = (GButton)GetChild("sendBtn3");
            pmIpt4 = (GTextInput)GetChild("pmIpt4");
            sendBtn4 = (GButton)GetChild("sendBtn4");
            pmIpt5 = (GTextInput)GetChild("pmIpt5");
            sendBtn5 = (GButton)GetChild("sendBtn5");
            pmIpt6 = (GTextInput)GetChild("pmIpt6");
            sendBtn6 = (GButton)GetChild("sendBtn6");
            pmIpt7 = (GTextInput)GetChild("pmIpt7");
            itemPop = (GComboBox)GetChild("itemPop");
            sendBtn7 = (GButton)GetChild("sendBtn7");
            petPop = (GComboBox)GetChild("petPop");
            sendBtn8 = (GButton)GetChild("sendBtn8");
            pmIpt8 = (GTextInput)GetChild("pmIpt8");
            pmIptQ = (GTextInput)GetChild("pmIptQ");
            itemEquip = (GComboBox)GetChild("itemEquip");
            sendBtn10 = (GButton)GetChild("sendBtn10");
            itemQuality = (GComboBox)GetChild("itemQuality");
            skillpop = (GComboBox)GetChild("skillpop");
            sendBtn11 = (GButton)GetChild("sendBtn11");
            pmIpt11 = (GTextInput)GetChild("pmIpt11");
            pmIpt = (GTextInput)GetChild("pmIpt");
            sendBtn9 = (GButton)GetChild("sendBtn9");
            heroPop = (GComboBox)GetChild("heroPop");
            sendBtn15 = (GButton)GetChild("sendBtn15");
            sendBtn12 = (GButton)GetChild("sendBtn12");
            sendBtn13 = (GButton)GetChild("sendBtn13");
            sendBtn14 = (GButton)GetChild("sendBtn14");
            sendBtn16 = (GButton)GetChild("sendBtn16");
            sendBtn17 = (GButton)GetChild("sendBtn17");
            sendBtn18 = (GButton)GetChild("sendBtn18");
        }
    }
}