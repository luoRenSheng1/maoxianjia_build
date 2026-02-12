using CommonEx;
using Config;
using Engine;
using EngineBase;
using msg;
using Village;

public class VillagePetInfoView : UIViewBase
{
    private  UI_PetInfo PetInfo => this.main as UI_PetInfo;
    
    private PetItemInfo _curPet;
    private CityInfo _buildInfo;
    private bool _isGuiding;
    public VillagePetInfoView()
    {
        this.name = "VillagePetInfo";
        this.package = "Village";
        this.component = "PetInfo";
        this.safeAreaInset = true;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        VillageBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _curPet = values[0] as PetItemInfo;
        _buildInfo = values[1] as CityInfo;
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.PetInfo.closeBtn.onClick.Add(this.Hide);
        this.PetInfo.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.PetInfo.uparrayBtn.onClick.Add(this.OnClickUploadBtn);
        this.PetInfo.downarrayBtn.onClick.Add(this.OnClickDownBtn);
        this.PetInfo.replaceBtn.onClick.Add(this.OnClickReplaceBtn);
    }

    protected override void OnShow()
    {
        base.OnShow();
        
        ((UI_PetCom) this.PetInfo.petCom).qualityCtrl.selectedIndex = _curPet.petCfg.Quality - 1;
        // ((UI_PetCom) this.PetInfo.petCom).petIcon.url = UIResource.GetPetIcon(_curPet.petCfg.IconPath);
        ((UI_PetCom) this.PetInfo.petCom).petIcon.url = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeByParam(int.Parse(_curPet.petCfg.IconPath)).Icon);
        // this.PetInfo.petName.text = _curPet.petCfg.Name;
        this.PetInfo.petName.text = ConfigUtils.GetTextById(_curPet.petCfg.Name);
        this.PetInfo.petLv.SetVar("value", _curPet.PetLv.ToString()).FlushVars();
        ((UI_roleQualityItem)this.PetInfo.petQ).quality.selectedIndex = _curPet.petCfg.Quality - 1;
        int addValue = VillageInfoManager.Instance.GetVillagePetAdd(_curPet.petCfg.Quality);
        this.PetInfo.jcLb.SetVar("value", (addValue).ToString()).FlushVars();
        ConfigPetLevelUnit petItem = ConfigUtils.GetPetLevelByQualityWithLevel(_curPet.quality, _curPet.PetLv+1);
        if (petItem != null)
        {
            // if (petItem.Item2)
            // {
            //     ((UI_BarExp) this.PetInfo.barExp).maxCtrl.selectedIndex = 1;
            // }
            // else
            // {
            //     ((UI_BarExp) this.PetInfo.barExp).maxCtrl.selectedIndex = 0;
            //     this.PetInfo.barExp.min = 0;
            //     this.PetInfo.barExp.max = petItem.Item1.CardNumber;
            //     this.PetInfo.barExp.value = _curPet.CardNumber;
            // }
            
            // 目前宠物没有经验条了，为了满足UI上的需求
            ((UI_BarExp) this.PetInfo.barExp).maxCtrl.selectedIndex = 0;
            this.PetInfo.barExp.min = 0;
            this.PetInfo.barExp.max = 1;
            this.PetInfo.barExp.value = 0;

        }
        
        this.PetInfo.upOrDown.selectedIndex = _curPet.DispatchBuild != VillageBuildType.None ? 0 : 1;
        if (this.PetInfo.upOrDown.selectedIndex == 1)
        {
            this.PetInfo.upOrDown.selectedIndex =
                VillageInfoManager.Instance.GetNoUploadIndex(_buildInfo.BuildType) == -1 ? 2 : 1;
        }

        if (_buildInfo.BuildType == VillageBuildType.Factory && GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.BuildFactory,
            giding = GuideID.guideId_4202,
            gid = GuideID.guideId_4203,
            tui = this.PetInfo.uparrayBtn,
            isForce = true,
            isSend = true,
            pType = PosType.Left,
            //npcTxt = "Beginner_Doc_002",
            //npcPosType = PosType.Down,
            touchCB = () =>
            {
                GuideManager.Instance.HideGuide();
            }
        })) { _isGuiding = true; }

        if (!_isGuiding && _buildInfo.BuildType == VillageBuildType.Stone && GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.BuildStone,
            giding = GuideID.Trigger_Click_VillageStoneAddPet,
            gid = GuideID.Trigger_Click_VillageStoneUploadPet,
            tui = this.PetInfo.uparrayBtn,
            isForce = true,
            isSend = true,
            pType = PosType.Left,
            //npcTxt = "Beginner_Doc_002",
            //npcPosType = PosType.Down,
        })) { _isGuiding = true; }
    }

    private void OnClickUploadBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        int index = VillageInfoManager.Instance.GetNoUploadIndex(_buildInfo.BuildType);
        if (index != -1)
        {
            var builder = PetDispatched_CS.CreateBuilder();
            builder.Build = (eBuildType) _buildInfo.BuildType;
            var buildPetSlot = BuildPetSlot.CreateBuilder();
            buildPetSlot.Index = (uint) index;
            buildPetSlot.PetId = (ulong) _curPet.PetGuid;
            builder.PetIdsList.Add(buildPetSlot.Build());
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_PetDispatched_CS, builder.Build());
        }
    }

    private void OnClickDownBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        if (_curPet != null)
        {
            var builder = PetDispatched_CS.CreateBuilder();
            builder.Build = (eBuildType) _buildInfo.BuildType;
            var buildPetSlot = BuildPetSlot.CreateBuilder();
            buildPetSlot.Index = (uint) _curPet.BuildInnerIndex;
            buildPetSlot.PetId = 0;
            builder.PetIdsList.Add(buildPetSlot.Build());
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_PetDispatched_CS, builder.Build());
        }

    }

    private void OnClickReplaceBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_PET_REPLACE_PET, _curPet);
    }

    protected override void OnHide()
    {
        base.OnHide();
        if(_isGuiding)
            GuideManager.Instance.HideGuide();
    }
}
