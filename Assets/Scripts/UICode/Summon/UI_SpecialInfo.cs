/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SpecialInfo : GComponent
    {
        public GList specialList;
        public GLoader bannerBg;
        public const string URL = "ui://i7ojazuusurfa";

        public static UI_SpecialInfo CreateInstance()
        {
            return (UI_SpecialInfo)UIPackage.CreateObject("Summon", "SpecialInfo");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            specialList = (GList)GetChild("specialList");
            bannerBg = (GLoader)GetChild("bannerBg");
        }
    }
}