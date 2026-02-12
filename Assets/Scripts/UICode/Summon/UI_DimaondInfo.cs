/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_DimaondInfo : GComponent
    {
        public GLoader bannerBg;
        public GGraph spine;
        public GList diamondList;
        public const string URL = "ui://i7ojazuup3wt2p";

        public static UI_DimaondInfo CreateInstance()
        {
            return (UI_DimaondInfo)UIPackage.CreateObject("Summon", "DimaondInfo");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bannerBg = (GLoader)GetChild("bannerBg");
            spine = (GGraph)GetChild("spine");
            diamondList = (GList)GetChild("diamondList");
        }
    }
}