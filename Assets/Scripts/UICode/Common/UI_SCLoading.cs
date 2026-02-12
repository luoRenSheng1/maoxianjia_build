/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_SCLoading : GComponent
    {
        public UI_LoadingCircleProgress loading;
        public Transition t0;
        public const string URL = "ui://0anhreyldnrxdxy4l";

        public static UI_SCLoading CreateInstance()
        {
            return (UI_SCLoading)UIPackage.CreateObject("Common", "SCLoading");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            loading = (UI_LoadingCircleProgress)GetChild("loading");
            t0 = GetTransition("t0");
        }
    }
}