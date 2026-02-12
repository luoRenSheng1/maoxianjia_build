/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_GetReward : GComponent
    {
        public UI_FrameMask_Normal frame;
        public GLoader img;
        public GTextField name;
        public GGroup title;
        public UI_ItemList rewardList;
        public GButton closeBtn;
        public GImage wb;
        public GLoader3D gxhdSpine;
        public GGroup titaction;
        public Transition shake;
        public Transition t1;
        public const string URL = "ui://0anhreylx3dkdxy2d";

        public static UI_GetReward CreateInstance()
        {
            return (UI_GetReward)UIPackage.CreateObject("Common", "GetReward");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (UI_FrameMask_Normal)GetChild("frame");
            img = (GLoader)GetChild("img");
            name = (GTextField)GetChild("name");
            title = (GGroup)GetChild("title");
            rewardList = (UI_ItemList)GetChild("rewardList");
            closeBtn = (GButton)GetChild("closeBtn");
            wb = (GImage)GetChild("wb");
            gxhdSpine = (GLoader3D)GetChild("gxhdSpine");
            titaction = (GGroup)GetChild("titaction");
            shake = GetTransition("shake");
            t1 = GetTransition("t1");
        }
    }
}