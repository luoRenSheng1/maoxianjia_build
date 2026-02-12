
using System.Collections.Generic;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Pet;
using EventDispatcher = EngineBase.EventDispatcher;
using PetBattleAttr = Engine.PetBattleAttr;
using PetSkillBookSlot = Engine.PetSkillBookSlot;
using PetTalent = Engine.PetTalent;
using UI_PetItem = Pet.UI_PetItem;

public class PetDetailView : UIViewBase
{
    private UI_PetDetail PetDetail => this.main as UI_PetDetail;
    
    private PetItemInfo _curPet;
    private bool isTips = false;
    private bool _isGuiding;
    // private List<int> _petTalentList = new List<int>();//宠物天赋id列表
    private List<PetTalent> _petTalentList = new List<PetTalent>();//宠物天赋id列表
    private List<PetSkillBookSlot> _bookSlotList = new List<PetSkillBookSlot>();//宠物技能书

    private ConfigCommonUnit _common300002;//不同品质回收的碎片数量
    private Dictionary<int, int> _petItemDic = new Dictionary<int, int>();
    
    public PetDetailView()
    {
        this.name = "PetDetail";
        this.package = "Pet";
        this.component = "PetDetail";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        PetBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _curPet = values[0] as PetItemInfo;
        if (values.Length == 2)
            isTips = (bool) values[1];
    }

    protected override void OnInit()
    {
        base.OnInit();
        _common300002 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(300002);
        // this.PetDetail.closeBtn.onClick.Add(this.Hide);
        this.PetDetail.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.PetDetail.uploadBtn.onClick.Add(this.OnClickUploadBtn);
        this.PetDetail.replaceBtn.onClick.Add(this.OnClickReplaceBtn);
        this.PetDetail.downBtn.onClick.Add(this.OnClickDownBtn);
        this.PetDetail.getBtn.onClick.Add(this.OnClickGetBtn);
        this.PetDetail.recyclePetBtn.onClick.Add(this.OnClickRecycleBtn);
        this.PetDetail.petTalentList.itemRenderer = PetTalentListRender;
        this.PetDetail.petBookList.itemRenderer = PetBookListRender;
    }

    protected override void OnShow()
    {
        base.OnShow();

        HandlerRecyclePetItem();
        
        PetItemInfo hasPetInfo = PetInfoManager.Instance.GetPet(_curPet.PetGuid);
        if (hasPetInfo != null)
        {
            _curPet = hasPetInfo;
        }

        _petTalentList = _curPet.talentsList;
        this.PetDetail.petTalentList.numItems = _petTalentList.Count;//5
        _bookSlotList = _curPet.bookSlotsList;
        this.PetDetail.petBookList.numItems = _curPet.bookSlotsList.Count;

        ((UI_PetItem) this.PetDetail.petItem).qualityCtrl.selectedIndex = _curPet.quality - 1;
        // ((UI_PetItem) this.PetDetail.petItem).icon = UIResource.GetPetIcon(_curPet.petCfg.IconPath);
        ((UI_PetItem) this.PetDetail.petItem).icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeByParam(int.Parse(_curPet.petCfg.IconPath)).Icon);
        ((UI_PetItem) this.PetDetail.petItem).petLv.SetVar("value", _curPet.PetLv.ToString()).FlushVars();
        
        this.PetDetail.petName.text = ConfigUtils.GetTextById(_curPet.petCfg.Name);
        this.PetDetail.gorwValue.SetVar("value",(_curPet.growRate * 100).ToString("f2") + "%").FlushVars();
        this.PetDetail.inherit.SetVar("value",(_curPet.petCfg.Inherit * ConstDefine.CONFIG_PLACE_EX*100).ToString("f2")).FlushVars();
        this.PetDetail.petQuality.qualityCtrl.selectedIndex = _curPet.quality - 1;
        // this.PetDetail.petSkill.icon = UIResource.GetItemUrl(ConfigUtils.GetSkillById(_curPet.skillId).SkillIcon);
        this.PetDetail.petSkill.icon = UIResource.GetPetInitSkillIcon(ConfigUtils.GetSkillById(_curPet.skillId).SkillIcon);
        this.PetDetail.petSkill.qualityCtrl.selectedIndex = ConfigUtils.GetSkillById(_curPet.skillId).SkillQuality - 1;
        this.PetDetail.petSkill.num.visible = true;
        this.PetDetail.petSkill.num.SetVar("value",_curPet.skillLevel.ToString()).FlushVars();
        this.PetDetail.skillName.text = ConfigUtils.GetTextById(ConfigUtils.GetSkillById(_curPet.skillId).Name);
        this.PetDetail.recyclePetBtn.itemIcon.url = UIResource.GetItemUrl(1010013.ToString());

        //天赋解锁
        var unlock1 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.PetTalent);
        this.PetDetail.TalentUnlock.selectedIndex = unlock1.Item1 ? 1 : 0;
        this.PetDetail.TalentUnlockText.text = unlock1.Item2;
        //技能书解锁
        var unlock2 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.PetSkillBook);
        this.PetDetail.SkillBookUnlock.selectedIndex = unlock2.Item1 ? 1 : 0;
        this.PetDetail.SkillBookUnlockText.text = unlock2.Item2;

        // this.PetDetail.skillDesc.text = ConfigUtils.GetTextById(ConfigUtils.GetSkillById(_curPet.skillId).SkillDes);
        ConfigPetSkillLevelUnit petSkillLevelData = ConfigUtils.GetPetSkillLevelUnitBySkillIdWithLevel(_curPet.skillId, _curPet.skillLevel);

        if (petSkillLevelData != null)
        {
            if (petSkillLevelData.SkillHurt > 0)
                this.PetDetail.skillDesc.text = string.Format(ConfigUtils.GetTextById(petSkillLevelData.Doc), (petSkillLevelData.SkillHurt * ConstDefine.CONFIG_PLACE).ToString("f2"));
            else
            {
                if (!petSkillLevelData.CoverCommonParam.Equals("0"))
                {
                    string[] param = petSkillLevelData.CoverCommonParam.Split("|");
                    string[] commons = param[0].Split(",");
                    if (commons.Length > 1)
                    {
                        // if (int.Parse(commons[0]) == 101)
                        // {
                        //     string value101 = (double.Parse(commons[1])/100).ToString("f2") + "%";
                        //     this.PetDetail.skillDesc.text = string.Format(ConfigUtils.GetTextById(petSkillLevelData.Doc), value101);
                        // }
                        // else
                        // {
                        //     string lastValue = EquipManager.Instance.SetAttributeValue(int.Parse(commons[0]), double.Parse(commons[1]), true);
                        //     this.PetDetail.skillDesc.text = string.Format(ConfigUtils.GetTextById(petSkillLevelData.Doc), lastValue);
                        // }
                        
                        // CoverCommonParam全为万分比
                        string lastValue = (int.Parse(commons[1]) * ConstDefine.CONFIG_PLACE).ToString("f2") + "%";
                        this.PetDetail.skillDesc.text = string.Format(ConfigUtils.GetTextById(petSkillLevelData.Doc), lastValue);
                    }
                    else
                    {
                        this.PetDetail.skillDesc.text = "";
                    }
                }
            }
        }
        
        // ((UI_petQualityItem)this.PetDetail.petQ).quality.selectedIndex = _curPet.petCfg.Quality - 1;
        // this.PetDetail.jcLb.SetVar("value", (_curPet.petCfg.Inherit*ConstDefine.CONFIG_PLACE_EX*100f).ToString("f2")).FlushVars();
        // this.PetDetail.atkSpeed.SetVar("value", (_curPet.petCfg.AtkSpeed*ConstDefine.CONFIG_PLACE_EX).ToString("f2")).FlushVars();
        
        if (hasPetInfo != null)
        {
            bool isUpload = false;
            Dictionary<int, PetItemInfo> upLoadPets = PetInfoManager.Instance.GetUpLoadPet();
            foreach (var pet in upLoadPets)
            {
                if (pet.Value.PetGuid == hasPetInfo.PetGuid)
                {
                    isUpload =  true;
                    break;
                }
            }
            
            // bool isUpload = PetInfoManager.Instance.IsInUpload(hasPetInfo);
            if (isUpload)
            {
                this.PetDetail.ctrl.selectedIndex = 0;
            }
            else
            {
                //判断还有没有上阵位置，有的话直接上阵，否则要替换
                bool isAll = PetInfoManager.Instance.GetBattlePetList().Count == PetInfoManager.Instance.UnLockPetPos;
                if (isAll)
                {
                    this.PetDetail.ctrl.selectedIndex = 2;
                    if (hasPetInfo.DispatchBuild != VillageBuildType.None)
                    {
                        // 已驻扎
                        this.PetDetail.isStation.selectedIndex = 1;
                    }
                    else
                    {
                        this.PetDetail.isStation.selectedIndex = 0;
                        this.PetDetail.recyclePetBtn.num.SetVar("value",_petItemDic[hasPetInfo.quality].ToString()).FlushVars();
                    }
                }
                else
                {
                    this.PetDetail.ctrl.selectedIndex = 1;
                    if (hasPetInfo.DispatchBuild != VillageBuildType.None)
                    {
                        // 已驻扎
                        this.PetDetail.isStation.selectedIndex = 1;
                    }
                    else
                    {
                        this.PetDetail.isStation.selectedIndex = 0;
                        this.PetDetail.recyclePetBtn.num.SetVar("value",_petItemDic[hasPetInfo.quality].ToString()).FlushVars();
                    }
                }
            }

        }
        else
        {
            this.PetDetail.ctrl.selectedIndex = 3;
        }

        this.PetDetail.sortingOrder = isTips ? 999 : 0;
        this.PetDetail.tipsCtrl.selectedIndex = isTips ? 1 : 0;

        //if (!GuideManager.Instance.NotShowThisGuide((int) GuideID.Click_EquipPetItem))
        //{
        //    GuideManager.Instance.HideGuide();
        //    _isGuiding = true;
        //    GuideManager.Instance.StartGuide(this.PetDetail.uploadBtn, GuideID.Click_EquipPetItem, PosType.Left, true, true);
        //}
        GameManager.Instance.TimerManager.SetTimer(0.01f, () => {
            //引导-点击装备宠物
            if (GuideManager.Instance.StarGuideByData(new GuideData()
            {
                giding = GuideID.Click_FirstPetItem,
                gid = GuideID.Click_EquipPetItem,
                tui = this.PetDetail.uploadBtn,
                isForce = true,
                isSend = true
            })) { _isGuiding = true; }
        });
    }

    private void HandlerRecyclePetItem()
    {
        string param1 = _common300002.Param1;
        string[] str = param1.Split('|');
        foreach (var s in str)
        {
            int quality = int.Parse(s.Split(',')[0]);
            int num = int.Parse(s.Split(',')[1]);
            _petItemDic[quality] = num;
        }
    }

    private void OnClickUploadBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        int index = PetInfoManager.Instance.GetNoUploadIndex();
        if (index != -1)
        {
            GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralUploadSE);
            var builder = PetInSlot_CS.CreateBuilder();
            builder.PetId = _curPet.PetGuid;
            builder.PetSlotId = (uint)index;
            PetInSlot_CS petInSlotCs = builder.Build();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_PetInSlot_CS, petInSlotCs);
        }
    }

    private void OnClickReplaceBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPLOAD_PET, _curPet);
    }

    private void OnClickDownBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        PetItemInfo pet = PetInfoManager.Instance.GetPet(_curPet.PetGuid);
        if (pet != null)
        {
            var builder = RemovePetFromSlot_CS.CreateBuilder();
            builder.PetSlotId = (uint)pet.BattleIndex;
            RemovePetFromSlot_CS petRemove = builder.Build();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_RemovePetFromSlot_CS, petRemove);
        }

    }

    private void OnClickGetBtn()
    {
        LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
        lobbyView?.OpenBottomPanel(5, 0);
    }

    private void OnClickRecycleBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        var builder = RecyclePets_CS.CreateBuilder();
        builder.PetId = _curPet.PetGuid;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_RecyclePets_CS, builder.Build());
        UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8055, _petItemDic[_curPet.quality]));
    }

    protected override void OnHide()
    {
        base.OnHide();
        isTips = false;
        if (_isGuiding)
        {
            _isGuiding = false;
            GuideManager.Instance.HideGuide();

        }
    }

    private void PetTalentListRender(int index, GObject item)
    {
        ((UI_PetTalentItem)item).lockCtrl.selectedIndex = 0;
        ConfigPetAptitudeUnit unit = ConfigUtils.GetPetAptitudeUnitById(_petTalentList[index].talentId);
        ((UI_PetTalentItem)item).qualityIcon.visible = true;
        ((UI_PetTalentItem)item).qualityCtrl.selectedIndex = unit.Quality - 1;
        ((UI_PetTalentItem)item).icon = UIResource.GetPetTalentIcon(unit.Item.ToString());//天赋图标
        ((UI_PetTalentItem)item).talentName.visible = true;
        ((UI_PetTalentItem)item).talentName.text = ConfigUtils.GetTextById(unit.Name);
        ((UI_PetTalentItem)item).data = unit.Id;
        ((UI_PetTalentItem)item).data = _petTalentList[index];
        ((UI_PetTalentItem)item).onClick.Set(this.OnClickTalentItem);
    }
    
    private void OnClickTalentItem(EventContext context)
    {
        // int talentId = (int)(context.sender as UI_PetTalentItem)?.data;
        // UIManager.Instance.ShowUIPanel("PetSkillTips", 1, talentId);
        
        PetTalent talent = (context.sender as UI_PetTalentItem)?.data as  PetTalent;
        UIManager.Instance.ShowUIPanel("PetSkillTips", 1, talent);
    }

    private void PetBookListRender(int index, GObject item)
    {
        // 解锁且有装备
        if (_bookSlotList[index].BookId != 0 && _bookSlotList[index].SlotStatus == eSlotStatus.eSlotStatus_Normal)
        {
            var unit = ConfigUtils.GetPetSkillBookUnitById(_bookSlotList[index].BookId);
            // ((UI_PetBookItem)item).icon = UIResource.GetItemUrl(unit.Icon.ToString());
            ((UI_PetBookItem)item).icon = UIResource.GetPetSkillBookIcon(unit.Icon.ToString());
            ((UI_PetBookItem)item).name.visible = true;
            ((UI_PetBookItem)item).name.text = ConfigUtils.GetTextById(unit.Name);
            ((UI_PetBookItem)item).qualityIcon.visible = true;
            ((UI_PetBookItem)item).qualityCtrl.selectedIndex = unit.Quality - 1;
            ((UI_PetBookItem)item).lockCtrl.selectedIndex = 2;
            // ((UI_PetBookItem)item).data = unit.Id;
            ((UI_PetBookItem)item).data = _bookSlotList[index];
            ((UI_PetBookItem)item).onClick.Set(this.OnClickSkillBookItem);
        }

        // 未解锁
        if (_bookSlotList[index].SlotStatus == eSlotStatus.eSlotStatus_Locked)
        {
            ((UI_PetBookItem)item).lockCtrl.selectedIndex = 1;
            ((UI_PetBookItem)item).onClick.Set(this.OnClickSkillBookItem2);
        }

        // 解锁且没有装备
        if (_bookSlotList[index].BookId == 0 &&  _bookSlotList[index].SlotStatus == eSlotStatus.eSlotStatus_Normal) 
        {
            ((UI_PetBookItem)item).lockCtrl.selectedIndex = 0;
            ((UI_PetBookItem)item).onClick.Set(this.OnClickSkillBookItem3);
        }
        
    }
    
    private void OnClickSkillBookItem(EventContext context)
    {
        PetSkillBookSlot skillBookSlot = (context.sender as UI_PetBookItem)?.data as PetSkillBookSlot;

        if (skillBookSlot == null)
            return;
        
        UIManager.Instance.ShowUIPanel("PetSkillTips", 2, skillBookSlot);
    }

    private void OnClickSkillBookItem2()
    {
        UIManager.Instance.Toast("未解锁！");
    }
    
    private void OnClickSkillBookItem3()
    {
        UIManager.Instance.Toast("未装备技能书！");
    }
    
}
