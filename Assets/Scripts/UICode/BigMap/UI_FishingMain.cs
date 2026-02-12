/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_FishingMain : GComponent
    {
        public UI_FishingBar fishingBar;
        public UI_FishingBtnAni fishingBtnAni;
        public const string URL = "ui://pdufy3keka12igj";

        public static UI_FishingMain CreateInstance()
        {
            return (UI_FishingMain)UIPackage.CreateObject("BigMap", "FishingMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            fishingBar = (UI_FishingBar)GetChild("fishingBar");
            fishingBtnAni = (UI_FishingBtnAni)GetChild("fishingBtnAni");
        }
    }
}