/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_RightTabBtn : GButton
    {
        public GComponent reddot;
        public const string URL = "ui://2pcsnr2kr0ab11";

        public static UI_RightTabBtn CreateInstance()
        {
            return (UI_RightTabBtn)UIPackage.CreateObject("Passport", "RightTabBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            reddot = (GComponent)GetChild("reddot");
        }
    }
}