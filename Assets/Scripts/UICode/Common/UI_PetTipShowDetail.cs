/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_PetTipShowDetail : GComponent
    {
        public Controller gender;
        public Controller occupationType;
        public UI_FrameMask_Tips frame;
        public GTextField gjxsLb;
        public GTextField ljLb;
        public GTextField bjLb;
        public GTextField bsLb;
        public GTextField gsLb;
        public GTextField ljxsLb;
        public GTextField ctLb;
        public GTextField atkLb;
        public GLoader skillIcon;
        public GTextField skillName;
        public GTextField skillDesc;
        public GTextField petDesc;
        public GGraph petIcon;
        public GTextField petName;
        public const string URL = "ui://0anhreylreczdxy2p";

        public static UI_PetTipShowDetail CreateInstance()
        {
            return (UI_PetTipShowDetail)UIPackage.CreateObject("Common", "PetTipShowDetail");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            gender = GetController("gender");
            occupationType = GetController("occupationType");
            frame = (UI_FrameMask_Tips)GetChild("frame");
            gjxsLb = (GTextField)GetChild("gjxsLb");
            ljLb = (GTextField)GetChild("ljLb");
            bjLb = (GTextField)GetChild("bjLb");
            bsLb = (GTextField)GetChild("bsLb");
            gsLb = (GTextField)GetChild("gsLb");
            ljxsLb = (GTextField)GetChild("ljxsLb");
            ctLb = (GTextField)GetChild("ctLb");
            atkLb = (GTextField)GetChild("atkLb");
            skillIcon = (GLoader)GetChild("skillIcon");
            skillName = (GTextField)GetChild("skillName");
            skillDesc = (GTextField)GetChild("skillDesc");
            petDesc = (GTextField)GetChild("petDesc");
            petIcon = (GGraph)GetChild("petIcon");
            petName = (GTextField)GetChild("petName");
        }
    }
}