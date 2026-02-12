/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_PopupEquipAttribute : GComponent
    {
        public GComponent frame;
        public GTextField name;
        public GTextField equipLv;
        public GComponent comEquipItem2;
        public GTextField fightLb2;
        public GList mainAttrList;
        public GList attrList;
        public const string URL = "ui://ddc23erlqbcg15";

        public static UI_PopupEquipAttribute CreateInstance()
        {
            return (UI_PopupEquipAttribute)UIPackage.CreateObject("Equip", "PopupEquipAttribute");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            name = (GTextField)GetChild("name");
            equipLv = (GTextField)GetChild("equipLv");
            comEquipItem2 = (GComponent)GetChild("comEquipItem2");
            fightLb2 = (GTextField)GetChild("fightLb2");
            mainAttrList = (GList)GetChild("mainAttrList");
            attrList = (GList)GetChild("attrList");
        }
    }
}