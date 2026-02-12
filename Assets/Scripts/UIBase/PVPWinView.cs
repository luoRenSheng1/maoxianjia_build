
using BestHTTP.Extensions;
using Engine;
using PVPMap;

public class PVPWinView : UIViewBase
{
    private UI_PVPWin PvpWin => this.main as UI_PVPWin;
    
    private string _myName;
    private string _myHeadIcon;
    private string _otherName;
    private string _otherHeadIcon;
    public PVPWinView()
    {
        this.name = "PVPWin";
        this.package = "PVPMap";
        this.component = "PVPWin";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        PVPMapBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _myName = (string) values[0];
        _myHeadIcon = (string) values[1];
        _otherName = (string) values[2];
        _otherHeadIcon = (string) values[3];
    }
    
    protected override void OnInit()
    {
        base.OnInit();
        this.PvpWin.closeBtn.onClick.Add(this.Hide);
    }

    protected override void OnShow()
    {
        base.OnShow();
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralWinSE);
        // this.PvpWin.myHeadIcon.icon = _myHeadIcon;
        this.PvpWin.myHeadIcon.icon = UIResource.GetItemUrl(DataManager.Instance.GetRoleData().GetAvatarUrl());
        this.PvpWin.myNameLb.text = _myName;
        // this.PvpWin.enemyHeadIcon.icon = _myHeadIcon;//_otherHeadIcon;
        this.PvpWin.enemyHeadIcon.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(_otherHeadIcon.ToInt32()).Icon);
        this.PvpWin.enemyNameLb.text = _otherName;
        // this.PvpWin.img.url = UIResource.GetImageUrlWithLang("zdsl", "PVPMap");
    }
}
