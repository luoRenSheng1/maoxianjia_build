/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_AttrUpItem2 : GComponent
    {
        public GLoader icon;
        public GTextField desc;
        public GTextField num;
        public GTextField time;
        public const string URL = "ui://0anhreylsyvcdxy8q";

        public static UI_AttrUpItem2 CreateInstance()
        {
            return (UI_AttrUpItem2)UIPackage.CreateObject("Common", "AttrUpItem2");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            icon = (GLoader)GetChild("icon");
            desc = (GTextField)GetChild("desc");
            num = (GTextField)GetChild("num");
            time = (GTextField)GetChild("time");
        }
    }
}