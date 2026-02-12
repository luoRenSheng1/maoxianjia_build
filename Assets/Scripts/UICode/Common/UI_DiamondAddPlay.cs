/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_DiamondAddPlay : GComponent
    {
        public UI_DiamondAdd node;
        public Transition t0;
        public const string URL = "ui://0anhreylqfmcdxyh7";

        public static UI_DiamondAddPlay CreateInstance()
        {
            return (UI_DiamondAddPlay)UIPackage.CreateObject("Common", "DiamondAddPlay");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            node = (UI_DiamondAdd)GetChild("node");
            t0 = GetTransition("t0");
        }
    }
}