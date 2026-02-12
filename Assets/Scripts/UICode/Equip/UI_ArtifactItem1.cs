/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_ArtifactItem1 : GButton
    {
        public Controller qualityCtrl;
        public GLoader qualityIcon;
        public GLoader3D spineEff;
        public GLoader icon1;
        public GTextField lvLb;
        public const string URL = "ui://ddc23erlef8udxybc";

        public static UI_ArtifactItem1 CreateInstance()
        {
            return (UI_ArtifactItem1)UIPackage.CreateObject("Equip", "ArtifactItem1");
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