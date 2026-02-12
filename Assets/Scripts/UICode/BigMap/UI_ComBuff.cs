/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_ComBuff : GComponent
    {
        public GList buffList;
        public const string URL = "ui://pdufy3kex2p02e";

        public static UI_ComBuff CreateInstance()
        {
            return (UI_ComBuff)UIPackage.CreateObject("BigMap", "ComBuff");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            buffList = (GList)GetChild("buffList");
        }
    }
}