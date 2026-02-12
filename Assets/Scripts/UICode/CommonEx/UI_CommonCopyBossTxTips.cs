/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_CommonCopyBossTxTips : GComponent
    {
        public GTextField pContentName;
        public GButton bossAttr;
        public GRichTextField pContentDesc;
        public const string URL = "ui://5moj1x39llb1dxye5";

        public static UI_CommonCopyBossTxTips CreateInstance()
        {
            return (UI_CommonCopyBossTxTips)UIPackage.CreateObject("CommonEx", "CommonCopyBossTxTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            pContentName = (GTextField)GetChild("pContentName");
            bossAttr = (GButton)GetChild("bossAttr");
            pContentDesc = (GRichTextField)GetChild("pContentDesc");
        }
    }
}