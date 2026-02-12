/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_CustomFlyAttack : GComponent
    {
        public Controller ctrlType;
        public GLoader tip;
        public Transition nomral;
        public const string URL = "ui://0anhreylnntddxy2y";

        public static UI_CustomFlyAttack CreateInstance()
        {
            return (UI_CustomFlyAttack)UIPackage.CreateObject("Common", "CustomFlyAttack");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrlType = GetController("ctrlType");
            tip = (GLoader)GetChild("tip");
            nomral = GetTransition("nomral");
        }
    }
}