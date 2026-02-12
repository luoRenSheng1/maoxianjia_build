using System.Collections.Generic;
using System.Text;
using Config;
using Engine;
using FairyGUI;
using Lobby;
using EventDispatcher = EngineBase.EventDispatcher;

public enum AttrType
{
    BASIC = 1,
    ADVANCED = 2,
}

public enum ObjType
{
    HERO = 1,
    PET = 2,
}

public enum HeroType
{
    GENERAL = 0,//通用属性
    LANDWARRIOR = 1,//地系战士
    WATERWARRIOR = 2,//水系战士
    FIREWARRIOR = 3,//火系战士
    WINDWARRIOR = 4,//风系战士
    LANDMAGE = 5,//地系法师
    WATERMAGE = 6,//水系法师
    FIREMAGE = 7,//火系法师
    WINDMAGE = 8,//风系法师
    LANDTAOIST = 9,//地系道士
    WATERTAOIST = 10,//水系道士
    FIRETAOIST = 11,//火系道士
    WINDTAOIST = 12,//风系道士
}

public class AllHeroAttrPanelView : UIViewBase
{
    private UI_AllHeroAttrPanel heroAttrUI => this.main as UI_AllHeroAttrPanel;
    
    private StringBuilder _stringBuilder = new StringBuilder();
    
    private FightAttrVo fightAttrVo;
    private PetItemInfo petItemInfo;
    private ObjType objType;

    private HeroInfo _myHero;//当前使用的英雄
    private int _heroCareer;//英雄职业：战士、法师、道士
    private int _heroCareerAttr;//英雄职业属性：地系、水系、火系、风系
    private HeroType _myHeroType;//当前使用的英雄类型
    private List<ConfigHeroAttributesUnit> _allBasicHeroAttributesUnit = new List<ConfigHeroAttributesUnit>();//英雄基础属性
    private List<ConfigHeroAttributesUnit> _allAdvanceHeroAttributesUnit = new List<ConfigHeroAttributesUnit>();//英雄高级属性
    private List<ConfigHeroAttributesUnit> _filterBasicAttrList = new List<ConfigHeroAttributesUnit>();//英雄
    private List<ConfigHeroAttributesUnit> _filterAdvanceAttrList = new List<ConfigHeroAttributesUnit>();//英雄
    
    private List<ConfigHeroAttributesUnit> _allBasicPetAttrbutesUnit = new List<ConfigHeroAttributesUnit>();//宠物基础属性
    private List<ConfigHeroAttributesUnit> _allAdvancePetAttrbutesUnit = new List<ConfigHeroAttributesUnit>();//宠物高级属性
    private List<ConfigHeroAttributesUnit> _filterPetBasicAttrList = new List<ConfigHeroAttributesUnit>();//宠物
    private List<ConfigHeroAttributesUnit> _filterPetAdvanceAttrList = new List<ConfigHeroAttributesUnit>();//宠物
    
    public AllHeroAttrPanelView()
    {
        this.name = "AllHeroAttrPanel";
        this.package = "Lobby";
        this.component = "AllHeroAttrPanel";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.heroAttrUI.closeBtn2.onClick.Add(this.Hide);
        this.heroAttrUI.closeBtn2.onClick.Add(this.HideWithSoundEffect);

        // _myHero = HeroInfoManager.Instance.GetMyHero();
        // _heroCareer = _myHero.HeroUnit.Vocation;//职业：战士、法师、道士
        // _heroCareerAttr = _myHero.HeroUnit.VocationAttr;//职业属性：地、水、火、风
        // GetHeroType();
        
        _allBasicHeroAttributesUnit = ConfigUtils.GetAllHeroAttributesUnitByObjTypeAndAttrType((int)ObjType.HERO, (int)AttrType.BASIC);
        _allAdvanceHeroAttributesUnit = ConfigUtils.GetAllHeroAttributesUnitByObjTypeAndAttrType((int)ObjType.HERO, (int)AttrType.ADVANCED);

        _allBasicPetAttrbutesUnit = ConfigUtils.GetAllHeroAttributesUnitByObjTypeAndAttrType((int)ObjType.PET, (int)AttrType.BASIC);
        _allAdvancePetAttrbutesUnit = ConfigUtils.GetAllHeroAttributesUnitByObjTypeAndAttrType((int)ObjType.PET, (int)AttrType.ADVANCED);
        
        this.heroAttrUI.basicAttrList.itemRenderer = BasicAttrListRender;
        this.heroAttrUI.advancedAttrList.itemRenderer = AdvanceAttrListRender;
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_BATTLE_HERO, this.UpdateAttrInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_BATTLE_HERO_Attr, this.UpdatePetAttrInfo);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_BATTLE_HERO, this.UpdateAttrInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_BATTLE_HERO_Attr, this.UpdatePetAttrInfo);
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        // fightAttrVo = values[0] as FightAttrVo;
        // if (values.Length > 1 && values[1] != null)
        // {
        //     petItemInfo = values[1] as PetItemInfo;
        // }

        if ((ObjType)values[0] == ObjType.HERO)
        {
            objType =  (ObjType)values[0];
            fightAttrVo = values[1] as FightAttrVo;
        }
        else
        {
            objType =  (ObjType)values[0];
            fightAttrVo = values[1] as FightAttrVo;
            petItemInfo = values[2] as PetItemInfo;
        }
    }

    protected override void OnHide()
    {
        base.OnHide();
        fightAttrVo = null;
        petItemInfo = null;

    }

    protected override void OnShow()
    {
        base.OnShow();
        _stringBuilder.Clear();
        // RoleData roleData = DataManager.Instance.GetRoleData();
        _filterBasicAttrList.Clear();
        _filterAdvanceAttrList.Clear();
        
        _filterPetBasicAttrList.Clear();
        _filterPetAdvanceAttrList.Clear();
        
        if (objType == ObjType.HERO)
        {
            UpdateAttrInfo();
        }

        if (objType == ObjType.PET)
        {
            UpdatePetAttrInfo();
        }

    }

    private void GetHeroType()
    {
        // 战士
        if (_heroCareer == 1 )
        {
            // 1:地  2:水   3:火   4:风
            if (_heroCareerAttr == 1)
            {
                _myHeroType = HeroType.LANDWARRIOR;
            }
            if (_heroCareerAttr == 2)
            {
                _myHeroType = HeroType.WATERWARRIOR;
            }
            if (_heroCareerAttr == 3)
            {
                _myHeroType = HeroType.FIREWARRIOR;
            }
            if (_heroCareerAttr == 4)
            {
                _myHeroType = HeroType.WINDWARRIOR;
            }
        }

        // 法师
        if (_heroCareer == 2)
        {
            // 1:地  2:水   3:火   4:风
            if (_heroCareerAttr == 1)
            {
                _myHeroType = HeroType.LANDMAGE;
            }
            if (_heroCareerAttr == 2)
            {
                _myHeroType = HeroType.WATERMAGE;
            }
            if (_heroCareerAttr == 3)
            {
                _myHeroType = HeroType.FIREMAGE;
            }
            if (_heroCareerAttr == 4)
            {
                _myHeroType = HeroType.WINDMAGE;
            }
        }

        // 道士
        if (_heroCareer == 3)
        {
            // 1:地  2:水   3:火   4:风
            if (_heroCareerAttr == 1)
            {
                _myHeroType = HeroType.LANDTAOIST;
            }
            if (_heroCareerAttr == 2)
            {
                _myHeroType = HeroType.WATERTAOIST;
            }
            if (_heroCareerAttr == 3)
            {
                _myHeroType = HeroType.FIRETAOIST;
            }
            if (_heroCareerAttr == 4)
            {
                _myHeroType = HeroType.WINDTAOIST;
            }
        }
    }

    private void FilterBasicAttrs()
    {
        foreach (var attr in _allBasicHeroAttributesUnit)
        {
            bool isGeneral = attr.HeroType == (int)HeroType.GENERAL;
            bool isMyType = attr.HeroType == (int)_myHeroType;
        
            if ((isGeneral || isMyType) && attr.ShowID == 1)
            {
                _filterBasicAttrList.Add(attr);
            }
            else if ((isGeneral || isMyType) && attr.ShowID == 2)
            {
                if (GetHeroAttrInfoByAttrId(attr.AttributesID).count > 0)
                {
                    _filterBasicAttrList.Add(attr);
                }
            }
        }
        
        _filterBasicAttrList.Sort((a,b) => a.SortID.CompareTo(b.SortID));
        
    }

    private void FilterAdvancedAttrs()
    {
        foreach (var attr in _allAdvanceHeroAttributesUnit)
        {
            bool isGeneral = attr.HeroType == (int)HeroType.GENERAL;
            bool isMyType = attr.HeroType == (int)_myHeroType;
        
            if ((isGeneral || isMyType) && attr.ShowID == 1)
            {
                _filterAdvanceAttrList.Add(attr);
            }
            else if ((isGeneral || isMyType) && attr.ShowID == 2)
            {
                if (GetHeroAttrInfoByAttrId(attr.AttributesID).count > 0)
                {
                    _filterAdvanceAttrList.Add(attr);
                }
            }
        }
        
        _filterAdvanceAttrList.Sort((a,b) => a.SortID.CompareTo(b.SortID));
    }

    private ItemData GetHeroAttrInfoByAttrId(int attrId)
    {
        ItemData itemData = new ItemData();
        itemData.id = attrId;
        
        switch (attrId)
        {
            case 1:
                itemData.count = fightAttrVo.PhysicAtk;
                break;
            case 2:
                itemData.count = fightAttrVo.MagicAtk;
                break;
            case 3:
                itemData.count = fightAttrVo.SorceryAtk;
                break;
            case 4:
                itemData.count = fightAttrVo.HP;
                break;
            case 5:
                itemData.count = fightAttrVo.PhysicDef;
                break;
            case 6:
                itemData.count = fightAttrVo.MagicDef;
                break;
            case 7:
                itemData.count = fightAttrVo.SorceryDef;
                break;
            case 8:
                itemData.count = fightAttrVo.Recovery;
                break;
            case 9:
                itemData.count = fightAttrVo.PetAtk;
                break;
            case 10:
                itemData.count = fightAttrVo.PhysicAtkADD;
                break;
            case 11:
                itemData.count = fightAttrVo.MagicAtkADD;
                break;
            case 12:
                itemData.count = fightAttrVo.SorceryAtkADD;
                break;
            case 13:
                itemData.count = fightAttrVo.HPADD;
                break;
            case 14:
                itemData.count = fightAttrVo.PhysicDefADD;
                break;
            case 15:
                itemData.count = fightAttrVo.MagicDefADD;
                break;
            case 16:
                itemData.count = fightAttrVo.SorceryDefADD;
                break;
            case 17:
                itemData.count = fightAttrVo.EarthAtkADD;
                break;
            case 18:
                itemData.count = fightAttrVo.WaterAtkADD;
                break;
            case 19:
                itemData.count = fightAttrVo.FireAtkADD;
                break;
            case 20:
                itemData.count = fightAttrVo.AirAtkADD;
                break;
            case 21:
                itemData.count = fightAttrVo.PetAtkADD;
                break;
            case 22:
                itemData.count = fightAttrVo.HPMultiple;
                break;
            case 23:
                itemData.count = fightAttrVo.AtkMultiple;
                break;
            case 24:
                itemData.count = fightAttrVo.JoukRate;
                break;
            case 25:
                itemData.count = fightAttrVo.AtkHitRate;
                break;
            case 26:
                itemData.count = fightAttrVo.ParryRate;
                break;
            case 27:
                itemData.count = fightAttrVo.ParryValue;
                break;
            case 28:
                itemData.count = fightAttrVo.IgnoreDef;
                break;
            case 29:
                itemData.count = fightAttrVo.CriticalStrike;
                break;
            case 30:
                itemData.count = fightAttrVo.CriticalInjury;
                break;
            case 31:
                itemData.count = fightAttrVo.BossDamageAdd;
                break;
            case 32:
                itemData.count = fightAttrVo.MonsterDamageAdd;
                break;
            case 33:
                itemData.count = fightAttrVo.Mitigation;
                break;
            case 34:
                itemData.count = fightAttrVo.Bloodsucking;
                break;
            case 35:
                itemData.count = fightAttrVo.AtkHPRecovery;
                break;
            case 36:
                itemData.count = fightAttrVo.SkillDamage;
                break;
            case 37:
                itemData.count = fightAttrVo.MagicTimesAdd;
                break;
            case 38:
                itemData.count = fightAttrVo.MagicTimes;
                break;
            case 39:
                itemData.count = fightAttrVo.GoldAdd;
                break;
            case 40:
                itemData.count = fightAttrVo.SkillCd;
                break;
            case 41:
                itemData.count = fightAttrVo.AtkSpeed;
                break;
            case 42:
                itemData.count = fightAttrVo.ComboAtk;
                break;
            case 43:
                itemData.count = fightAttrVo.CounterAtk;
                break;
            case 44:
                itemData.count = fightAttrVo.LoreEquipRate;
                break;
            case 45:
                itemData.count = fightAttrVo.DropLoreEquipRate;
                break;
            case 46:
                itemData.count = fightAttrVo.OnlineAwardTimes;
                break;
            case 47:
                itemData.count = fightAttrVo.HomeLimitMaxTime;
                break;
            case 48:
                itemData.count = fightAttrVo.BattleFinalAttack;
                break;
            case 49:
                itemData.count = fightAttrVo.KilledRecovery;
                break;
            case 101:
                itemData.count = fightAttrVo.Atk;
                break;
            case 102:
                itemData.count = fightAttrVo.Def;
                break;
        }
        
        return itemData;
    }

    // 英雄属性
    private void UpdateAttrInfo()
    {
        _filterBasicAttrList.Clear();
        _filterAdvanceAttrList.Clear();
        
        _myHero = HeroInfoManager.Instance.GetMyHero();
        _heroCareer = _myHero.HeroUnit.Vocation;//职业：战士、法师、道士
        _heroCareerAttr = _myHero.HeroUnit.VocationAttr;//职业属性：地、水、火、风
        GetHeroType();

        if (fightAttrVo == null)
            return;
        
        FilterBasicAttrs();
        FilterAdvancedAttrs();
        this.heroAttrUI.basicAttrList.numItems = _filterBasicAttrList.Count;
        this.heroAttrUI.advancedAttrList.numItems = _filterAdvanceAttrList.Count;
    }

    private void FilterPetBasicAttrs()
    {
        foreach (var attr in _allBasicPetAttrbutesUnit)
        {
            
            if (attr.ShowID == 1)
            {
                _filterPetBasicAttrList.Add(attr);
            }
        }
        
        _filterPetBasicAttrList.Sort((a,b) => a.SortID.CompareTo(b.SortID));
    }

    private void FilterPetAdvancedAttrs()
    {
        foreach (var attr in _allAdvancePetAttrbutesUnit)
        {
            if (attr.ShowID == 1)
            {
                _filterPetAdvanceAttrList.Add(attr);
            }
        }
        
        _filterPetAdvanceAttrList.Sort((a,b) => a.SortID.CompareTo(b.SortID));
    }

    // 宠物属性
    private void UpdatePetAttrInfo()
    {
        _filterPetBasicAttrList.Clear();
        _filterPetAdvanceAttrList.Clear();
        
        if (fightAttrVo == null)
            return;
        
        if (petItemInfo == null)
            return;
        
        FilterPetBasicAttrs();
        FilterPetAdvancedAttrs();
        this.heroAttrUI.basicAttrList.numItems = _filterPetBasicAttrList.Count;
        this.heroAttrUI.advancedAttrList.numItems = _filterPetAdvanceAttrList.Count;
    }

    private void BasicAttrListRender(int index, GObject item)
    {
        if (index % 4 == 2 || index % 4 == 3)
        {
            ((UI_HeroAttrItem)item).status.selectedIndex = 1;
        }
        else
        {
            ((UI_HeroAttrItem)item).status.selectedIndex = 0;
        }
        
        // 英雄
        if (objType == ObjType.HERO)
        {
            var unit = _filterBasicAttrList[index];
            ItemData itemData = GetHeroAttrInfoByAttrId(unit.AttributesID);
            ((UI_HeroAttrItem)item).attrName.text = ConfigUtils.GetTextById(unit.Doc);
            // ((UI_HeroAttrItem)item).attrName.text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(itemData.id).AttrName);
            string attValue = EquipManager.Instance.SetAttributeValue(itemData.id, itemData.count);
            ((UI_HeroAttrItem)item).attrValue.text = attValue;
        }
        
        // 宠物
        if (objType == ObjType.PET)
        {
            var unit = _filterPetBasicAttrList[index];
            ((UI_HeroAttrItem)item).attrName.text = ConfigUtils.GetTextById(unit.Doc);
            if (unit.AttributesID == 201)
            {
                ((UI_HeroAttrItem)item).attrValue.text = (petItemInfo.growRate*100).ToString("f2")+"%";
            }
            else if (unit.AttributesID == 202)
            {
                ((UI_HeroAttrItem)item).attrValue.text = (petItemInfo.petCfg.Inherit * ConstDefine.CONFIG_PLACE_EX*100).ToString("f2")+"%";
            }
            else
            {
                ItemData itemData = GetHeroAttrInfoByAttrId(unit.AttributesID);
                string attValue = EquipManager.Instance.SetAttributeValue(itemData.id, itemData.count);
                ((UI_HeroAttrItem)item).attrValue.text = attValue;
            }
        }
    }

    private void AdvanceAttrListRender(int index, GObject item)
    {
        if (index % 4 == 2 || index % 4 == 3)
        {
            ((UI_HeroAttrItem)item).status.selectedIndex = 1;
        }
        else
        {
            ((UI_HeroAttrItem)item).status.selectedIndex = 0;
        }
        
        // 英雄
        if (objType == ObjType.HERO)
        {
            var unit = _filterAdvanceAttrList[index];
            ItemData itemData = GetHeroAttrInfoByAttrId(unit.AttributesID);
            ((UI_HeroAttrItem)item).attrName.text = ConfigUtils.GetTextById(unit.Doc);
            // ((UI_HeroAttrItem)item).attrName.text = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(itemData.id).AttrName);
            string attValue = EquipManager.Instance.SetAttributeValue(itemData.id, itemData.count);
            ((UI_HeroAttrItem)item).attrValue.text = attValue;
        }
        
        // 宠物
        if (objType == ObjType.PET)
        {
            var unit = _filterPetAdvanceAttrList[index];
            ItemData itemData = GetHeroAttrInfoByAttrId(unit.AttributesID);
            ((UI_HeroAttrItem)item).attrName.text = ConfigUtils.GetTextById(unit.Doc);
            string attValue = EquipManager.Instance.SetAttributeValue(itemData.id, itemData.count);
            ((UI_HeroAttrItem)item).attrValue.text = attValue;
        }
        
    }
}
