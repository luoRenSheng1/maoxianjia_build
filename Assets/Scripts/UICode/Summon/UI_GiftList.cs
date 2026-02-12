/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_GiftList : GComponent
    {
        public GList giftList;
        public const string URL = "ui://i7ojazuuftc92m";

        public static UI_GiftList CreateInstance()
        {
            return (UI_GiftList)UIPackage.CreateObject("Summon", "GiftList");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            giftList = (GList)GetChild("giftList");
        }
    }
}