/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace TreasureLevelup
{
    public partial class UI_ComAttrItem : GLabel
    {
        public Controller lockCtrl;
        public Controller quality;
        public Controller ctrl;
        public GTextField curValue;
        public GTextField nextValue;
        public const string URL = "ui://v1wfpt3li9sl28";

        public static UI_ComAttrItem CreateInstance()
        {
            return (UI_ComAttrItem)UIPackage.CreateObject("TreasureLevelup", "ComAttrItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            lockCtrl = GetController("lockCtrl");
            quality = GetController("quality");
            ctrl = GetController("ctrl");
            curValue = (GTextField)GetChild("curValue");
            nextValue = (GTextField)GetChild("nextValue");
        }
    }
}