/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Village
{
    public partial class UI_PetInfo : GComponent
    {
        public Controller qualityCtrl;
        public Controller upOrDown;
        public GComponent frame;
        public UI_PetCom petCom;
        public GTextField petName;
        public GTextField petLv;
        public GProgressBar barExp;
        public GComponent petQ;
        public GTextField jcLb;
        public GButton downarrayBtn;
        public GButton uparrayBtn;
        public GButton replaceBtn;
        public GButton closeBtn;
        public const string URL = "ui://8glegefcoppey";

        public static UI_PetInfo CreateInstance()
        {
            return (UI_PetInfo)UIPackage.CreateObject("Village", "PetInfo");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            qualityCtrl = GetController("qualityCtrl");
            upOrDown = GetController("upOrDown");
            frame = (GComponent)GetChild("frame");
            petCom = (UI_PetCom)GetChild("petCom");
            petName = (GTextField)GetChild("petName");
            petLv = (GTextField)GetChild("petLv");
            barExp = (GProgressBar)GetChild("barExp");
            petQ = (GComponent)GetChild("petQ");
            jcLb = (GTextField)GetChild("jcLb");
            downarrayBtn = (GButton)GetChild("downarrayBtn");
            uparrayBtn = (GButton)GetChild("uparrayBtn");
            replaceBtn = (GButton)GetChild("replaceBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}