/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_fkTabBtn : GButton
    {
        public Controller ctrlTitle;
        public GImage bg;
        public GTextField title1;
        public GTextField title2;
        public GComponent reddot;
        public const string URL = "ui://nmzfxo89plio12";

        public static UI_fkTabBtn CreateInstance()
        {
            return (UI_fkTabBtn)UIPackage.CreateObject("Shop", "fkTabBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrlTitle = GetController("ctrlTitle");
            bg = (GImage)GetChild("bg");
            title1 = (GTextField)GetChild("title1");
            title2 = (GTextField)GetChild("title2");
            reddot = (GComponent)GetChild("reddot");
        }
    }
}