/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_RewardItemCom : GButton
    {
        public Controller disableCtrl;
        public Controller ctrlQuality;
        public Controller hasCount;
        public GTextField txtLv;
        public GLoader3D itemSpineEff;
        public const string URL = "ui://5moj1x39ogtkdxycq";

        public static UI_RewardItemCom CreateInstance()
        {
            return (UI_RewardItemCom)UIPackage.CreateObject("CommonEx", "RewardItemCom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            disableCtrl = GetController("disableCtrl");
            ctrlQuality = GetController("ctrlQuality");
            hasCount = GetController("hasCount");
            txtLv = (GTextField)GetChild("txtLv");
            itemSpineEff = (GLoader3D)GetChild("itemSpineEff");
        }
    }
}