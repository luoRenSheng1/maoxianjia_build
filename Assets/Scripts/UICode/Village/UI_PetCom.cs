/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Village
{
    public partial class UI_PetCom : GButton
    {
        public Controller qualityCtrl;
        public Controller isStation;
        public Controller hasValue;
        public Controller buildType;
        public GLoader qualityIcon;
        public GLoader petIcon;
        public GTextField lvLb;
        public GTextField stationBuild;
        public GComponent redPoint;
        public const string URL = "ui://8glegefcoppew";

        public static UI_PetCom CreateInstance()
        {
            return (UI_PetCom)UIPackage.CreateObject("Village", "PetCom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            qualityCtrl = GetController("qualityCtrl");
            isStation = GetController("isStation");
            hasValue = GetController("hasValue");
            buildType = GetController("buildType");
            qualityIcon = (GLoader)GetChild("qualityIcon");
            petIcon = (GLoader)GetChild("petIcon");
            lvLb = (GTextField)GetChild("lvLb");
            stationBuild = (GTextField)GetChild("stationBuild");
            redPoint = (GComponent)GetChild("redPoint");
        }
    }
}