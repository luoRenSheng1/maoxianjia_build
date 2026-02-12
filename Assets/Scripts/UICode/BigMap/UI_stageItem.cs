/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_stageItem : GLabel
    {
        public Controller nameType;
        public GImage bg;
        public GGraph model;
        public GImage titleBg;
        public GImage titleBg1;
        public GTextField title1;
        public GImage titleBg2;
        public GTextField title2;
        public GImage titleBg3;
        public GTextField title3;
        public GLoader huntingTaskFlagIcon;
        public Transition bgEffect;
        public const string URL = "ui://pdufy3kehbptidt";

        public static UI_stageItem CreateInstance()
        {
            return (UI_stageItem)UIPackage.CreateObject("BigMap", "stageItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            nameType = GetController("nameType");
            bg = (GImage)GetChild("bg");
            model = (GGraph)GetChild("model");
            titleBg = (GImage)GetChild("titleBg");
            titleBg1 = (GImage)GetChild("titleBg1");
            title1 = (GTextField)GetChild("title1");
            titleBg2 = (GImage)GetChild("titleBg2");
            title2 = (GTextField)GetChild("title2");
            titleBg3 = (GImage)GetChild("titleBg3");
            title3 = (GTextField)GetChild("title3");
            huntingTaskFlagIcon = (GLoader)GetChild("huntingTaskFlagIcon");
            bgEffect = GetTransition("bgEffect");
        }
    }
}