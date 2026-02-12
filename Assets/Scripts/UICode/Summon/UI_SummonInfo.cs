/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SummonInfo : GComponent
    {
        public UI_Shade summonListShade;
        public const string URL = "ui://i7ojazuusurf6";

        public static UI_SummonInfo CreateInstance()
        {
            return (UI_SummonInfo)UIPackage.CreateObject("Summon", "SummonInfo");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            summonListShade = (UI_Shade)GetChild("summonListShade");
        }
    }
}