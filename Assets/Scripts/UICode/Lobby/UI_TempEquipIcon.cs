/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_TempEquipIcon : GComponent
    {
        public GLoader tempEquipIcon;
        public const string URL = "ui://s7x7ku0nnc1udxy1o";

        public static UI_TempEquipIcon CreateInstance()
        {
            return (UI_TempEquipIcon)UIPackage.CreateObject("Lobby", "TempEquipIcon");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            tempEquipIcon = (GLoader)GetChild("tempEquipIcon");
        }
    }
}