using System;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using msg;
using RoleMain;
using RuneInfo = Engine.RuneInfo;

public class RuneDetailView : UIViewBase
{
    private UI_RuneDetail RuneDetail => this.main as UI_RuneDetail;
    
    private RuneInfo _curRuneInfo;

    private ConfigCommonUnit _common500001;
    private ConfigCommonUnit _common500002;
    private ConfigCommonUnit _common500003;
    private ConfigCommonUnit _common500004;
    private ConfigCommonUnit _common500005;
    
    private bool isTips = false;
    
    public RuneDetailView()
    {
        this.name = "RuneDetail";
        this.package = "RoleMain";
        this.component = "RuneDetail";
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
        _curRuneInfo = values[0] as RuneInfo;
        if (values.Length == 2)
            isTips = (bool) values[1];
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.RuneDetail.closeBtn.onClick.Add(this.Hide);
        this.RuneDetail.uploadBtn.onClick.Add(this.OnClickUploadBtn);
        this.RuneDetail.replaceBtn.onClick.Add(this.OnClickReplaceBtn);
        this.RuneDetail.downBtn.onClick.Add(this.OnClickDownBtn);
        this.RuneDetail.RecycleBtn.onClick.Add(this.OnClickRecycleBtn);

        _common500001 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(500001);
        _common500002 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(500002);
        _common500003 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(500003);
        _common500004 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(500004);
        _common500005 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(500005);
    }

    protected override void OnShow()
    {
        base.OnShow();
        
        RuneInfo hasRuneInfo = RuneInfoManager.Instance.GetRune(_curRuneInfo.Guid);
        if (hasRuneInfo != null)
        {
            _curRuneInfo = hasRuneInfo;
        }
        ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(_curRuneInfo.SkillId);
        ((UI_ItemCom) this.RuneDetail.runeItem).ctrlQuality.selectedIndex = skillUnit.SkillQuality - 1;
        ((UI_ItemCom) this.RuneDetail.runeItem).icon = UIResource.GetItemUrl(_curRuneInfo.ItemTypeUnit.Icon);
        // this.RuneDetail.runeName.text = _curRuneInfo.ItemTypeUnit.Name;
        this.RuneDetail.runeName.text = ConfigUtils.GetTextById(_curRuneInfo.ItemTypeUnit.Name);
        // ((UI_petQualityItem)this.RuneDetail.petQ).quality.selectedIndex = _curRuneInfo.Quality - 1;
        if (hasRuneInfo != null)
        {
            bool isUpload = RuneInfoManager.Instance.IsInUpload(hasRuneInfo);
            if (isUpload)
            {
                this.RuneDetail.ctrl.selectedIndex = 0;
            }
            else
            {
                //判断还有没有上阵位置，有的话直接上阵，否则要替换
                bool isAll = RuneInfoManager.Instance.GetBattleRuneList().Count == RuneInfoManager.Instance.UnLockRunePos;
                if (isAll)
                {
                    this.RuneDetail.ctrl.selectedIndex = 2;
                }
                else
                {
                    this.RuneDetail.ctrl.selectedIndex = 1;
                }
            }

        }

        ConfigStageUnit stageUnit = ConfigUtils.GetStageUnitByIdAndNode(MapObjectManager.Instance.GuanKaStageId, MapObjectManager.Instance.GuanKaMonsterIndex);
        ((UI_RecycleBtn) this.RuneDetail.RecycleBtn).recyleMoney.text = StringUtils.FormatCurrency(
            Math.Ceiling(double.Parse(stageUnit.Gold) * GetRuneGoldByQuality(_curRuneInfo.Quality) * ConstDefine.CONFIG_PLACE_EX));
        
        // this.RuneDetail.skillDesc.SetVar("skillName", skillUnit.Name).SetVar("value", _curRuneInfo.MagicTimes.ToString()).FlushVars();
        this.RuneDetail.skillDesc.SetVar("skillName", ConfigUtils.GetTextById(skillUnit.Name)).SetVar("value", _curRuneInfo.MagicTimes.ToString()).FlushVars();
        this.RuneDetail.sortingOrder = isTips ? 999 : 0;
        this.RuneDetail.isTips.selectedIndex = isTips ? 1 : 0;

    }

    protected override void OnHide()
    {
        base.OnHide();
        isTips = false;
    }

    private int GetRuneGoldByQuality(int quality)
    {
        switch (quality)
        {
            case 3:
                return int.Parse(_common500001.Param1);
                break;
            case 4:
                return int.Parse(_common500002.Param1);
                break;
            case 5:
                return int.Parse(_common500003.Param1);
                break;
            case 6:
                return int.Parse(_common500004.Param1);
                break;
            case 7:
                return int.Parse(_common500005.Param1);
                break;
            default:
                return int.Parse(_common500001.Param1);
                break;
        }
    }

    private void OnClickUploadBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        int index = RuneInfoManager.Instance.GetNoUploadIndex();
        if (index != -1)
        {
            if (RuneInfoManager.Instance.HasSameRuneInfo(_curRuneInfo))
            {
                UIManager.Instance.ToastByKey(10190);
                return;
            }
            // GameManager.Instance.SoundManager.PlayEffectWithoutLoop(18);
            var builder = RuneInSlot_CS.CreateBuilder();
            builder.RuneGuid = _curRuneInfo.Guid;
            builder.SlotId = (uint)index;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_RuneInSlot_CS, builder.Build());
        }
        else
        {
            UIManager.Instance.Toast("该位置已经有符石了!");
        }
    }

    private void OnClickReplaceBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPLOAD_RUNE, _curRuneInfo);
    }

    private void OnClickDownBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        RuneInfo rune = RuneInfoManager.Instance.GetRune(_curRuneInfo.Guid);
        if (rune != null)
        {
            var builder = RemoveRuneFromSlot_CS.CreateBuilder();
            builder.SlotId = (uint)rune.BattleIndex;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_RemoveRuneFromSlot_CS, builder.Build());
        }

    }

    private void OnClickRecycleBtn()
    {
        ConfigStageUnit stageUnit = ConfigUtils.GetStageUnitByIdAndNode(MapObjectManager.Instance.GuanKaStageId, MapObjectManager.Instance.GuanKaMonsterIndex);
        var builder = RunesSell_CS.CreateBuilder();
        builder.RuneGuidList.Add(_curRuneInfo.Guid);
        builder.StageNode = (uint) stageUnit.Node;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_RunesSell_CS, builder.Build());
        
        SetVisible(false);
    }

}
