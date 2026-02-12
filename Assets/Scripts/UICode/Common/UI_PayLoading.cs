/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_PayLoading : GComponent
    {
        public Controller ctrl;
        public UI_LoadingCircleProgress loading;
        public Transition t0;
        public const string URL = "ui://0anhreylqz32dxy4p";

        public static UI_PayLoading CreateInstance()
        {
            return (UI_PayLoading)UIPackage.CreateObject("Common", "PayLoading");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            loading = (UI_LoadingCircleProgress)GetChild("loading");
            t0 = GetTransition("t0");
        }
    }
}