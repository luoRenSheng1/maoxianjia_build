using System.Collections.Generic;
using Config;
using Engine;
using FairyGUI;
using Summon;

public class SummonProbabilityView : UIViewBase
{
    private UI_SummonProbability Probability => this.main as UI_SummonProbability;

    /// <summary>
    /// 1=宠物  2=技能
    /// </summary>
    private int _summonType;
    private Dictionary<int, string> _petLotteryDict = new Dictionary<int, string>();
    private int _curLevel;
    private ConfigRaffleLevelUnit _configRaffle;
    private List<ConfigRaffleLevelUnit> _raffleLevelUnits;
    
    public SummonProbabilityView()
    {
        this.name = "SummonProbability";
        this.package = "Summon";
        this.component = "SummonProbability";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        SummonBinder.BindAll();
    }
    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _summonType = (int) values[0];
    }
    protected override void OnInit()
    {
        base.OnInit();
        // this.Probability.closeBtn.onClick.Add(this.Hide);
        this.Probability.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.Probability.preBtn.onClick.Add(this.OnClickPreBtn);
        this.Probability.nextBtn.onClick.Add(this.OnClickNextBtn);
        this.Probability.propList.itemRenderer = ProbLotteryItemRender;
    }

    protected override void OnShow()
    {
        base.OnShow();
        RoleData roleData = DataManager.Instance.GetRoleData();
        if (_summonType == 1)
        {
            _curLevel = roleData.petLotteryLv;
            _raffleLevelUnits = ConfigUtils.GetRaffleUnits(1);
        }
        else if (_summonType == 2)
        {
            _curLevel = roleData.SkillLotteryLv;
            _raffleLevelUnits = ConfigUtils.GetRaffleUnits(2);
        }

        UpdateRaffleProbUI();
    }

    private void UpdateRaffleProbUI()
    {
        this.Probability.lvLb.text = _curLevel.ToString();
        this.Probability.preBtn.visible = _curLevel > 1;
        this.Probability.nextBtn.visible = _curLevel < _raffleLevelUnits.Count;
        if (_summonType == 1)
        {
            _configRaffle = ConfigUtils.GetRaffleUnit(_curLevel, 1);
        }else if (_summonType == 2)
        {
            _configRaffle = ConfigUtils.GetRaffleUnit(_curLevel, 2);
        }
        _petLotteryDict.Clear();
        _petLotteryDict.Add(0, _configRaffle.Quality1Pro);
        _petLotteryDict.Add(1, _configRaffle.Quality2Pro);
        _petLotteryDict.Add(2, _configRaffle.Quality3Pro);
        _petLotteryDict.Add(3, _configRaffle.Quality4Pro);
        _petLotteryDict.Add(4, _configRaffle.Quality5Pro);
        _petLotteryDict.Add(5, _configRaffle.Quality6Pro);
        _petLotteryDict.Add(6, _configRaffle.Quality7Pro);

        this.Probability.propList.numItems = _petLotteryDict.Count;
    }

    private void OnClickPreBtn()
    {
        if (_curLevel > 1)
        {
            _curLevel--;
        }

        UpdateRaffleProbUI();
    }

    private void OnClickNextBtn()
    {
        if (_curLevel < _raffleLevelUnits.Count)
            _curLevel++;
        UpdateRaffleProbUI();
    }
    
    private void ProbLotteryItemRender(int index, GObject item)
    {

        if (index % 2 == 0)
        {
            ((UI_probablyItem)item).ctrl.selectedIndex = 0;
        }
        else
        {
            ((UI_probablyItem)item).ctrl.selectedIndex = 1;
        }
        
        ((UI_probablyItem) item).qualityCtrl.selectedIndex = index;
        ((UI_probablyItem) item).proLb.SetVar("value",(_petLotteryDict[index]).ToString()).FlushVars();
    }
}
