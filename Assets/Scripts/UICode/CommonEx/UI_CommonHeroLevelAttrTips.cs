/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_CommonHeroLevelAttrTips : GComponent
    {
        public Controller lockCtrl;
        public GTextField pContent;
        public GTextField lockLb;
        public GLoader flagImg;
        public const string URL = "ui://5moj1x39ozj9dxy5f";

        public static UI_CommonHeroLevelAttrTips CreateInstance()
        {
            return (UI_CommonHeroLevelAttrTips)UIPackage.CreateObject("CommonEx", "CommonHeroLevelAttrTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            lockCtrl = GetController("lockCtrl");
            pContent = (GTextField)GetChild("pContent");
            lockLb = (GTextField)GetChild("lockLb");
            flagImg = (GLoader)GetChild("flagImg");
        }
    }
}