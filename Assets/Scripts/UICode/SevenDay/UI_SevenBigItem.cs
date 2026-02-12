/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace SevenDay
{
    public partial class UI_SevenBigItem : GButton
    {
        public Controller tag;
        public GList rwList;
        public GButton rewardBtn;
        public const string URL = "ui://1tqiwa7amg7g8";

        public static UI_SevenBigItem CreateInstance()
        {
            return (UI_SevenBigItem)UIPackage.CreateObject("SevenDay", "SevenBigItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            tag = GetController("tag");
            rwList = (GList)GetChild("rwList");
            rewardBtn = (GButton)GetChild("rewardBtn");
        }
    }
}