using msg;

namespace Engine
{
    public static class UIResource
    {
        //货币
        public const string GoldIcon = "ui://Common/ICON-Gold";
        public const string DiamondIcon = "ui://Common/ICON-diamond";
        public const string Diamond02Icon = "ui://Common/ICON-diamond02";
        public const string Item_10000001 = "ui://s7x7ku0ntbnhdxy4c";
        
        //属性图标
        public const string Huifu = "ui://CommonEx/ICON-VIP";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public static string GetCurrencyURL(int itemType)
        {
            switch (itemType)
            {
                default:
                    return GoldIcon;
                case 1:
                    return DiamondIcon;
                case 2:
                    return Diamond02Icon;
                case 3:
                    return Item_10000001;
                case 4://符石
                    return Huifu;
            }
        }
        
        // public static string GetPartURL(int itemType)
        // {
        //     switch (itemType)
        //     {
        //         default:
        //             return Part_None;
        //         case (int)EN_EQUIP_PARTS.Weapon:
        //             return Part_Weapon;
        //         case (int)EN_EQUIP_PARTS.Helmet:
        //             return Part_Helmet;
        //         case (int)EN_EQUIP_PARTS.ShoulderArmor:
        //             return Part_ShoulderArmor;
        //         case (int)EN_EQUIP_PARTS.Breastplate:
        //             return Part_Breastplate;
        //         case (int)EN_EQUIP_PARTS.Cloak:
        //             return Part_Cloak;
        //         case (int)EN_EQUIP_PARTS.BracerGuards:
        //             return Part_BracerGuards;
        //         case (int)EN_EQUIP_PARTS.Gloves:
        //             return Part_Gloves;
        //         case (int)EN_EQUIP_PARTS.Belt:
        //             return Part_Belt;
        //         case (int)EN_EQUIP_PARTS.Pants:
        //             return Part_Pants;
        //         case (int)EN_EQUIP_PARTS.Shoes:
        //             return Part_Shoes;
        //     }
        // }
        
        
        public static string GetItemUrl(string icon)
        {
            return string.Format("ItemPath/{0}.png", icon);
        }

        public static string GetMapItemBgByName(string name)
        {
            return string.Format("Map/event/{0}.png", name);
        }
        
        public static string GetMapItemByMapId(int mapId, int index)
        {
            return string.Format("Map/map{0}/map_item/map0{1}.png", mapId,index);
        }

        public static string GetMapBgColorByMapId(int mapId)
        {
            return string.Format("Map/map{0}/map_bg/map_bg0.png", mapId);
        }
        
        public static string GetChapterMapBgByMapId(int mapId, int bgId)
        {
            return string.Format("Map/map{0}/chapterMap/mapBg{1}.jpg", mapId, bgId);
        }

        //public static string GetChapterMapBgNewMapId(int mapId, int bgId)
        //{
        //    return string.Format("Map/map{0}/chapterMap/map_{1}.png", 1, bgId);
        //}

        public static string GetChapterMapPathBgByMapId(int mapId)
        {
            return string.Format("Map/map{0}/chapterMap/mapPath.png", mapId);
        }
        
        public static string GetMapBgByMapId(int mapId)
        {
            return string.Format("Map/map{0}/map_bg/map_bg.png", mapId);
        }

        public static string GetMapMiddleByMapId(int mapId)
        {
            return string.Format("Map/map{0}/map_middle/map_middle.png", mapId);
        }
        /// <summary>
        /// 事件的地图上显示
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <returns>加载器的url</returns>
        public static string GetMapEventBgByEventId(int eventId)
        {
            return string.Format("Map/event/event{0}.png", eventId);
        }
        /// <summary>
        /// 新的随机事件icon
        /// </summary>
        /// <param name="chapterId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public static string GetMapEventIcon(int chapterId, int eventId)
        {
            eRandomEventType type = (eRandomEventType)eventId;

            switch (type)
            {
                case eRandomEventType.eRandomEventType_NpcTask://狩猎任务
                case eRandomEventType.eRandomEventType_StageDropPet://搜寻宠物
                    return "Map/event/event3.png";
                default:
                    //{
                    //    if(chapterId == 3)
                    //    {
                    //        return $"Map/event/2_{eventId}.png";
                    //    }
                    //}
                    return $"Map/event/{chapterId}_{eventId}.png";
            }
        }

        public static string GetMapEventPopBgByEventId(int eventId)
        {
            return string.Format("Map/event/popEvent{0}.png", eventId);
        }
        
        /// <summary>
        /// 获取大地图每个节点的图标
        /// </summary>
        /// <param name="id">chapter表中的地图id+节点id(即stage i)</param>
        /// <returns></returns>
        public static string GetStageIconByMapIdAndStageId(string id)
        {
            return string.Format("Map/stageIcon/{0}.png", id);
        }
        
        public static string GetAttributeIconById(int id)
        {
            return string.Format("Common/attribute/{0}.png", id);
        }

        // 怪物特性图标
        public static string GetMonsterEntryIcon(int iconId)
        {
            return string.Format("MonsterEntryIcon/{0}.png", iconId);
        }

        public static string GetAttrIconById(string name)
        {
            return string.Format("Common/attrIcon/{0}.png", name);
        }
        
        public static string GetBloodIconByName(string name)
        {
            return string.Format("Common/model/{0}.png", name);
        }
        
        public static string GetPetIcon(string petId)
        {
            return string.Format("Pet/PetIcon/{0}.png", petId); 
        }

        // 宠物初始技能
        public static string GetPetInitSkillIcon(string petSkillId)
        {
            return string.Format("Pet/PetInitSkillIcon/{0}.png", petSkillId);
        }

        public static string GetPetSkillBookIcon(string petSkillBookIcon)
        {
            return string.Format("Pet/PetSkillBookIcon/{0}.png", petSkillBookIcon);
        }

        public static string GetHeroBody(string heroBody)
        {
            return string.Format("Hero/{0}.png", heroBody);  
        }

        public static string GetDungeonStageItemUrl(string url)
        {
            return string.Format("DungeonStageItem/{0}.png", url);
        }

        public static string GetVillageBuildImg(string url)
        {
            return string.Format("VillageBuildImg/{0}.png", url);
        }
        
        public static string GetVillageMapItem(int mapIndex)
        {
            return string.Format("VillageBuildImg/VillageMapItem/0{0}.jpg", mapIndex);
        }

        public static string GetVillageBuildTitleIcon(int index)
        {
            return string.Format("VillageBuildImg/BuildTitleIcon/bg{0}.png", index+1);
        }

        public static string GetFuncPreIcon(string systemUnitIcon)
        {
            return string.Format("FuncPreview/{0}.png",systemUnitIcon);
        }

        public static string GetExploserQuality(int quality)
        {
            return string.Format("VillageBuildImg/explorer/q{0}.png", quality);
        }
        
        public static string GetVillageTrainIcon(string url)
        {
            return string.Format("VillageBuildImg/VillageTrainIcon/{0}.png", url);
        }

        public static string GetRoleBg(string url)
        {
            return string.Format("Role/{0}.png", url);
        }

        public static string GetRoleOccupationImg(string url)
        {
            return string.Format("Role/RoleOccupationImg/{0}.png", url);
        }
        
        public static string GetRoleAttrImgImg(string url)
        {
            return string.Format("Role/RoleAttrImg/{0}.png", url);
        }

        public static string GetSummonUrl(string url)
        {
            return string.Format("Summon/{0}.png", url);
        }

        public static string GetShopUrl(string url)
        {
            return string.Format("Shop/{0}.png", url);
        }
        public static string GetImageUrlWithLang(string url, string packageName)
        {
            string sName = GameManager.Instance.GetLanguageTypeName();
            return string.Format("LangImg/" + sName + "/" + packageName + "/{0}.png", url);
        }
        
        public static string GetTalentUrl(string icon)
        {
            return string.Format("Talent/{0}.png", icon);
        }

        public static string GetAchievementIcon(string icon)
        {
            return string.Format("Achievement/{0}.png", icon);
        }

        public static string GetPetTalentIcon(string icon)
        {
            return string.Format("PetTalent/{0}.png", icon);
        }

        public static string GetHeroSkillIcon(string icon)
        {
            return string.Format("HeroSkill/{0}.png", icon);
        }
    }
}