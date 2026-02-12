/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Artifact
{
    public partial class UI_ArtifactAttrItem3 : GComponent
    {
        public GTextField attrName;
        public GTextField attrValue;
        public const string URL = "ui://v9y2d69nkqmi1";

        public static UI_ArtifactAttrItem3 CreateInstance()
        {
            return (UI_ArtifactAttrItem3)UIPackage.CreateObject("Artifact", "ArtifactAttrItem3");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            attrName = (GTextField)GetChild("attrName");
            attrValue = (GTextField)GetChild("attrValue");
        }
    }
}