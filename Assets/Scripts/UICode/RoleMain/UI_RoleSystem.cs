/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RoleSystem : GComponent
    {
        public Controller typeCtrl;
        public Controller showPetHand;
        public UI_RoleMain roleInfo;
        public GList tabList;
        public UI_Currency btnGold;
        public UI_Currency btnDia;
        public GGraph hideUpload1;
        public GGraph hideUpload2;
        public UI_RoleCurrency btnGold2;
        public UI_RoleCurrency btnDia2;
        public const string URL = "ui://m37flevdozj91z";

        public static UI_RoleSystem CreateInstance()
        {
            return (UI_RoleSystem)UIPackage.CreateObject("RoleMain", "RoleSystem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeCtrl = GetController("typeCtrl");
            showPetHand = GetController("showPetHand");
            roleInfo = (UI_RoleMain)GetChild("roleInfo");
            tabList = (GList)GetChild("tabList");
            btnGold = (UI_Currency)GetChild("btnGold");
            btnDia = (UI_Currency)GetChild("btnDia");
            hideUpload1 = (GGraph)GetChild("hideUpload1");
            hideUpload2 = (GGraph)GetChild("hideUpload2");
            btnGold2 = (UI_RoleCurrency)GetChild("btnGold2");
            btnDia2 = (UI_RoleCurrency)GetChild("btnDia2");
        }
    }
}