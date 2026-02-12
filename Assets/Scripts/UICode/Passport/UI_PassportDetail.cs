/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_PassportDetail : GComponent
    {
        public GComponent frame;
        public UI_AdvanccBuyBtn buyBtn;
        public GButton closeBtn;
        public const string URL = "ui://2pcsnr2kauqc1h";

        public static UI_PassportDetail CreateInstance()
        {
            return (UI_PassportDetail)UIPackage.CreateObject("Passport", "PassportDetail");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            buyBtn = (UI_AdvanccBuyBtn)GetChild("buyBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}