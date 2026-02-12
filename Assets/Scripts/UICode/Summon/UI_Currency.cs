/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_Currency : GLabel
    {
        public GTextField txtValue;
        public const string URL = "ui://i7ojazuusurf4";

        public static UI_Currency CreateInstance()
        {
            return (UI_Currency)UIPackage.CreateObject("Summon", "Currency");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            txtValue = (GTextField)GetChild("txtValue");
        }
    }
}