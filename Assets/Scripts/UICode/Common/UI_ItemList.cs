/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_ItemList : GComponent
    {
        public GList itemList;
        public const string URL = "ui://0anhreylumzkdxy7b";

        public static UI_ItemList CreateInstance()
        {
            return (UI_ItemList)UIPackage.CreateObject("Common", "ItemList");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            itemList = (GList)GetChild("itemList");
        }
    }
}