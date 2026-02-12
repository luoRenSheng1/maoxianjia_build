/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_GoldAddPlay : GComponent
    {
        public UI_GoldAdd node;
        public Transition t0;
        public const string URL = "ui://0anhreylqfmcdxyh5";

        public static UI_GoldAddPlay CreateInstance()
        {
            return (UI_GoldAddPlay)UIPackage.CreateObject("Common", "GoldAddPlay");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            node = (UI_GoldAdd)GetChild("node");
            t0 = GetTransition("t0");
        }
    }
}