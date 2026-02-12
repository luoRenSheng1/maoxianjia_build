
using System.Collections.Generic;
using System.Linq;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Setting;
using AvatarInfo = Engine.AvatarInfo;
using EventDispatcher = EngineBase.EventDispatcher;

public class SettingMainView : UIViewBase
{
    private UI_SettingMain Setting => this.main as UI_SettingMain;
    private List<ConfigHeroUnit> _heros;
    private List<HeroInfo> hasHeros;
    private bool _isSelected = false;
    private string iconUrl;
    private List<ConfigItemTypeUnit> _itemTypeUnits = new List<ConfigItemTypeUnit>();
    private List<AvatarInfo> _avatarList;
    private int _avatarId;
    private List<int> unlockedAvatarIds = new List<int>();

    public SettingMainView()
    {
        this.name = "SettingMain";
        this.package = "Setting";
        this.component = "SettingMain";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        SettingBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.Setting.closeBtn.onClick.Add(this.Hide);
        this.Setting.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.Setting.editBtn.onClick.Add(this.OnClickEditPlayerName);
        this.Setting.problemBtn.onClick.Add(this.OnClickProblemBtn);
        this.Setting.selectServerBtn.onClick.Add(this.OnClickServerBtn);
        this.Setting.exchangeCodeBtn.onClick.Add(this.OnClickExchangeCode);
        this.Setting.gameAnnounceBtn.onClick.Add(this.OnClickGameAnnounceBtn);
        this.Setting.musicBar.onGripTouchEnd.Add(this.MusicBarChange);
        this.Setting.soundBar.onGripTouchEnd.Add(this.SoundBarChange);
        this.Setting.musicCheck.onChanged.Add(this.OnMusicCheck);
        this.Setting.soundCheck.onChanged.Add(this.OnSouncCheck);
        this.Setting.changeIconBtn.onClick.Add(this.OnClickChangeIcon);
        this.Setting.btnRecord.onClick.Add(this.OnClickFilingRecord);
        this.Setting.linkLb.onClickLink.Add(this.OnClickLinkTxt);
        
        this.Setting.musicBar.value = GameManager.Instance.SoundManager.musicVolume*100f;
        this.Setting.soundBar.value = GameManager.Instance.SoundManager.soundVolume*100f;
        
        _itemTypeUnits = ConfigDataGroup.GetInstance<ConfigItemType>().Data.Values.Where(type => type.Type == 9).ToList();// 所有头像
        _avatarList = RoleManager.Instance.GetAvatarList();// 已解锁头像列表
        unlockedAvatarIds = RoleManager.Instance.GetAvatarList().Select(avatar => avatar.Id).ToList();
        SortAvatarList();//所有头像排序
        this.Setting.headIconList.itemRenderer = HeadIconRender;
        this.Setting.headIconList.onClickItem.Add(this.OnClickHeadIconListItem);
        _heros = ConfigDataGroup.GetInstance<ConfigHero>().Data.Values.ToList();
        hasHeros = HeroInfoManager.Instance.GetHeroInfos();

        this.Setting.useBtn.onClick.Add(this.OnClickUseHead);// 使用头像按钮
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CHANGENAMECOUNTER, this.ChangePlayerNameSucc);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_AVATARS_UPDATE, this.ChangeHeadIconSucc);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_AVATARS_UPDATE, this.UpdateHeadIcon);

        //TODO 多语言选择
        List<string> languageNames = new List<string>();
        int index = 0;
        for (int i = 0; i < GameManager.Instance.languageList.Count; i++)
        {
            index = GameManager.Instance.languageList[i];
            languageNames.Add(GameManager.Instance.GetTextNameByIdWithIndex("Language", index));
        }
        this.Setting.lang.selectedIndex = 0;
        this.Setting.LangChangePop.items = languageNames.ToArray();
        this.Setting.LangChangePop.selectedIndex = GameManager.Instance.languageIndex;
        this.Setting.LangChangePop.onChanged.Add(OnClickComboBox);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CHANGENAMECOUNTER, this.ChangePlayerNameSucc);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_AVATARS_UPDATE, this.ChangeHeadIconSucc);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_AVATARS_UPDATE, this.UpdateHeadIcon);
    }

    protected override void OnShow()
    {
        base.OnShow();
        MusicBarChange();
        SoundBarChange();
        var roleData = DataManager.Instance.GetRoleData();
        this.Setting.playerNameLb.text = roleData.userName;
        // this.Setting.vipLb.title = roleData.vipLv.ToString();
        this.Setting.fightLb.text = StringUtils.FormatCurrency(RoleManager.Instance.TotalFight);
        this.Setting.headIcon.icon = UIResource.GetItemUrl(roleData.GetAvatarUrl());
        // this.Setting.headIcon.icon = UIResource.GetHeroBody(iconUrl);
        this.Setting.uidLb.text = roleData.userID.ToString();
        this.Setting.serverName.text = "1";
        this.Setting.appVersionLb.SetVar("value", VersionManager.Instance.GetAppVersion());
        this.Setting.txtRecord.text = ConstDefine.FILING_TEXT;
        
        RoleManager.Instance.SendToGetAvatarListCS();
        UpdateHeadIcon();
    }

    private void OnClickEditPlayerName()
    {
        UIManager.Instance.ShowUIPanel("PlayerNameEdit");
    }
    private void MusicBarChange()
    {
        float v = (float) this.Setting.musicBar.value;
        GameManager.Instance.SoundManager.musicVolume = v/100f;
        this.Setting.musicCheck.selected = this.Setting.musicBar.value > 1;
    }

    private void SoundBarChange()
    {
        float v = (float) this.Setting.soundBar.value;
        GameManager.Instance.SoundManager.soundVolume = v/100f;
        this.Setting.soundCheck.selected = this.Setting.soundBar.value > 1;
    }

    private void OnMusicCheck()
    {
        this.Setting.musicBar.value = this.Setting.musicCheck.selected ? 100 : 0;
        GameManager.Instance.SoundManager.musicVolume = (float)(this.Setting.musicBar.value)/100f;
    }

    private void OnSouncCheck()
    {
        this.Setting.soundBar.value = this.Setting.soundCheck.selected ? 100 : 0;
        GameManager.Instance.SoundManager.soundVolume = (float)(this.Setting.soundBar.value)/100f;
    }

    private void OnClickGameAnnounceBtn()
    {
        UIManager.Instance.ShowUIPanel("Notice");
    }

    private void OnClickExchangeCode()
    {
        UIManager.Instance.ShowUIPanel("ExchangeCode");
    }

    private void OnClickServerBtn()
    {
        UIManager.Instance.ToastByKey(206);
        // UIManager.Instance.ShowUIPanel("ServerSelect");
    }

    private void OnClickProblemBtn()
    {
        UIManager.Instance.ToastByKey(5056);
        // UIManager.Instance.ShowUIPanel("ProblemEdit");
    }

    private void ChangeHeadIconSucc()
    {
        if (IsShow() && IsOnStage())
        {
            var roleData = DataManager.Instance.GetRoleData();
            this.Setting.headIcon.icon = UIResource.GetItemUrl(roleData.GetAvatarUrl());
        }
    }

    private void ChangePlayerNameSucc()
    {
        if (IsShow() && IsOnStage())
        {
            var roleData = DataManager.Instance.GetRoleData();
            this.Setting.playerNameLb.text = roleData.userName;
        }
    }

    private void SortAvatarList()
    {
        // 获取已解锁的头像ID列表
        unlockedAvatarIds = RoleManager.Instance.GetAvatarList().Select(avatar => avatar.Id).Distinct().ToList();
        // 将所有头像（包括未解锁的）排序
        _itemTypeUnits.Sort((a, b) =>
        {
            bool isAUnlocked = unlockedAvatarIds.Contains(a.Id);
            bool isBUnlocked = unlockedAvatarIds.Contains(b.Id);

            // 1. 已解锁的排在未解锁的前面
            if (isAUnlocked != isBUnlocked)
            {
                return isAUnlocked ? -1 : 1;
            }

            // 2. 相同解锁状态下，按品质从小到大排序
            int qualityCompare = a.Quality.CompareTo(b.Quality);
            if (qualityCompare != 0)
            {
                return qualityCompare;
            }

            // 3. 品质相同则按ID从小到大排序
            return a.Id.CompareTo(b.Id);
        });
    }

    private void HeadIconRender(int index, GObject item)
    {
        SortAvatarList();
        ((UI_HeadIcon) item).icon = UIResource.GetItemUrl(_itemTypeUnits[index].Icon);
        // List<AvatarInfo> list = RoleManager.Instance.GetAvatarList();//已解锁的头像列表
        ((UI_HeadIcon) item).status.selectedIndex = 0;//头像未解锁

        foreach (var i in _avatarList)
        {
            // 如果解锁了该头像
            if (i.Id == _itemTypeUnits[index].Id)
            {
                ((UI_HeadIcon) item).status.selectedIndex = 1;//头像已解锁
            }
        }
        
        if (DataManager.Instance.mRoleData.avatarID == _itemTypeUnits[index].Id)
        {
            ((UI_HeadIcon) item).isUse.selectedIndex = 1;
        }
        else
        {
            ((UI_HeadIcon) item).isUse.selectedIndex = 0;
        }
        
        // ((UI_HeadIcon) item).img.url = UIResource.GetImageUrlWithLang("shiyongzhong", "Common");
    }

    // 获取头像列表中，单个item的信息
    private void OnClickHeadIconListItem(EventContext context)
    {
        UI_HeadIcon item = context.data as UI_HeadIcon;
        int index = this.Setting.headIconList.GetChildIndex(item);
        _avatarId = _itemTypeUnits[index].Id;
    }

    private void UpdateHeadIcon()
    {
        this.Setting.headIconList.numItems = _itemTypeUnits.Count;
    }

    private void OnClickChangeIcon()
    {
        if (_isSelected)
        {
            this.Setting.type.selectedIndex = 0;
        }
        else
        {
            this.Setting.type.selectedIndex = 1;
        }

        _isSelected = !_isSelected;
    }

    private void OnClickFilingRecord()
    {
        GameManager.Instance.OpenLinkURL(ConstDefine.URL_FILING);
    }
    
    private void OnClickLinkTxt(EventContext context)
    {
        GRichTextField t = context.sender as GRichTextField;
        string[] eventData = ((string) context.data).Split(":");
        if (eventData[1] == "userTips")
        {
            GameManager.Instance.OpenLinkURL(ConstDefine.URL_USER);
        }else if (eventData[1] == "PrivacyTips")
        {
            GameManager.Instance.OpenLinkURL(ConstDefine.URL_PRIVACY);
        }
    }
    
    // 更换头像
    private void OnClickUseHead()
    {
        if (_avatarId != null && unlockedAvatarIds.Contains(_avatarId))
        {
            // 更换头像
            var builder = SetAvatar_CS.CreateBuilder();
            builder.Id = (uint)_avatarId;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_SetAvatar_CS, builder.Build());
        }
        else
        {
            UIManager.Instance.ToastByKey(10008);
        }
    }

    private void OnClickComboBox()
    {
        GameManager.Instance.SetLanguage(this.Setting.LangChangePop.selectedIndex);
        UIManager.Instance.DestroyController(this);
    }
}
