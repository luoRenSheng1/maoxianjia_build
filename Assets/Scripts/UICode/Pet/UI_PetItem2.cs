/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetItem2 : GButton
    {
        public Controller qualityCtrl;
        public GLoader qualityIcon;
        public GComponent redPoint;
        public const string URL = "ui://lxs2h4iff34jdxy9b";

        public static UI_PetItem2 CreateInstance()
        {
            return (UI_PetItem2)UIPackage.CreateObject("Pet", "PetItem2");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            qualityCtrl = GetController("qualityCtrl");
            qualityIcon = (GLoader)GetChild("qualityIcon");
            redPoint = (GComponent)GetChild("redPoint");
        }
    }
}