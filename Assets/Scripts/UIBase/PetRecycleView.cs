using System;
using System.Collections.Generic;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Pet;
using EventDispatcher = EngineBase.EventDispatcher;
using PetSkillBook = Engine.PetSkillBook;
/// <summary>
/// 回收界面
/// </summary>
public class PetRecycleView : UIViewBase
{
    private UI_PetRecycle PetRecycle => this.main as UI_PetRecycle;

    private int _recycleType;
    private ConfigCommonUnit _common300002;
    private Dictionary<int,int> _debris = new Dictionary<int, int>();
    private List<PetItemInfo> _noUpLoadPetList = new List<PetItemInfo>();//没有上阵的宠物列表
    private Dictionary<int, double> _reDebrisDict = new Dictionary<int, double>();
    private List<int> _qualityList = new List<int>();
    private int _debrisNum = 0;
    
    private Dictionary<int, double> _reGoldDict = new Dictionary<int, double>();
    private List<PetSkillBook> _noUpLoadSkillBookList = new List<PetSkillBook>();//没有装备的宠物技能书列表
    private List<ItemData> _skillBookItemList = new List<ItemData>();//没有装备的宠物技能书道具列表
    private int _reGoldNum = 0;

    public PetRecycleView()
    {
        this.name = "PetRecycle";
        this.package = "Pet";
        this.component = "PetRecycle";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        PetBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        _common300002 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(300002);
        RecyclePetGainDebrisHandler();
        // this.PetRecycle.closeBtn.onClick.Add(this.Hide);
        this.PetRecycle.closeBtn.onClick.Add(this.HideWithSoundEffect);
        _noUpLoadPetList = PetInfoManager.Instance.GetNoUpLoadPet();
        this.PetRecycle.recycleBtn.onClick.Add(this.OnClickPetRecycle);//宠物回收
        this.PetRecycle.recycleBtn2.onClick.Add(this.OnClickSkillBookRecycle);//技能书回收
        this.PetRecycle.list.itemRenderer = QualityListRender;
         
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RECYCLE_PET,UpdateNoUpLoadPets);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RECYCLE_SKILL_BOOK,UpdateSkillBookInPackage);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RECYCLE_PET,UpdateNoUpLoadPets);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RECYCLE_SKILL_BOOK,UpdateSkillBookInPackage);
    }
    
    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _recycleType = (int)values[0];
    }

    protected override void OnShow()
    {
        base.OnShow();
        _reDebrisDict.Clear();
        _qualityList.Clear();
        this.PetRecycle.type.selectedIndex = _recycleType;
        ((UI_TabCom)this.PetRecycle.tabCom).num1.text = "0";
        
        // 宠物回收
        if(_recycleType == 0)
            UpdateNoUpLoadPets();
        
        // 宠物技能书回收
        if (_recycleType == 1)
            UpdateSkillBookInPackage();
    }

    private void UpdateNoUpLoadPets()
    {
        ((UI_TabCom)this.PetRecycle.tabCom).icon.url = UIResource.GetItemUrl(1010013.ToString());//碎片图标
        _noUpLoadPetList = PetInfoManager.Instance.GetNoUpLoadPet();
        this.PetRecycle.status.selectedIndex = _noUpLoadPetList.Count == 0 ? 1 : 0;
        // this.PetRecycle.recycleBtn.data = ;
        this.PetRecycle.list.numItems = 7;//_qualityTypeList.Count
    }

    private void UpdateSkillBookInPackage()
    {
        ((UI_TabCom)this.PetRecycle.tabCom).icon.url = UIResource.GetItemUrl(2000.ToString());//金币图标
        _noUpLoadSkillBookList = PetInfoManager.Instance.GetPetSkillBookList();//技能书
        
        //技能书道具
        _skillBookItemList.Clear();
        ItemData skillbookItemData1 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.UNCOMMON);
        ItemData skillbookItemData2 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.EPIC);
        ItemData skillbookItemData3 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.LEGEND);
        ItemData skillbookItemData4 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.MYTH);
        ItemData skillbookItemData5 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.IMMORTAL);
        ItemData skillbookItemData6 = ItemInfoManager.Instance.GetItemData((int)SkillBookType.RANDOM);
        if (skillbookItemData6.count > 0)
        {
            _skillBookItemList.Add(skillbookItemData6);
        }
        if (skillbookItemData5.count > 0)
        {
            _skillBookItemList.Add(skillbookItemData5);
        }
        if (skillbookItemData4.count > 0)
        {
            _skillBookItemList.Add(skillbookItemData4);
        }
        if (skillbookItemData3.count > 0)
        {
            _skillBookItemList.Add(skillbookItemData3);
        }
        if (skillbookItemData2.count > 0)
        {
            _skillBookItemList.Add(skillbookItemData2);
        }
        if (skillbookItemData1.count > 0)
        {
            _skillBookItemList.Add(skillbookItemData1);
        }
        
        this.PetRecycle.status.selectedIndex = _noUpLoadSkillBookList.Count == 0 && _skillBookItemList.Count == 0 ? 1 : 0;
        this.PetRecycle.list.numItems = 5;
    }

    private void QualityListRender(int index, GObject cell)
    {
        UI_InheritReItem item = (UI_InheritReItem)cell;
        item.num.visible = false;
        item.isCheck.selectedIndex = 0;
        
        // 回收宠物
        if (_recycleType == 0)
        {
            item.quality.selectedIndex = index;

            // 初始化当前品质的碎片值为0
            if (!_reDebrisDict.ContainsKey(index + 1))
            {
                _reDebrisDict[index + 1] = 0;
            }

            int petNum = 0;//不同品质的宠物的数量
            double debris = 0;//碎片数量
            foreach (var petData in _noUpLoadPetList)
            {
                if (petData.quality - 1 == index)
                {
                    petNum++;
                    
                    debris += _debris[petData.quality];
                    _reDebrisDict[petData.quality] = debris;
                }
            }
            item.num.SetVar("value", petNum.ToString()).FlushVars();
            
            item.btnCheck.data = index;
            item.btnCheck.onClick.Add(OnClickCheckBox);
        }

        // 回收技能书
        if (_recycleType == 1)
        {
            item.quality.selectedIndex = index + 2;
            int qualityLb = index + 3;
            // 初始化当前品质的金币值为0
            if (!_reGoldDict.ContainsKey(index + 3))
            {
                _reGoldDict[index + 3] = 0;
            }
            
            int bookNum = 0;//不同品质的宠物技能书的数量
            double gold = 0;//金币数量
            foreach (var skillBook in _noUpLoadSkillBookList)
            {
                var skillBookUnit = ConfigUtils.GetPetSkillBookUnitById((int)skillBook.BookId);
                if (skillBookUnit.Quality == qualityLb)
                {
                    bookNum += (int)skillBook.Amount;//数量
                    gold += skillBook.Gold * skillBook.Amount;
                    _reGoldDict[index + 3] = gold;//回收价格
                    
                }
            }

            // 技能书道具
            foreach (var itemData in _skillBookItemList)
            {
                ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemData.id);
                if (itemTypeUnit.Quality == qualityLb)
                {
                    bookNum += (int)itemData.count;
                    gold += itemTypeUnit.Sell * itemData.count;
                    _reGoldDict[index + 3] = gold;//回收价格
                }
            }
            
            item.num.SetVar("value", bookNum.ToString()).FlushVars();
            
            item.btnCheck.data = index;
            item.btnCheck.onClick.Add(OnClickCheckBox);
        }
    }
    
    private void OnClickCheckBox(EventContext context)
    {
        int index = (int)((GButton)context.sender).data;
        UI_InheritReItem item = (UI_InheritReItem)this.PetRecycle.list.GetChildAt(index);
        if (item.isCheck.selectedIndex == 0)
        {
            item.isCheck.selectedIndex = 1;
            item.num.visible = true;
            if (_recycleType == 0)
                _qualityList.Add(index + 1);

            if (_recycleType == 1)
                _qualityList.Add(index + 3);
        }
        else
        {
            item.isCheck.selectedIndex = 0;
            item.num.visible = false;
            if (_recycleType == 0)  
                _qualityList.Remove(index + 1);
            
            if (_recycleType == 1)
                _qualityList.Remove(index + 3);
        }

        SetDebrisNum();
    }

    private void SetDebrisNum()
    {
        double selectNum = 0;
        GObject[] objList = this.PetRecycle.list.GetChildren();

        if (_recycleType == 0)
        {
            for (int i = 0; i < 7; i++)
            {
                UI_InheritReItem obj = objList[i] as UI_InheritReItem;
                if (obj.isCheck.selectedIndex == 1)
                {
                    int quality = i + 1;
                    _reDebrisDict.TryGetValue(quality, out double debrisValue);
                    selectNum += debrisValue;
                }
            }
            
            _debrisNum = (int)Math.Ceiling(selectNum);// 向上取整
            ((UI_TabCom)this.PetRecycle.tabCom).num1.text = StringUtils.FormatCurrency(_debrisNum);
        }

        if (_recycleType == 1)
        {
            for (int i = 0; i < 5; i++)
            {
                UI_InheritReItem obj = objList[i] as UI_InheritReItem;
                if (obj.isCheck.selectedIndex == 1)
                {
                    int quality = i + 3;
                    _reGoldDict.TryGetValue(quality, out double goldValue);
                    selectNum += goldValue;
                }
            }
            
            _reGoldNum = (int)Math.Ceiling(selectNum);// 向上取整
            ((UI_TabCom)this.PetRecycle.tabCom).num1.text = StringUtils.FormatCurrency(_reGoldNum);
        }
        
    }

    // common表中宠物各品质回收的碎片数量
    private void RecyclePetGainDebrisHandler()
    {
        string[] debris = _common300002.Param1.Split('|');
        foreach (var d in debris)
        {
            string[] s = d.Split(',');
            _debris[int.Parse(s[0])] = int.Parse(s[1]);
        }
    }

    private void OnClickPetRecycle()
    {
        if (_debrisNum == 0)
            return;
        
        this.Hide();
        
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GoldPickupSE);
        
        var builder = BatchRecyclePets_CS.CreateBuilder();
        foreach (var quality in _qualityList)
        {
            builder.AddQuality((ePetQuality)quality);
        }
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_BatchRecyclePets_CS, builder.Build());
        UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8055, _debrisNum));
        
        // 清空缓存数据和选择状态
        _reDebrisDict.Clear();
        _qualityList.Clear();
        _debrisNum = 0;
        // 重置所有品质项的复选框状态
        GObject[] children = this.PetRecycle.list.GetChildren();
        foreach (UI_InheritReItem item in children)
        {
            if (item != null)
            {
                item.isCheck.selectedIndex = 0;
                item.num.visible = false;
            }
        }
        ((UI_TabCom)this.PetRecycle.tabCom).num1.text = StringUtils.FormatCurrency(0);
    }

    private void OnClickSkillBookRecycle()
    {
        if (_reGoldNum == 0)
            return;
        
        this.Hide();
        
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GoldPickupSE);
        
        var builder = BatchRecyclePetBooks_CS.CreateBuilder();
        foreach (var quality in _qualityList)
        {
            builder.AddQuality((ePetQuality)quality);
        }
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_BatchRecyclePetBooks_CS, builder.Build());
        UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8045, _reGoldNum));
        
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RECYCLE_SKILL_BOOK);
        
        // 清空缓存数据和选择状态
        _reGoldDict.Clear();
        _qualityList.Clear();
        _reGoldNum = 0;
        // 重置所有品质项的复选框状态
        GObject[] children = this.PetRecycle.list.GetChildren();
        foreach (UI_InheritReItem item in children)
        {
            if (item != null)
            {
                item.isCheck.selectedIndex = 0;
                item.num.visible = false;
            }
        }
        ((UI_TabCom)this.PetRecycle.tabCom).num1.text = StringUtils.FormatCurrency(0);
    }


}
