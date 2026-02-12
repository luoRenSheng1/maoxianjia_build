/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_PassTaskPanel : GComponent
    {
        public GList passtaskList;
        public const string URL = "ui://2pcsnr2kr0ab1e";

        public static UI_PassTaskPanel CreateInstance()
        {
            return (UI_PassTaskPanel)UIPackage.CreateObject("Passport", "PassTaskPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            passtaskList = (GList)GetChild("passtaskList");
        }
    }
}