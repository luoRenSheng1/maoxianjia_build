/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_TalentSelect : GComponent
    {
        public GLoader bg;
        public GList btnList;
        public const string URL = "ui://m37flevdp9n0dxy94";

        public static UI_TalentSelect CreateInstance()
        {
            return (UI_TalentSelect)UIPackage.CreateObject("RoleMain", "TalentSelect");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bg = (GLoader)GetChild("bg");
            btnList = (GList)GetChild("btnList");
        }
    }
}