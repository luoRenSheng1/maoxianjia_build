/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_FengkuangPay : GComponent
    {
        public Controller isPay;
        public Controller btnCtrl;
        public GComponent frame1;
        public GLoader bg1;
        public GLoader bg2;
        public GTextField title;
        public GGraph spine;
        public GLoader bg3;
        public GTextField flTitle;
        public GButton payBtn;
        public GButton payBtn2;
        public GButton payBtn3;
        public GList rwList;
        public GButton getBtn;
        public GTextField getTitle;
        public GButton closeBtn;
        public const string URL = "ui://nmzfxo89plio16";

        public static UI_FengkuangPay CreateInstance()
        {
            return (UI_FengkuangPay)UIPackage.CreateObject("Shop", "FengkuangPay");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            isPay = GetController("isPay");
            btnCtrl = GetController("btnCtrl");
            frame1 = (GComponent)GetChild("frame1");
            bg1 = (GLoader)GetChild("bg1");
            bg2 = (GLoader)GetChild("bg2");
            title = (GTextField)GetChild("title");
            spine = (GGraph)GetChild("spine");
            bg3 = (GLoader)GetChild("bg3");
            flTitle = (GTextField)GetChild("flTitle");
            payBtn = (GButton)GetChild("payBtn");
            payBtn2 = (GButton)GetChild("payBtn2");
            payBtn3 = (GButton)GetChild("payBtn3");
            rwList = (GList)GetChild("rwList");
            getBtn = (GButton)GetChild("getBtn");
            getTitle = (GTextField)GetChild("getTitle");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}