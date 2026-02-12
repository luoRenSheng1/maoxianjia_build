/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_RuinItem : GComponent
    {
        public Controller isSelected;
        public UI_RuinBtn ruinBtn;
        public GTextField name;
        public GTextField ruinTime;
        public GList attrList;
        public const string URL = "ui://pdufy3kew224id7";

        public static UI_RuinItem CreateInstance()
        {
            return (UI_RuinItem)UIPackage.CreateObject("BigMap", "RuinItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            isSelected = GetController("isSelected");
            ruinBtn = (UI_RuinBtn)GetChild("ruinBtn");
            name = (GTextField)GetChild("name");
            ruinTime = (GTextField)GetChild("ruinTime");
            attrList = (GList)GetChild("attrList");
        }
    }
}