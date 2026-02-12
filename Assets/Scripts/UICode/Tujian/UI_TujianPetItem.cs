/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Tujian
{
    public partial class UI_TujianPetItem : GButton
    {
        public GGraph spine;
        public GTextField petName;
        public const string URL = "ui://yjhb9afbj2lcf";

        public static UI_TujianPetItem CreateInstance()
        {
            return (UI_TujianPetItem)UIPackage.CreateObject("Tujian", "TujianPetItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            spine = (GGraph)GetChild("spine");
            petName = (GTextField)GetChild("petName");
        }
    }
}