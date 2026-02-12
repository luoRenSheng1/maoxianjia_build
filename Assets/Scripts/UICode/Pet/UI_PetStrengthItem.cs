/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetStrengthItem : GComponent
    {
        public GButton item;
        public GTextField curLb;
        public GTextField nextLb;
        public const string URL = "ui://lxs2h4ifk6272z";

        public static UI_PetStrengthItem CreateInstance()
        {
            return (UI_PetStrengthItem)UIPackage.CreateObject("Pet", "PetStrengthItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            item = (GButton)GetChild("item");
            curLb = (GTextField)GetChild("curLb");
            nextLb = (GTextField)GetChild("nextLb");
        }
    }
}