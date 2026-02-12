/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_ComRedPoint : GComponent
    {
        public Controller status;
        public GTextField txt_num;
        public Transition t0;
        public const string URL = "ui://0anhreyltomp1r";

        public static UI_ComRedPoint CreateInstance()
        {
            return (UI_ComRedPoint)UIPackage.CreateObject("Common", "ComRedPoint");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            txt_num = (GTextField)GetChild("txt_num");
            t0 = GetTransition("t0");
        }
    }
}