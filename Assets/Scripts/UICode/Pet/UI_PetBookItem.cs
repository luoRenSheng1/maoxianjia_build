/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetBookItem : GButton
    {
        public Controller lockCtrl;
        public Controller qualityCtrl;
        public Controller hasCount;
        public GLoader qualityIcon;
        public GTextField name;
        public GTextField txtLv;
        public GLoader3D spine;
        public GLoader selectIcon;
        public GComponent bookRedPoint;
        public Transition reacts;
        public const string URL = "ui://lxs2h4ifhz5cdxy7a";

        public static UI_PetBookItem CreateInstance()
        {
            return (UI_PetBookItem)UIPackage.CreateObject("Pet", "PetBookItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            lockCtrl = GetController("lockCtrl");
            qualityCtrl = GetController("qualityCtrl");
            hasCount = GetController("hasCount");
            qualityIcon = (GLoader)GetChild("qualityIcon");
            name = (GTextField)GetChild("name");
            txtLv = (GTextField)GetChild("txtLv");
            spine = (GLoader3D)GetChild("spine");
            selectIcon = (GLoader)GetChild("selectIcon");
            bookRedPoint = (GComponent)GetChild("bookRedPoint");
            reacts = GetTransition("reacts");
        }
    }
}