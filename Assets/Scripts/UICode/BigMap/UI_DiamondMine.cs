/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_DiamondMine : GComponent
    {
        public GLoader icon;
        public GLoader3D touch;
        public GProgressBar bar;
        public const string URL = "ui://pdufy3ke100ni5v";

        public static UI_DiamondMine CreateInstance()
        {
            return (UI_DiamondMine)UIPackage.CreateObject("BigMap", "DiamondMine");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            icon = (GLoader)GetChild("icon");
            touch = (GLoader3D)GetChild("touch");
            bar = (GProgressBar)GetChild("bar");
        }
    }
}