/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_InheritMain : GComponent
    {
        public Controller type;
        public GComponent frame;
        public UI_NewInheritObtain newInheritObtain;
        public UI_NewInheritRecommend newInheritRecommend;
        public const string URL = "ui://ddc23erlkqmidxy9d";

        public static UI_InheritMain CreateInstance()
        {
            return (UI_InheritMain)UIPackage.CreateObject("Equip", "InheritMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            type = GetController("type");
            frame = (GComponent)GetChild("frame");
            newInheritObtain = (UI_NewInheritObtain)GetChild("newInheritObtain");
            newInheritRecommend = (UI_NewInheritRecommend)GetChild("newInheritRecommend");
        }
    }
}