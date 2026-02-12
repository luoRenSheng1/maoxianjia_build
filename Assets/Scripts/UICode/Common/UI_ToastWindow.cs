/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_ToastWindow : GComponent
    {
        public Controller ctrlMode;
        public UI_ToastCom tips;
        public UI_ToastIconCom iconTips;
        public Transition t0;
        public Transition iconAni;
        public const string URL = "ui://0anhreyltomp1q";

        public static UI_ToastWindow CreateInstance()
        {
            return (UI_ToastWindow)UIPackage.CreateObject("Common", "ToastWindow");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrlMode = GetController("ctrlMode");
            tips = (UI_ToastCom)GetChild("tips");
            iconTips = (UI_ToastIconCom)GetChild("iconTips");
            t0 = GetTransition("t0");
            iconAni = GetTransition("iconAni");
        }
    }
}