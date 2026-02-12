/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_BtnIconSkillBig : GButton
    {
        public Controller ctrlAuto;
        public Transition t0;
        public const string URL = "ui://s7x7ku0nhqycdxy11";

        public static UI_BtnIconSkillBig CreateInstance()
        {
            return (UI_BtnIconSkillBig)UIPackage.CreateObject("Lobby", "BtnIconSkillBig");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrlAuto = GetController("ctrlAuto");
            t0 = GetTransition("t0");
        }
    }
}