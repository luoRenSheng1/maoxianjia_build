/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_ArtifactAttrItem : GComponent
    {
        public GTextField curAttr;
        public GTextField nextAttr;
        public const string URL = "ui://ddc23erlef8udxyb4";

        public static UI_ArtifactAttrItem CreateInstance()
        {
            return (UI_ArtifactAttrItem)UIPackage.CreateObject("Equip", "ArtifactAttrItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            curAttr = (GTextField)GetChild("curAttr");
            nextAttr = (GTextField)GetChild("nextAttr");
        }
    }
}