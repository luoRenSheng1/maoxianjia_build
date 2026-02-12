/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_BossTime : GComponent
    {
        public GTextField title;
        public const string URL = "ui://0anhreyliv53dxyha";

        public static UI_BossTime CreateInstance()
        {
            return (UI_BossTime)UIPackage.CreateObject("Common", "BossTime");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            title = (GTextField)GetChild("title");
        }
    }
}