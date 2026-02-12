/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_modelObj : GComponent
    {
        public GLoader logo;
        public GLabel bubble;
        public GImage bg;
        public GGraph model;
        public GGraph btnClick;
        public GProgressBar bar;
        public GLoader3D petRun;
        public Transition objBgEffect;
        public const string URL = "ui://pdufy3kesshp5x";

        public static UI_modelObj CreateInstance()
        {
            return (UI_modelObj)UIPackage.CreateObject("BigMap", "modelObj");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            logo = (GLoader)GetChild("logo");
            bubble = (GLabel)GetChild("bubble");
            bg = (GImage)GetChild("bg");
            model = (GGraph)GetChild("model");
            btnClick = (GGraph)GetChild("btnClick");
            bar = (GProgressBar)GetChild("bar");
            petRun = (GLoader3D)GetChild("petRun");
            objBgEffect = GetTransition("objBgEffect");
        }
    }
}