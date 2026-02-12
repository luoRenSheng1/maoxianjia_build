/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetStrengList : GComponent
    {
        public GList petList;
        public const string URL = "ui://lxs2h4ifncnadxy76";

        public static UI_PetStrengList CreateInstance()
        {
            return (UI_PetStrengList)UIPackage.CreateObject("Pet", "PetStrengList");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            petList = (GList)GetChild("petList");
        }
    }
}