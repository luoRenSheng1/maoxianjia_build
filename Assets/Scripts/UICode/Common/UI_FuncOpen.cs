/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_FuncOpen : GComponent
    {
        public GLoader img;
        public GList funcList;
        public Transition t0;
        public const string URL = "ui://0anhreylktw8dxy7c";

        public static UI_FuncOpen CreateInstance()
        {
            return (UI_FuncOpen)UIPackage.CreateObject("Common", "FuncOpen");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            img = (GLoader)GetChild("img");
            funcList = (GList)GetChild("funcList");
            t0 = GetTransition("t0");
        }
    }
}