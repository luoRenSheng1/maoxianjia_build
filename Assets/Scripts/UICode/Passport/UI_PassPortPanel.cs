/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_PassPortPanel : GComponent
    {
        public GList passportList;
        public GImage suo;
        public const string URL = "ui://2pcsnr2kr0ab1d";

        public static UI_PassPortPanel CreateInstance()
        {
            return (UI_PassPortPanel)UIPackage.CreateObject("Passport", "PassPortPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            passportList = (GList)GetChild("passportList");
            suo = (GImage)GetChild("suo");
        }
    }
}