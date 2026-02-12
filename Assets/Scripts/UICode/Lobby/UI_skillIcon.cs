/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_skillIcon : GComponent
    {
        public GLoader skillIcon;
        public const string URL = "ui://s7x7ku0nhargdxy3b";

        public static UI_skillIcon CreateInstance()
        {
            return (UI_skillIcon)UIPackage.CreateObject("Lobby", "skillIcon");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            skillIcon = (GLoader)GetChild("skillIcon");
        }
    }
}