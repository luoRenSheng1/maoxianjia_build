/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_Main : GComponent
    {
        public Controller c1;
        public Controller showTask;
        public GComponent frame;
        public UI_Panel panel;
        public GGraph lvPos;
        public GButton userInfo;
        public UI_ComInfo comPandaInfo;
        public UI_ComBuff comBuff;
        public GLoader maskBG;
        public Transition showAni;
        public Transition hideAni;
        public const string URL = "ui://s7x7ku0noi4p3";

        public static UI_Main CreateInstance()
        {
            return (UI_Main)UIPackage.CreateObject("Lobby", "Main");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            c1 = GetController("c1");
            showTask = GetController("showTask");
            frame = (GComponent)GetChild("frame");
            panel = (UI_Panel)GetChild("panel");
            lvPos = (GGraph)GetChild("lvPos");
            userInfo = (GButton)GetChild("userInfo");
            comPandaInfo = (UI_ComInfo)GetChild("comPandaInfo");
            comBuff = (UI_ComBuff)GetChild("comBuff");
            maskBG = (GLoader)GetChild("maskBG");
            showAni = GetTransition("showAni");
            hideAni = GetTransition("hideAni");
        }
    }
}