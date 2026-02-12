/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Setting
{
    public partial class UI_SettingMain : GComponent
    {
        public Controller type;
        public Controller lang;
        public GComponent frame;
        public GButton closeBtn;
        public GLabel headIcon;
        public GButton changeIconBtn;
        public GTextField fightLb;
        public GTextField playerNameLb;
        public GButton editBtn;
        public GTextField uidLb;
        public GTextField serverName;
        public UI_Slider1 musicBar;
        public UI_Slider1 soundBar;
        public GButton musicCheck;
        public GButton soundCheck;
        public GButton problemBtn;
        public GButton selectServerBtn;
        public GComboBox LangChangePop;
        public GButton exchangeCodeBtn;
        public GButton gameAnnounceBtn;
        public GTextField appVersionLb;
        public GTextField txtRecord;
        public GButton btnRecord;
        public GRichTextField linkLb;
        public GList headIconList;
        public GButton useBtn;
        public const string URL = "ui://zs0w02qtffpwd";

        public static UI_SettingMain CreateInstance()
        {
            return (UI_SettingMain)UIPackage.CreateObject("Setting", "SettingMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            type = GetController("type");
            lang = GetController("lang");
            frame = (GComponent)GetChild("frame");
            closeBtn = (GButton)GetChild("closeBtn");
            headIcon = (GLabel)GetChild("headIcon");
            changeIconBtn = (GButton)GetChild("changeIconBtn");
            fightLb = (GTextField)GetChild("fightLb");
            playerNameLb = (GTextField)GetChild("playerNameLb");
            editBtn = (GButton)GetChild("editBtn");
            uidLb = (GTextField)GetChild("uidLb");
            serverName = (GTextField)GetChild("serverName");
            musicBar = (UI_Slider1)GetChild("musicBar");
            soundBar = (UI_Slider1)GetChild("soundBar");
            musicCheck = (GButton)GetChild("musicCheck");
            soundCheck = (GButton)GetChild("soundCheck");
            problemBtn = (GButton)GetChild("problemBtn");
            selectServerBtn = (GButton)GetChild("selectServerBtn");
            LangChangePop = (GComboBox)GetChild("LangChangePop");
            exchangeCodeBtn = (GButton)GetChild("exchangeCodeBtn");
            gameAnnounceBtn = (GButton)GetChild("gameAnnounceBtn");
            appVersionLb = (GTextField)GetChild("appVersionLb");
            txtRecord = (GTextField)GetChild("txtRecord");
            btnRecord = (GButton)GetChild("btnRecord");
            linkLb = (GRichTextField)GetChild("linkLb");
            headIconList = (GList)GetChild("headIconList");
            useBtn = (GButton)GetChild("useBtn");
        }
    }
}