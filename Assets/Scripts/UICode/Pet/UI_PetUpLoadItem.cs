/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetUpLoadItem : GComponent
    {
        public Controller state;
        public Controller showHandCtrl;
        public GGraph spine;
        public GComponent redDot;
        public UI_PetQualityLb petQuality;
        public GTextField petName;
        public const string URL = "ui://lxs2h4ifoo1wp";

        public static UI_PetUpLoadItem CreateInstance()
        {
            return (UI_PetUpLoadItem)UIPackage.CreateObject("Pet", "PetUpLoadItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            state = GetController("state");
            showHandCtrl = GetController("showHandCtrl");
            spine = (GGraph)GetChild("spine");
            redDot = (GComponent)GetChild("redDot");
            petQuality = (UI_PetQualityLb)GetChild("petQuality");
            petName = (GTextField)GetChild("petName");
        }
    }
}