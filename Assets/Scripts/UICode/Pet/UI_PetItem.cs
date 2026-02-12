/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetItem : GButton
    {
        public Controller qualityCtrl;
        public Controller isUpload;
        public Controller lockCtrl;
        public GLoader qualityIcon;
        public GTextField petLv;
        public GComponent redPoint;
        public const string URL = "ui://lxs2h4ifhz5cdxy7c";

        public static UI_PetItem CreateInstance()
        {
            return (UI_PetItem)UIPackage.CreateObject("Pet", "PetItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            qualityCtrl = GetController("qualityCtrl");
            isUpload = GetController("isUpload");
            lockCtrl = GetController("lockCtrl");
            qualityIcon = (GLoader)GetChild("qualityIcon");
            petLv = (GTextField)GetChild("petLv");
            redPoint = (GComponent)GetChild("redPoint");
        }
    }
}