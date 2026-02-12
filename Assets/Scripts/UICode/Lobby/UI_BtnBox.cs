/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_BtnBox : GComponent
    {
        public UI_BtnBoxKey btnBoxKey;
        public UI_BtnBoxIcon btnBoxIcon;
        public UI_BtnBoxLv btnBoxLv;
        public GComponent finger;
        public GGraph guidePos;
        public UI_BtnBoxLvUp btnBoxLvUp;
        public Transition t0;
        public const string URL = "ui://s7x7ku0npdpadxxz2";

        public static UI_BtnBox CreateInstance()
        {
            return (UI_BtnBox)UIPackage.CreateObject("Lobby", "BtnBox");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            btnBoxKey = (UI_BtnBoxKey)GetChild("btnBoxKey");
            btnBoxIcon = (UI_BtnBoxIcon)GetChild("btnBoxIcon");
            btnBoxLv = (UI_BtnBoxLv)GetChild("btnBoxLv");
            finger = (GComponent)GetChild("finger");
            guidePos = (GGraph)GetChild("guidePos");
            btnBoxLvUp = (UI_BtnBoxLvUp)GetChild("btnBoxLvUp");
            t0 = GetTransition("t0");
        }
    }
}