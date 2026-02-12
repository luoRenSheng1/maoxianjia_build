/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_LeftTabBtn : GButton
    {
        public GComponent reddot;
        public const string URL = "ui://2pcsnr2kr0abz";

        public static UI_LeftTabBtn CreateInstance()
        {
            return (UI_LeftTabBtn)UIPackage.CreateObject("Passport", "LeftTabBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            reddot = (GComponent)GetChild("reddot");
        }
    }
}