/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_EquipSplitEffect : GComponent
    {
        public GLoader3D splitSpine;
        public const string URL = "ui://s7x7ku0nc90sdxy1m";

        public static UI_EquipSplitEffect CreateInstance()
        {
            return (UI_EquipSplitEffect)UIPackage.CreateObject("Lobby", "EquipSplitEffect");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            splitSpine = (GLoader3D)GetChild("splitSpine");
        }
    }
}