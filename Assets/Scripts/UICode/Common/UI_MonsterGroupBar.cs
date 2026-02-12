/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_MonsterGroupBar : GComponent
    {
        public Controller ctrlStatus;
        public GList monsterGroupList;
        public const string URL = "ui://0anhreyleceedxy1u";

        public static UI_MonsterGroupBar CreateInstance()
        {
            return (UI_MonsterGroupBar)UIPackage.CreateObject("Common", "MonsterGroupBar");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrlStatus = GetController("ctrlStatus");
            monsterGroupList = (GList)GetChild("monsterGroupList");
        }
    }
}