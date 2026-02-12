using CommonEx;
using Config;
using Engine;
using EngineBase;
using msg;
using RoleMain;
using System;
using SkillInfo = Engine.SkillInfo;

public class SkillDetailView : UIViewBase
{
    private UI_SkillDetail SkillDetail => this.main as UI_SkillDetail;
    
    private SkillInfo _curSkillInfo;
    private bool isTips = false;
    private bool _isGuiding;
    
    public SkillDetailView()
    {
        this.name = "SkillDetail";
        this.package = "RoleMain";
        this.component = "SkillDetail";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        RoleMainBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _curSkillInfo = values[0] as SkillInfo;
        if (values.Length == 2)
            isTips = (bool) values[1];
    }

    protected override void OnInit()
    {
        base.OnInit();
  
        // this.SkillDetail.closeBtn.onClick.Add(this.Hide);
        this.SkillDetail.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.SkillDetail.uploadBtn.onClick.Add(this.OnClickUploadBtn);
        this.SkillDetail.replaceBtn.onClick.Add(this.OnClickReplaceBtn);
        this.SkillDetail.downBtn.onClick.Add(this.OnClickDownBtn);
        this.SkillDetail.getBtn.onClick.Add(this.OnClickGetBtn);
        this.SkillDetail.skilltem.onClick.Add(this.OnClickSkillItem);
    }

    protected override void OnShow()
    {
        base.OnShow();
        
        SkillInfo hasSkillInfo = SkillInfoManager.Instance.GetSkill(_curSkillInfo.SkillId);
        if (hasSkillInfo != null)
        {
            _curSkillInfo = hasSkillInfo;
        }
        
        ((UI_ItemCom) this.SkillDetail.skilltem).ctrlQuality.selectedIndex = _curSkillInfo.SkillUnit.SkillQuality - 1;
        ((UI_ItemCom) this.SkillDetail.skilltem).icon = UIResource.GetItemUrl(_curSkillInfo.SkillUnit.SkillIcon);
        this.SkillDetail.skillName.text = ConfigUtils.GetTextById(_curSkillInfo.SkillUnit.Name);
        double skillDamageRate = 0;
        ConfigSkillLevelUnit levelUnit = ConfigUtils.GetSkillLevelUnit(_curSkillInfo.SkillId, _curSkillInfo.Level);
        skillDamageRate = levelUnit.SkillValue;
        
        this.SkillDetail.skillDesc.text = StringUtils.Format(ConfigUtils.GetTextById(_curSkillInfo.SkillUnit.SkillDes), (skillDamageRate*ConstDefine.CONFIG_PLACE).ToString("f2"));
        
        this.SkillDetail.cdLb.SetVar("value", (_curSkillInfo.SkillUnit.Cd*ConstDefine.CONFIG_PLACE_EX).ToString("")).FlushVars();
        this.SkillDetail.skillLv.SetVar("value", _curSkillInfo.Level.ToString()).FlushVars();
        // ((UI_petQualityItem)this.SkillDetail.petQ).quality.selectedIndex = _curSkillInfo.SkillUnit.SkillQuality - 1;
        ((UI_roleQualityItem)this.SkillDetail.petQ).quality.selectedIndex = _curSkillInfo.SkillUnit.SkillQuality - 1;
        ConfigSkillLevelUnit skillLevelUnit = ConfigUtils.GetSkillLevelUnit(_curSkillInfo.SkillUnit.Id, _curSkillInfo.Level+1);
        if (skillLevelUnit != null)
        {
            this.SkillDetail.petExp.min = 0;
            this.SkillDetail.petExp.max = skillLevelUnit.CardNumber;
            this.SkillDetail.petExp.value = _curSkillInfo.CardNumber;
        }
        
        ConfigSkillLevelUnit skillLevelUnit2 = ConfigUtils.GetSkillLevelUnit(_curSkillInfo.SkillUnit.Id, _curSkillInfo.Level);
        if (skillLevelUnit2 != null)
        {
            // ((UI_CommonAttrItem) this.SkillDetail.pdLb).type.selectedIndex = (int) EN_BUFF_ADD_TYPE.SkillDamage - 1;
            // ((UI_CommonAttrItem) this.SkillDetail.pdLb).pContent.SetVar("value", (Math.Max(_curSkillInfo.CarryAtkValue, skillLevelUnit2.CarryValue)*ConstDefine.CONFIG_PLACE_EX*100f).ToString("f2")).FlushVars();
            // ((UI_CommonAttrItem) this.SkillDetail.qjLb).type.selectedIndex = (int) EN_BUFF_ADD_TYPE.SkillDamage - 1;
            // ((UI_CommonAttrItem) this.SkillDetail.qjLb).pContent.SetVar("value", (Math.Max(_curSkillInfo.OwnerAtkValue, skillLevelUnit2.AttrValue)*ConstDefine.CONFIG_PLACE_EX*100f).ToString("f2")).FlushVars(); 
            ((UI_skillAttrLb) this.SkillDetail.pdLb).pContent.SetVar("name",ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId((int)EN_BUFF_ADD_TYPE.SkillDamage).AttrName)).SetVar("value", (Math.Max(_curSkillInfo.CarryAtkValue, skillLevelUnit2.CarryValue)*ConstDefine.CONFIG_PLACE_EX*100f).ToString("f2")).FlushVars();
            ((UI_skillAttrLb) this.SkillDetail.qjLb).pContent.SetVar("name",ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId((int)EN_BUFF_ADD_TYPE.SkillDamage).AttrName)).SetVar("value", (Math.Max(_curSkillInfo.OwnerAtkValue, skillLevelUnit2.AttrValue)*ConstDefine.CONFIG_PLACE_EX*100f).ToString("f2")).FlushVars();
        }
        
        if (hasSkillInfo != null)
        {
            bool isUpload = SkillInfoManager.Instance.IsInUpload(hasSkillInfo);
            if (isUpload)
            {
                this.SkillDetail.ctrl.selectedIndex = 0;
            }
            else
            {
                //判断还有没有上阵位置，有的话直接上阵，否则要替换
                bool isAll = SkillInfoManager.Instance.GetBattleSkillList().Count == SkillInfoManager.Instance.UnLockSkillPos;
                // bool isAll = SkillInfoManager.Instance.GetBattleSkillList().Count == ConstDefine.PetSkillIndex;
                if (isAll)
                {
                    this.SkillDetail.ctrl.selectedIndex = 2;
                }
                else
                {
                    this.SkillDetail.ctrl.selectedIndex = 1;
                }
            }

        }
        else
        {
            this.SkillDetail.ctrl.selectedIndex = 3;
        }
        this.SkillDetail.sortingOrder = isTips ? 999 : 0;
        this.SkillDetail.tipsCtrl.selectedIndex = isTips ? 1 : 0;
        //引导-点击装备技能
        if (RoleManager.Instance.GetSkillSlotInfoCount() <= 0 && GuideManager.Instance.StarGuideByData(new GuideData()
        {
            giding = GuideID.Click_FirstSkillItem,
            bid = GuideID.Click_SkillIntensify,
            gid = GuideID.Click_EquipSkillItem,
            tui = this.SkillDetail.uploadBtn,
            isForce = true,
            isSend = true,
            isLucency = false
        })) { _isGuiding = true; }
    }

    private void OnClickUploadBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        int index = SkillInfoManager.Instance.GetNoUploadIndex();
        if (index != -1)
        {
            GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralUploadSE);
            var builder = SkillInSlot_CS.CreateBuilder();
            builder.SkillId = (uint) _curSkillInfo.SkillUnit.Id;
            builder.SkillSlotId = (uint)index;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_SkillInSlot_CS, builder.Build());
        }
    }

    private void OnClickReplaceBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPLOAD_SKILL, _curSkillInfo);
    }

    private void OnClickDownBtn()
    {
        UIManager.Instance.CloseUIPanel(this.name);
        SkillInfo skill = SkillInfoManager.Instance.GetSkill(_curSkillInfo.SkillUnit.Id);
        if (skill != null)
        {
            var builder = RemoveSkillFromSlot_CS.CreateBuilder();
            builder.SkillSlotId = (uint)skill.BattleIndex;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_RemoveSkillFromSlot_CS, builder.Build());
        }

    }

    private void OnClickGetBtn()
    {
        LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
        lobbyView?.OpenBottomPanel(5, 0);
    }

    private void OnClickSkillItem()
    {
        UIManager.Instance.ShowUIPanel("SkillEffectPre", _curSkillInfo.SkillUnit.AttackEffect);
    }

    protected override void OnHide()
    {
        base.OnHide();
        isTips = false;
        if (_isGuiding)
        {
            GuideManager.Instance.HideGuide();
        }
        _isGuiding = false;

        var lobby = UIManager.Instance.FindByName("Lobby") as LobbyView;
        //技能开放
        lobby?.ChkGuide();
    }
}
