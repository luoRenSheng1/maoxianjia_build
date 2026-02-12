/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_GoldRain : GComponent
    {
        public GImage yin;
        public GImage TW;
        public GGroup gold;
        public GMovieClip guang;
        public GMovieClip fei;
        public GImage yin2;
        public Transition t0;
        public const string URL = "ui://pdufy3keq32eic9";

        public static UI_GoldRain CreateInstance()
        {
            return (UI_GoldRain)UIPackage.CreateObject("BigMap", "GoldRain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            yin = (GImage)GetChild("yin");
            TW = (GImage)GetChild("TW");
            gold = (GGroup)GetChild("gold");
            guang = (GMovieClip)GetChild("guang");
            fei = (GMovieClip)GetChild("fei");
            yin2 = (GImage)GetChild("yin2");
            t0 = GetTransition("t0");
        }
    }
}