/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_UpLvBuildConditItem : GLabel
    {
        public Controller reachCtrl;
        public Controller buildType;
        public GTextField indexLb;
        public GTextField needLb;
        public GTextField totalLb;
        public const string URL = "ui://5moj1x39pchvdxy4d";

        public static UI_UpLvBuildConditItem CreateInstance()
        {
            return (UI_UpLvBuildConditItem)UIPackage.CreateObject("CommonEx", "UpLvBuildConditItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            reachCtrl = GetController("reachCtrl");
            buildType = GetController("buildType");
            indexLb = (GTextField)GetChild("indexLb");
            needLb = (GTextField)GetChild("needLb");
            totalLb = (GTextField)GetChild("totalLb");
        }
    }
}