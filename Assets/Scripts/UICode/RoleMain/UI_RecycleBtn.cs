/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RecycleBtn : GButton
    {
        public GTextField recyleMoney;
        public const string URL = "ui://m37flevdph24dxy6z";

        public static UI_RecycleBtn CreateInstance()
        {
            return (UI_RecycleBtn)UIPackage.CreateObject("RoleMain", "RecycleBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            recyleMoney = (GTextField)GetChild("recyleMoney");
        }
    }
}