/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_TaskBarExp : GProgressBar
    {
        public Controller maxCtrl;
        public Controller barType;
        public GTextField maxLevel;
        public const string URL = "ui://s7x7ku0nllb1dxycy";

        public static UI_TaskBarExp CreateInstance()
        {
            return (UI_TaskBarExp)UIPackage.CreateObject("Lobby", "TaskBarExp");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            maxCtrl = GetController("maxCtrl");
            barType = GetController("barType");
            maxLevel = (GTextField)GetChild("maxLevel");
        }
    }
}