/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_StageBigMap : GComponent
    {
        public UI_BigMap bgiMap;
        public GTextField txtTime;
        public const string URL = "ui://pdufy3kelht02";

        public static UI_StageBigMap CreateInstance()
        {
            return (UI_StageBigMap)UIPackage.CreateObject("BigMap", "StageBigMap");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bgiMap = (UI_BigMap)GetChild("bgiMap");
            txtTime = (GTextField)GetChild("txtTime");
        }
    }
}