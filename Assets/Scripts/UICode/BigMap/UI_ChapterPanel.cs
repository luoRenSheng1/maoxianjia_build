/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_ChapterPanel : GComponent
    {
        public GLoader3D battleSpine;
        public GLoader3D tipSpine;
        public GLoader3D cursorSpine;
        public GLoader3D qpSpine;
        public GProgressBar fishingWorkBar;
        public GGraph hero;
        public GGraph fogMask;
        public UI_EquipQiPaoItem equipQiPao;
        public GGraph moveMapMask;
        public GLoader FixedPos1;
        public GLoader FixedPos2;
        public GLoader FixedPos3;
        public UI_MineSelect MineSelect;
        public GLoader3D AiXin;
        public GLoader3D PetLost;
        public const string URL = "ui://pdufy3keusf5iix";

        public static UI_ChapterPanel CreateInstance()
        {
            return (UI_ChapterPanel)UIPackage.CreateObject("BigMap", "ChapterPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            battleSpine = (GLoader3D)GetChild("battleSpine");
            tipSpine = (GLoader3D)GetChild("tipSpine");
            cursorSpine = (GLoader3D)GetChild("cursorSpine");
            qpSpine = (GLoader3D)GetChild("qpSpine");
            fishingWorkBar = (GProgressBar)GetChild("fishingWorkBar");
            hero = (GGraph)GetChild("hero");
            fogMask = (GGraph)GetChild("fogMask");
            equipQiPao = (UI_EquipQiPaoItem)GetChild("equipQiPao");
            moveMapMask = (GGraph)GetChild("moveMapMask");
            FixedPos1 = (GLoader)GetChild("FixedPos1");
            FixedPos2 = (GLoader)GetChild("FixedPos2");
            FixedPos3 = (GLoader)GetChild("FixedPos3");
            MineSelect = (UI_MineSelect)GetChild("MineSelect");
            AiXin = (GLoader3D)GetChild("AiXin");
            PetLost = (GLoader3D)GetChild("PetLost");
        }
    }
}