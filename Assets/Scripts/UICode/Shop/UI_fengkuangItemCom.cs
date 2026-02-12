/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_fengkuangItemCom : GComponent
    {
        public Controller status;
        public GButton itemCom;
        public GButton getBtn;
        public const string URL = "ui://nmzfxo89plio11";

        public static UI_fengkuangItemCom CreateInstance()
        {
            return (UI_fengkuangItemCom)UIPackage.CreateObject("Shop", "fengkuangItemCom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            itemCom = (GButton)GetChild("itemCom");
            getBtn = (GButton)GetChild("getBtn");
        }
    }
}