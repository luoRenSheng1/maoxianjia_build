/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_ItemCom : GButton
    {
        public Controller ctrlQuality;
        public Controller hasCtrl;
        public Controller hasCount;
        public Controller disableCtrl;
        public GTextField txtLv;
        public GLoader3D itemSpineEff;
        public const string URL = "ui://5moj1x39ewjfdxy22";

        public static UI_ItemCom CreateInstance()
        {
            return (UI_ItemCom)UIPackage.CreateObject("CommonEx", "ItemCom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrlQuality = GetController("ctrlQuality");
            hasCtrl = GetController("hasCtrl");
            hasCount = GetController("hasCount");
            disableCtrl = GetController("disableCtrl");
            txtLv = (GTextField)GetChild("txtLv");
            itemSpineEff = (GLoader3D)GetChild("itemSpineEff");
        }
    }
}