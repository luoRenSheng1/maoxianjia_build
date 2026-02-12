/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_RuinBtn : GButton
    {
        public Transition reacts;
        public const string URL = "ui://pdufy3kew224idi";

        public static UI_RuinBtn CreateInstance()
        {
            return (UI_RuinBtn)UIPackage.CreateObject("BigMap", "RuinBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            reacts = GetTransition("reacts");
        }
    }
}