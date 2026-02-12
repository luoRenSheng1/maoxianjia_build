/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Artifact
{
    public partial class UI_ArtifactAttrTip : GComponent
    {
        public GComponent frame;
        public GButton equipBtn;
        public GTextField name;
        public GTextField equipLv;
        public GLoader zlIcon;
        public GTextField value;
        public GList attrList;
        public const string URL = "ui://v9y2d69nkqmi0";

        public static UI_ArtifactAttrTip CreateInstance()
        {
            return (UI_ArtifactAttrTip)UIPackage.CreateObject("Artifact", "ArtifactAttrTip");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            equipBtn = (GButton)GetChild("equipBtn");
            name = (GTextField)GetChild("name");
            equipLv = (GTextField)GetChild("equipLv");
            zlIcon = (GLoader)GetChild("zlIcon");
            value = (GTextField)GetChild("value");
            attrList = (GList)GetChild("attrList");
        }
    }
}