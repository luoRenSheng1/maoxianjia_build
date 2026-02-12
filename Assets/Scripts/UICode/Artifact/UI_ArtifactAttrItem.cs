/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Artifact
{
    public partial class UI_ArtifactAttrItem : GComponent
    {
        public GTextField curAttrName;
        public GTextField curAttr;
        public GTextField nextAttr;
        public const string URL = "ui://v9y2d69nkqmidxy9b";

        public static UI_ArtifactAttrItem CreateInstance()
        {
            return (UI_ArtifactAttrItem)UIPackage.CreateObject("Artifact", "ArtifactAttrItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            curAttrName = (GTextField)GetChild("curAttrName");
            curAttr = (GTextField)GetChild("curAttr");
            nextAttr = (GTextField)GetChild("nextAttr");
        }
    }
}