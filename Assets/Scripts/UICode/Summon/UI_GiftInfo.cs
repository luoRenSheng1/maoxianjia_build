/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_GiftInfo : GComponent
    {
        public UI_GiftList summonListShade;
        public GLoader bannerBg;
        public GGraph spine;
        public const string URL = "ui://i7ojazuuftc92l";

        public static UI_GiftInfo CreateInstance()
        {
            return (UI_GiftInfo)UIPackage.CreateObject("Summon", "GiftInfo");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            summonListShade = (UI_GiftList)GetChild("summonListShade");
            bannerBg = (GLoader)GetChild("bannerBg");
            spine = (GGraph)GetChild("spine");
        }
    }
}