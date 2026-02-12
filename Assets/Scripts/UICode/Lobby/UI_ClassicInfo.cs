/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_ClassicInfo : GComponent
    {
        public Controller maxCtrl;
        public GTextField classicName;
        public GButton classItem;
        public GList maxAttrList;
        public const string URL = "ui://s7x7ku0nmtmsdxycn";

        public static UI_ClassicInfo CreateInstance()
        {
            return (UI_ClassicInfo)UIPackage.CreateObject("Lobby", "ClassicInfo");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            maxCtrl = GetController("maxCtrl");
            classicName = (GTextField)GetChild("classicName");
            classItem = (GButton)GetChild("classItem");
            maxAttrList = (GList)GetChild("maxAttrList");
        }
    }
}