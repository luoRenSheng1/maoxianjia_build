
using Config;
using Engine;
using msg;
using RoleMain;

public class RuneRecycleView : UIViewBase
{
    private UI_RuneRecyle Recyle => this.main as UI_RuneRecyle;
    
    private int _useCount = 1;
    private int _maxCnt = 0;
    public RuneRecycleView()
    {
        this.name = "RuneRecycle";
        this.package = "RoleMain";
        this.component = "RuneRecyle";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        RoleMainBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.Recyle.closeBtn.onClick.Add(this.Hide);
        this.Recyle.addBtn.onClick.Add(this.OnClickAddBtn);
        this.Recyle.reduceBtn.onClick.Add(this.OnClickReduceBtn);
        this.Recyle.recyleBtn.onClick.Add(this.OnClickRecycleBtn);
        this.Recyle.cntLb.onChanged.Add(this.OnChangeCntLb);
        _maxCnt = 99999999;
    }

    protected override void OnShow()
    {
        base.OnShow();
        _useCount = (int) DataManager.Instance.GetRoleData().runeRecycleNum;
        UpdateCntTxt();
    }
    
    private void OnClickAddBtn()
    {
        if (_maxCnt > _useCount)
        {
            _useCount++;
            UpdateCntTxt();
        }
    }
    
    private void OnClickReduceBtn()
    {
        if (_useCount > 1)
        {
            _useCount--;
            UpdateCntTxt();
        }
    }
    
    private void UpdateCntTxt()
    {
        this.Recyle.cntLb.text = _useCount.ToString();
        this.Recyle.addBtn.enabled = _useCount <_maxCnt;
        this.Recyle.reduceBtn.enabled = _useCount > 1;
    }

    private void OnChangeCntLb()
    {
        _useCount = int.Parse(this.Recyle.cntLb.text);
        UpdateCntTxt();
    }

    private void OnClickRecycleBtn()
    {
        ConfigStageUnit stageUnit = ConfigUtils.GetStageUnitByIdAndNode(MapObjectManager.Instance.GuanKaStageId, MapObjectManager.Instance.GuanKaMonsterIndex);
        var builder = RunesSell_CS.CreateBuilder();
        builder.AtkCounter = (uint) _useCount;
        builder.StageNode = (uint) stageUnit.Node;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_RunesSell_CS, builder.Build());
        
        SetVisible(false);
    }
}
