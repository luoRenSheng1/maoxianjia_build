/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Artifact
{
    public partial class UI_ArtifactItem : GButton
    {
        public Controller qualityCtrl;
        public GLoader qualityIcon;
        public GLoader3D spineEff;
        public GLoader icon1;
        public GTextField lvLb;
        public const string URL = "ui://v9y2d69nhz5cdxy9w";

        public static UI_ArtifactItem CreateInstance()
        {
            return (UI_ArtifactItem)UIPackage.CreateObject("Artifact", "ArtifactItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            qualityCtrl = GetController("qualityCtrl");
            qualityIcon = (GLoader)GetChild("qualityIcon");
            spineEff = (GLoader3D)GetChild("spineEff");
            icon1 = (GLoader)GetChild("icon1");
            lvLb = (GTextField)GetChild("lvLb");
        }
    }
}