/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_probablyItem : GComponent
    {
        public Controller qualityCtrl;
        public Controller ctrl;
        public GTextField proLb;
        public const string URL = "ui://i7ojazuuk4ck1h";

        public static UI_probablyItem CreateInstance()
        {
            return (UI_probablyItem)UIPackage.CreateObject("Summon", "probablyItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            qualityCtrl = GetController("qualityCtrl");
            ctrl = GetController("ctrl");
            proLb = (GTextField)GetChild("proLb");
        }
    }
}