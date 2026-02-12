/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Village
{
    public partial class UI_PetShow : GButton
    {
        public Controller hasPet;
        public Controller showHandCtrl;
        public GGraph spine;
        public GLabel petName;
        public GComponent qualityLb;
        public const string URL = "ui://8glegefcoppet";

        public static UI_PetShow CreateInstance()
        {
            return (UI_PetShow)UIPackage.CreateObject("Village", "PetShow");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            hasPet = GetController("hasPet");
            showHandCtrl = GetController("showHandCtrl");
            spine = (GGraph)GetChild("spine");
            petName = (GLabel)GetChild("petName");
            qualityLb = (GComponent)GetChild("qualityLb");
        }
    }
}