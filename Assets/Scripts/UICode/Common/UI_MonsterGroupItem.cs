/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_MonsterGroupItem : GComponent
    {
        public Controller type;
        public Controller full;
        public GProgressBar fullBar;
        public const string URL = "ui://0anhreyleceedxy1v";

        public static UI_MonsterGroupItem CreateInstance()
        {
            return (UI_MonsterGroupItem)UIPackage.CreateObject("Common", "MonsterGroupItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            type = GetController("type");
            full = GetController("full");
            fullBar = (GProgressBar)GetChild("fullBar");
        }
    }
}