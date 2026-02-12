/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_PetMain : GComponent
    {
        public GLoader petBg;
        public GLoader rlLoader;
        public GList petAllList;
        public GComponent petUpItem2;
        public GComponent petUpItem3;
        public GComponent petUpItem4;
        public GComponent petUpItem0;
        public GComponent petUpItem1;
        public GGroup upGroup;
        public GButton allStrengthBtn;
        public GTextField atkLb;
        public GTextField hpLb;
        public GComponent redDot;
        public const string URL = "ui://m37flevdozj926";

        public static UI_PetMain CreateInstance()
        {
            return (UI_PetMain)UIPackage.CreateObject("RoleMain", "PetMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            petBg = (GLoader)GetChild("petBg");
            rlLoader = (GLoader)GetChild("rlLoader");
            petAllList = (GList)GetChild("petAllList");
            petUpItem2 = (GComponent)GetChild("petUpItem2");
            petUpItem3 = (GComponent)GetChild("petUpItem3");
            petUpItem4 = (GComponent)GetChild("petUpItem4");
            petUpItem0 = (GComponent)GetChild("petUpItem0");
            petUpItem1 = (GComponent)GetChild("petUpItem1");
            upGroup = (GGroup)GetChild("upGroup");
            allStrengthBtn = (GButton)GetChild("allStrengthBtn");
            atkLb = (GTextField)GetChild("atkLb");
            hpLb = (GTextField)GetChild("hpLb");
            redDot = (GComponent)GetChild("redDot");
        }
    }
}