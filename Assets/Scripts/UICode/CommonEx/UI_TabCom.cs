/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_TabCom : GComponent
    {
        public Controller type;
        public Controller status;
        public GLoader icon;
        public GTextField num1;
        public GTextField num2;
        public const string URL = "ui://5moj1x39kqmidxy8g";

        public static UI_TabCom CreateInstance()
        {
            return (UI_TabCom)UIPackage.CreateObject("CommonEx", "TabCom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            type = GetController("type");
            status = GetController("status");
            icon = (GLoader)GetChild("icon");
            num1 = (GTextField)GetChild("num1");
            num2 = (GTextField)GetChild("num2");
        }
    }
}