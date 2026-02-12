/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Village
{
    public partial class UI_BuildInfo : GComponent
    {
        public Controller titleCtrl;
        public Controller hasReward;
        public Controller showPetHand;
        public GComponent frame;
        public GLoader bg;
        public GLoader3D buildSpine;
        public GTextField title;
        public GTextField addValue;
        public GButton itemBtn;
        public GTextField capacityValue;
        public GTextField hx;
        public GTextField timeLb;
        public GButton helpBtn;
        public UI_PetShow petUp0;
        public UI_PetShow petUp1;
        public UI_PetShow petUp2;
        public GGroup upGroup;
        public GList petList2;
        public GGroup rloader;
        public GButton rewardBtn;
        public GTextField rewardValue;
        public GButton getBtn;
        public GComponent redDot;
        public GGraph hideUpload1;
        public GGraph hideUpload2;
        public GButton closeBtn;
        public const string URL = "ui://8glegefcnif6i";

        public static UI_BuildInfo CreateInstance()
        {
            return (UI_BuildInfo)UIPackage.CreateObject("Village", "BuildInfo");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            titleCtrl = GetController("titleCtrl");
            hasReward = GetController("hasReward");
            showPetHand = GetController("showPetHand");
            frame = (GComponent)GetChild("frame");
            bg = (GLoader)GetChild("bg");
            buildSpine = (GLoader3D)GetChild("buildSpine");
            title = (GTextField)GetChild("title");
            addValue = (GTextField)GetChild("addValue");
            itemBtn = (GButton)GetChild("itemBtn");
            capacityValue = (GTextField)GetChild("capacityValue");
            hx = (GTextField)GetChild("hx");
            timeLb = (GTextField)GetChild("timeLb");
            helpBtn = (GButton)GetChild("helpBtn");
            petUp0 = (UI_PetShow)GetChild("petUp0");
            petUp1 = (UI_PetShow)GetChild("petUp1");
            petUp2 = (UI_PetShow)GetChild("petUp2");
            upGroup = (GGroup)GetChild("upGroup");
            petList2 = (GList)GetChild("petList2");
            rloader = (GGroup)GetChild("rloader");
            rewardBtn = (GButton)GetChild("rewardBtn");
            rewardValue = (GTextField)GetChild("rewardValue");
            getBtn = (GButton)GetChild("getBtn");
            redDot = (GComponent)GetChild("redDot");
            hideUpload1 = (GGraph)GetChild("hideUpload1");
            hideUpload2 = (GGraph)GetChild("hideUpload2");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}