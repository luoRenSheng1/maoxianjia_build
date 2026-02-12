/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_LoadingUI : GComponent
    {
        public Controller showType;
        public GLoader bg;
        public GProgressBar pro_Loading;
        public GTextField txtTips;
        public UI_LoadingCircleProgress txtValue;
        public const string URL = "ui://0anhreylgeqby7";

        public static UI_LoadingUI CreateInstance()
        {
            return (UI_LoadingUI)UIPackage.CreateObject("Common", "LoadingUI");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            showType = GetController("showType");
            bg = (GLoader)GetChild("bg");
            pro_Loading = (GProgressBar)GetChild("pro_Loading");
            txtTips = (GTextField)GetChild("txtTips");
            txtValue = (UI_LoadingCircleProgress)GetChild("txtValue");
        }
    }
}