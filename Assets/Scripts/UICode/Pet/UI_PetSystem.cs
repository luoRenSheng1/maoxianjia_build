/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetSystem : GComponent
    {
        public Controller typeCtrl;
        public UI_PetSystemDetail petDetail;
        public GList tabList;
        public const string URL = "ui://lxs2h4ifs4440";

        public static UI_PetSystem CreateInstance()
        {
            return (UI_PetSystem)UIPackage.CreateObject("Pet", "PetSystem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeCtrl = GetController("typeCtrl");
            petDetail = (UI_PetSystemDetail)GetChild("petDetail");
            tabList = (GList)GetChild("tabList");
        }
    }
}