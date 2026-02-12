/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Tujian
{
    public partial class UI_TujianMain : GComponent
    {
        public GList tujianList;
        public GButton closeBtn;
        public const string URL = "ui://yjhb9afbr0wot";

        public static UI_TujianMain CreateInstance()
        {
            return (UI_TujianMain)UIPackage.CreateObject("Tujian", "TujianMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            tujianList = (GList)GetChild("tujianList");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}