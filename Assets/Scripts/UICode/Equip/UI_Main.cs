/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_Main : GComponent
    {
        public Controller uiType;
        public GComponent frame;
        public UI_NewEquipObtain uiEquipObtain;
        public UI_NewEquipRecommend uiEquipRecommand;
        public const string URL = "ui://ddc23erlun3j12";

        public static UI_Main CreateInstance()
        {
            return (UI_Main)UIPackage.CreateObject("Equip", "Main");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            uiType = GetController("uiType");
            frame = (GComponent)GetChild("frame");
            uiEquipObtain = (UI_NewEquipObtain)GetChild("uiEquipObtain");
            uiEquipRecommand = (UI_NewEquipRecommend)GetChild("uiEquipRecommand");
        }
    }
}