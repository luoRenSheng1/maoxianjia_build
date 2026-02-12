/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SummonHeroHelp : GComponent
    {
        public GComponent frame;
        public GList heroIntroList;
        public GButton closeBtn;
        public const string URL = "ui://i7ojazuuu1cl1p";

        public static UI_SummonHeroHelp CreateInstance()
        {
            return (UI_SummonHeroHelp)UIPackage.CreateObject("Summon", "SummonHeroHelp");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            heroIntroList = (GList)GetChild("heroIntroList");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}