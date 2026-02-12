/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_ComBuff : GComponent
    {
        public GList buffList;
        public const string URL = "ui://s7x7ku0na2kwdxyau";

        public static UI_ComBuff CreateInstance()
        {
            return (UI_ComBuff)UIPackage.CreateObject("Lobby", "ComBuff");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            buffList = (GList)GetChild("buffList");
        }
    }
}