using CommonEx;
using Config;
using Engine;
using EngineBase;
using msg;
using RoleMain;
using Village;
using PetBattleAttr = Engine.PetBattleAttr;

public class HintView : UIViewBase
{
    private UI_Hint PetDetail => this.main as UI_Hint;
    
    private PetItemInfo _curPet;
    private CityInfo _buildInfo;

    public HintView()
    {
        this.name = "Hint";
        this.package = "Village";
        this.component = "Hint";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        RoleMainBinder.BindAll();
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
        this.PetDetail.closeBtn2.onClick.Add(this.Hide);
        this.PetDetail.yesBtn.onClick.Add(this.OnClickUploadBtn);
        this.PetDetail.noBtn.onClick.Add(this.Hide);
    }

    protected override void OnShow()
    {
        base.OnShow();
        
        PetItemInfo hasPetInfo = PetInfoManager.Instance.GetPet(_curPet.PetGuid);
        if (hasPetInfo != null)
        {
            _curPet = hasPetInfo;
        }
    }

    private void OnClickUploadBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        int index = VillageInfoManager.Instance.GetNoUploadIndex(_buildInfo.BuildType);
        if (index != -1)
        {
            var builder = PetSwitchBetweenBuilds_CS.CreateBuilder();
            builder.SrcBuild = (eBuildType) _curPet.DispatchBuild;
            var buildPetSlot1 = BuildPetSlot.CreateBuilder();
            buildPetSlot1.Index = (uint) _curPet.BuildInnerIndex;
            // buildPetSlot1.PetId = (uint) _curPet.petCfg.Id;
            buildPetSlot1.PetId = (ulong) _curPet.PetGuid;
            builder.SrcPetIds = buildPetSlot1.Build();

            builder.DstBuild = (eBuildType) _buildInfo.BuildType;
            var buildPetSlot2 = BuildPetSlot.CreateBuilder();
            buildPetSlot2.Index = (uint) index;
            // buildPetSlot2.PetId = (uint) _curPet.petCfg.Id;
            buildPetSlot2.PetId = (ulong) _curPet.PetGuid;
            builder.DstPetIds = buildPetSlot2.Build();
            
            VillageInfoManager.Instance.SetVillagePetByBuild(_curPet,VillageBuildType.None);
            
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_PetSwitchBetweenBuilds_CS, builder.Build());
        }
        else
        {
            OnClickReplaceBtn();
        }
    }

    private void OnClickReplaceBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_PET_REPLACE_PET, _curPet);
    }
}
