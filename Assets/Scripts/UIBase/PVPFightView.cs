using System.Collections.Generic;
using CommonEx;
using Engine;
using FairyGUI;
using PVPMap;

public class PVPFightView : UIViewBase
{
    private UI_PVPFight FightView => this.main as UI_PVPFight;

    private int _myHeroId;
    private int _enemyHeroId;
    private List<PetItemInfo> _myPetList;
    private List<PetItemInfo> _enemyPetList;
    private double _myFight;
    private double _enemyFight;
    private string _myName;
    private string _otherName;
    public PVPFightView()
    {
        this.name = "PVPFight";
        this.package = "PVPMap";
        this.component = "PVPFight";
        this.removePackage = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        PVPMapBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _myHeroId = (int) values[0];
        _enemyHeroId = (int) values[1];
        _myPetList = values[2] as List<PetItemInfo>;
        _enemyPetList = values[3] as List<PetItemInfo>;
        _myFight = (double) values[4];
        _enemyFight = (double) values[5];
        _myName = (string) values[6];
        _otherName = (string) values[7];
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.FightView.myPetList.itemRenderer = MyPetListItemRender;
        this.FightView.enemyPetList.itemRenderer = EnemyPetListItemRender;
    }

    protected override void OnShow()
    {
        base.OnShow();
        Utils.PlaySpineAnim(this.FightView.spine, "idle", false);
        
        string strResName = ConfigUtils.GetHeroModelPathByID(_myHeroId);
        Utils.SetSpineModelOnFGUI(this.FightView.hero, strResName, 140);
        
        string strResName2 = ConfigUtils.GetHeroModelPathByID(_enemyHeroId);
        Utils.SetSpineModelOnFGUI(this.FightView.monster, strResName2, 140, "idle",null, true);
        
        this.FightView.myFightLb.text = StringUtils.FormatCurrency(_myFight);
        this.FightView.enemyFightLb.text = StringUtils.FormatCurrency(_enemyFight);

        this.FightView.myPetList.numItems = _myPetList.Count;
        this.FightView.enemyPetList.numItems = _enemyPetList.Count;
        
        this.FightView.t0.Play();

        this.FightView.myNameLb.text = _myName;
        this.FightView.otherNameLb.text = _otherName;
    }

    private void MyPetListItemRender(int index, GObject item)
    {
        PetItemInfo petItemInfo = _myPetList[index];
        ((UI_ItemCom) item).ctrlQuality.selectedIndex = petItemInfo.petCfg.Quality - 1;
        ((UI_ItemCom) item).hasCount.selectedIndex = 0;
        ((UI_ItemCom) item).txtLv.text = petItemInfo.PetLv.ToString();
        // ((UI_ItemCom) item).icon = UIResource.GetPetIcon(petItemInfo.petCfg.IconPath);
        ((UI_ItemCom) item).icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeByParam(int.Parse(petItemInfo.petCfg.IconPath)).Icon);
    }

    private void EnemyPetListItemRender(int index, GObject item)
    {
        PetItemInfo petItemInfo = _enemyPetList[index];
        ((UI_ItemCom) item).ctrlQuality.selectedIndex = petItemInfo.petCfg.Quality - 1;
        ((UI_ItemCom) item).hasCount.selectedIndex = 0;
        ((UI_ItemCom) item).txtLv.text = petItemInfo.PetLv.ToString();
        // ((UI_ItemCom) item).icon = UIResource.GetPetIcon(petItemInfo.petCfg.IconPath);
        ((UI_ItemCom) item).icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeByParam(int.Parse(petItemInfo.petCfg.IconPath)).Icon);
    }
}
