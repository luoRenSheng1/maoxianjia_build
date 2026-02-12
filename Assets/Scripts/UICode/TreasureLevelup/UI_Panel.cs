/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace TreasureLevelup
{
    public partial class UI_Panel : GComponent
    {
        public Controller type;
        public Controller treasureProType;
        public GButton btnCurrency;
        public GTextField txtCurrLv;
        public GTextField txtNextLv;
        public GButton btnUpgrade;
        public UI_BtnBuy btnBuy;
        public GLoader proBg;
        public GList treasureNumPro;
        public GComponent redDot;
        public GComponent lvRedDot;
        public GTextField adLb;
        public GTextField txtCountdown;
        public GButton btnSkip;
        public GButton btnHasten;
        public GList listAttrs;
        public GGraph spine;
        public GButton closeBtn;
        public const string URL = "ui://v1wfpt3li9sl1a";

        public static UI_Panel CreateInstance()
        {
            return (UI_Panel)UIPackage.CreateObject("TreasureLevelup", "Panel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            type = GetController("type");
            treasureProType = GetController("treasureProType");
            btnCurrency = (GButton)GetChild("btnCurrency");
            txtCurrLv = (GTextField)GetChild("txtCurrLv");
            txtNextLv = (GTextField)GetChild("txtNextLv");
            btnUpgrade = (GButton)GetChild("btnUpgrade");
            btnBuy = (UI_BtnBuy)GetChild("btnBuy");
            proBg = (GLoader)GetChild("proBg");
            treasureNumPro = (GList)GetChild("treasureNumPro");
            redDot = (GComponent)GetChild("redDot");
            lvRedDot = (GComponent)GetChild("lvRedDot");
            adLb = (GTextField)GetChild("adLb");
            txtCountdown = (GTextField)GetChild("txtCountdown");
            btnSkip = (GButton)GetChild("btnSkip");
            btnHasten = (GButton)GetChild("btnHasten");
            listAttrs = (GList)GetChild("listAttrs");
            spine = (GGraph)GetChild("spine");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}