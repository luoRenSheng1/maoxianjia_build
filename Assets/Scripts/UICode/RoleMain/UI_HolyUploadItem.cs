/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_HolyUploadItem : GComponent
    {
        public Controller status;
        public Controller isSelect;
        public GLoader icon;
        public GComponent redPoint;
        public GLoader guide;
        public const string URL = "ui://m37flevda2kwdxyb2";

        public static UI_HolyUploadItem CreateInstance()
        {
            return (UI_HolyUploadItem)UIPackage.CreateObject("RoleMain", "HolyUploadItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            isSelect = GetController("isSelect");
            icon = (GLoader)GetChild("icon");
            redPoint = (GComponent)GetChild("redPoint");
            guide = (GLoader)GetChild("guide");
        }
    }
}