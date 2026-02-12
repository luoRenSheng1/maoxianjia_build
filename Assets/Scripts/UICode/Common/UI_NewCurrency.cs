/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_NewCurrency : GLabel
    {
        public GTextField txtValue;
        public const string URL = "ui://0anhreyltdjudxyh8";

        public static UI_NewCurrency CreateInstance()
        {
            return (UI_NewCurrency)UIPackage.CreateObject("Common", "NewCurrency");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            txtValue = (GTextField)GetChild("txtValue");
        }
    }
}