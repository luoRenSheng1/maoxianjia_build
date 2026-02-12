/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_PetItemEx : GButton
    {
        public Controller ctrlQuality;
        public Controller disableCtrl;
        public Controller uploadCtrl;
        public Controller levelUp;
        public Controller type;
        public GTextField lvLb;
        public UI_BarExp numBar;
        public UI_PetBar numBar2;
        public const string URL = "ui://5moj1x39iqvadxy67";

        public static UI_PetItemEx CreateInstance()
        {
            return (UI_PetItemEx)UIPackage.CreateObject("CommonEx", "PetItemEx");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrlQuality = GetController("ctrlQuality");
            disableCtrl = GetController("disableCtrl");
            uploadCtrl = GetController("uploadCtrl");
            levelUp = GetController("levelUp");
            type = GetController("type");
            lvLb = (GTextField)GetChild("lvLb");
            numBar = (UI_BarExp)GetChild("numBar");
            numBar2 = (UI_PetBar)GetChild("numBar2");
        }
    }
}