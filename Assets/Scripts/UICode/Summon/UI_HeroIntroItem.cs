/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_HeroIntroItem : GComponent
    {
        public Controller ctrl;
        public GTextField desc;
        public const string URL = "ui://i7ojazuul5cy3p";

        public static UI_HeroIntroItem CreateInstance()
        {
            return (UI_HeroIntroItem)UIPackage.CreateObject("Summon", "HeroIntroItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            desc = (GTextField)GetChild("desc");
        }
    }
}