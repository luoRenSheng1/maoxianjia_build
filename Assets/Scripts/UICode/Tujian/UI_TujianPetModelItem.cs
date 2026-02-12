/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Tujian
{
    public partial class UI_TujianPetModelItem : GComponent
    {
        public Controller bgCtrl;
        public GList petList;
        public const string URL = "ui://yjhb9afbr0wou";

        public static UI_TujianPetModelItem CreateInstance()
        {
            return (UI_TujianPetModelItem)UIPackage.CreateObject("Tujian", "TujianPetModelItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bgCtrl = GetController("bgCtrl");
            petList = (GList)GetChild("petList");
        }
    }
}