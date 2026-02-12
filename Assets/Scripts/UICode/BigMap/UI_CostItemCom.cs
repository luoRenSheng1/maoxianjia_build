/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_CostItemCom : GComponent
    {
        public Controller stautsCtrl;
        public GLoader icon;
        public GTextField num;
        public const string URL = "ui://pdufy3ketdju5k";

        public static UI_CostItemCom CreateInstance()
        {
            return (UI_CostItemCom)UIPackage.CreateObject("BigMap", "CostItemCom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            stautsCtrl = GetController("stautsCtrl");
            icon = (GLoader)GetChild("icon");
            num = (GTextField)GetChild("num");
        }
    }
}