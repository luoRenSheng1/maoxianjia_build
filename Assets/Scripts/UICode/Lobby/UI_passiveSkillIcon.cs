/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_passiveSkillIcon : GComponent
    {
        public GLoader skillIcon;
        public const string URL = "ui://s7x7ku0n9jppdxy4n";

        public static UI_passiveSkillIcon CreateInstance()
        {
            return (UI_passiveSkillIcon)UIPackage.CreateObject("Lobby", "passiveSkillIcon");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            skillIcon = (GLoader)GetChild("skillIcon");
        }
    }
}