/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_CardItem : GComponent
    {
        public Controller typeCtrl;
        public GButton item;
        public GTextField nameLb;
        public const string URL = "ui://i7ojazuuq9cg19";

        public static UI_CardItem CreateInstance()
        {
            return (UI_CardItem)UIPackage.CreateObject("Summon", "CardItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeCtrl = GetController("typeCtrl");
            item = (GButton)GetChild("item");
            nameLb = (GTextField)GetChild("nameLb");
        }
    }
}