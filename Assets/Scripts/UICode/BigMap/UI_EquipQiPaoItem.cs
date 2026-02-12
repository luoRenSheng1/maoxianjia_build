/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_EquipQiPaoItem : GComponent
    {
        public GLoader equipIcon;
        public GLoader3D qiPaoSpine;
        public const string URL = "ui://pdufy3ketdju68";

        public static UI_EquipQiPaoItem CreateInstance()
        {
            return (UI_EquipQiPaoItem)UIPackage.CreateObject("BigMap", "EquipQiPaoItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            equipIcon = (GLoader)GetChild("equipIcon");
            qiPaoSpine = (GLoader3D)GetChild("qiPaoSpine");
        }
    }
}