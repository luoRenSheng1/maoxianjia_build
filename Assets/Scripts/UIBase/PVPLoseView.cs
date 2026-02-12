using BestHTTP.Extensions;
using Engine;
using PVPMap;

public class PVPLoseView : UIViewBase
{
    private UI_PVPLose PvpLose => this.main as UI_PVPLose;

    private string _myName;
    private string _myHeadIcon;
    private string _otherName;
    private string _otherHeadIcon;
    public PVPLoseView()
    {
        this.name = "PVPLose";
        this.package = "PVPMap";
        this.component = "PVPLose";
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
        this.PvpLose.closeBtn.onClick.Add(this.Hide);
    }

    protected override void OnShow()
    {
        base.OnShow();
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralLoseSE);
        // this.PvpLose.myHeadIcon.icon = _myHeadIcon;
        this.PvpLose.myHeadIcon.icon = UIResource.GetItemUrl(DataManager.Instance.GetRoleData().GetAvatarUrl());
        this.PvpLose.myNameLb.text = _myName;
        // this.PvpLose.enemyHeadIcon.icon = _myHeadIcon;//_otherHeadIcon;
        this.PvpLose.enemyHeadIcon.icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(_otherHeadIcon.ToInt32()).Icon);
        this.PvpLose.enemyNameLb.text = _otherName;
        // this.PvpLose.img.url = UIResource.GetImageUrlWithLang("zdsb","PVPMap");
    }
}
