/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_MakeEquipBtnAni : GComponent
    {
        public UI_MakeEquipBtn makeBtn;
        public Transition t0;
        public const string URL = "ui://ddc23erlllb1dxybt";

        public static UI_MakeEquipBtnAni CreateInstance()
        {
            return (UI_MakeEquipBtnAni)UIPackage.CreateObject("Equip", "MakeEquipBtnAni");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            makeBtn = (UI_MakeEquipBtn)GetChild("makeBtn");
            t0 = GetTransition("t0");
        }
    }
}