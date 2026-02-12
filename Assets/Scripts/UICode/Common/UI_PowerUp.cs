/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_PowerUp : GComponent
    {
        public Controller superCtrl;
        public GLoader3D spineUI;
        public GLoader3D superUI;
        public GTextField fightValLb;
        public GTextField addValue;
        public Transition t0;
        public const string URL = "ui://0anhreylisqldxy3g";

        public static UI_PowerUp CreateInstance()
        {
            return (UI_PowerUp)UIPackage.CreateObject("Common", "PowerUp");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            superCtrl = GetController("superCtrl");
            spineUI = (GLoader3D)GetChild("spineUI");
            superUI = (GLoader3D)GetChild("superUI");
            fightValLb = (GTextField)GetChild("fightValLb");
            addValue = (GTextField)GetChild("addValue");
            t0 = GetTransition("t0");
        }
    }
}