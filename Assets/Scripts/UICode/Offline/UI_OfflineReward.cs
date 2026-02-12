/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Offline
{
    public partial class UI_OfflineReward : GComponent
    {
        public Controller ctrl;
        public GComponent frame;
        public GTextField timeLb;
        public GTextField keyLb;
        public GTextField goldLb;
        public GList itemList;
        public GButton adGetRw;
        public GButton getRw;
        public GComponent redPoint1;
        public GComponent redPoint2;
        public const string URL = "ui://jbbd3ox4ctxhdxy34";

        public static UI_OfflineReward CreateInstance()
        {
            return (UI_OfflineReward)UIPackage.CreateObject("Offline", "OfflineReward");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            frame = (GComponent)GetChild("frame");
            timeLb = (GTextField)GetChild("timeLb");
            keyLb = (GTextField)GetChild("keyLb");
            goldLb = (GTextField)GetChild("goldLb");
            itemList = (GList)GetChild("itemList");
            adGetRw = (GButton)GetChild("adGetRw");
            getRw = (GButton)GetChild("getRw");
            redPoint1 = (GComponent)GetChild("redPoint1");
            redPoint2 = (GComponent)GetChild("redPoint2");
        }
    }
}