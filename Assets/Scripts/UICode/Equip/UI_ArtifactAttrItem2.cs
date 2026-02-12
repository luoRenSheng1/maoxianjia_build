/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_ArtifactAttrItem2 : GComponent
    {
        public GTextField name;
        public GTextField num;
        public const string URL = "ui://ddc23erlef8udxyb6";

        public static UI_ArtifactAttrItem2 CreateInstance()
        {
            return (UI_ArtifactAttrItem2)UIPackage.CreateObject("Equip", "ArtifactAttrItem2");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            name = (GTextField)GetChild("name");
            num = (GTextField)GetChild("num");
        }
    }
}