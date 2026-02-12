/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_ChapterTaskStageDetail : GComponent
    {
        public Controller resetBtnStatus;
        public GComponent frame;
        public GLabel currency;
        public GButton huntingShopBtn;
        public GTextField name;
        public GList list;
        public UI_CostItemCom costItem;
        public GButton resetTaskBtn;
        public GButton closeBtn;
        public const string URL = "ui://pdufy3kep9n01n";

        public static UI_ChapterTaskStageDetail CreateInstance()
        {
            return (UI_ChapterTaskStageDetail)UIPackage.CreateObject("BigMap", "ChapterTaskStageDetail");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            resetBtnStatus = GetController("resetBtnStatus");
            frame = (GComponent)GetChild("frame");
            currency = (GLabel)GetChild("currency");
            huntingShopBtn = (GButton)GetChild("huntingShopBtn");
            name = (GTextField)GetChild("name");
            list = (GList)GetChild("list");
            costItem = (UI_CostItemCom)GetChild("costItem");
            resetTaskBtn = (GButton)GetChild("resetTaskBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}