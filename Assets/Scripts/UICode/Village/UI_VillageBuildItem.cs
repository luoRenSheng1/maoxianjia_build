/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Village
{
    public partial class UI_VillageBuildItem : GLabel
    {
        public Controller lockCtrl;
        public Controller redCtrl;
        public UI_BuildItem buildItem;
        public const string URL = "ui://8glegefcuobvf";

        public static UI_VillageBuildItem CreateInstance()
        {
            return (UI_VillageBuildItem)UIPackage.CreateObject("Village", "VillageBuildItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            lockCtrl = GetController("lockCtrl");
            redCtrl = GetController("redCtrl");
            buildItem = (UI_BuildItem)GetChild("buildItem");
        }
    }
}