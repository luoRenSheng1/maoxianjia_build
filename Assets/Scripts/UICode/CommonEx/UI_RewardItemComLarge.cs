/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_RewardItemComLarge : GButton
    {
        public Controller disableCtrl;
        public Controller ctrlQuality;
        public Controller hasCount;
        public UI_ItemCom itemCom;
        public const string URL = "ui://5moj1x39rjj2dxycz";

        public static UI_RewardItemComLarge CreateInstance()
        {
            return (UI_RewardItemComLarge)UIPackage.CreateObject("CommonEx", "RewardItemComLarge");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            disableCtrl = GetController("disableCtrl");
            ctrlQuality = GetController("ctrlQuality");
            hasCount = GetController("hasCount");
            itemCom = (UI_ItemCom)GetChild("itemCom");
        }
    }
}