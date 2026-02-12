/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RuneDetailItem : GComponent
    {
        public Controller ctrl;
        public Controller quality;
        public GTextField runeName;
        public GTextField runeDesc;
        public const string URL = "ui://m37flevdeau0dxy6x";

        public static UI_RuneDetailItem CreateInstance()
        {
            return (UI_RuneDetailItem)UIPackage.CreateObject("RoleMain", "RuneDetailItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            quality = GetController("quality");
            runeName = (GTextField)GetChild("runeName");
            runeDesc = (GTextField)GetChild("runeDesc");
        }
    }
}