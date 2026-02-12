/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Achievement
{
    public partial class UI_ItemBtn : GButton
    {
        public GTextField num;
        public const string URL = "ui://b8bvql0irwnpj";

        public static UI_ItemBtn CreateInstance()
        {
            return (UI_ItemBtn)UIPackage.CreateObject("Achievement", "ItemBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            num = (GTextField)GetChild("num");
        }
    }
}