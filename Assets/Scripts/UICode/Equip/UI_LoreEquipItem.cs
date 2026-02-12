/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_LoreEquipItem : GButton
    {
        public Controller qualityCtrl;
        public Controller ctrbg;
        public Controller hasCnt;
        public GLoader qualityIcon;
        public GTextField lvLb;
        public GComponent loreRedPoint;
        public const string URL = "ui://ddc23erlef8udxyaw";

        public static UI_LoreEquipItem CreateInstance()
        {
            return (UI_LoreEquipItem)UIPackage.CreateObject("Equip", "LoreEquipItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            qualityCtrl = GetController("qualityCtrl");
            ctrbg = GetController("ctrbg");
            hasCnt = GetController("hasCnt");
            qualityIcon = (GLoader)GetChild("qualityIcon");
            lvLb = (GTextField)GetChild("lvLb");
            loreRedPoint = (GComponent)GetChild("loreRedPoint");
        }
    }
}