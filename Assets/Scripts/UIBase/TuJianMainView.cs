
using System.Collections.Generic;
using System.Linq;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Spine.Unity;
using Tujian;
using EventDispatcher = EngineBase.EventDispatcher;

public class TuJianMainView : UIViewBase
{
    private UI_TujianMain _tujianMain => this.main as UI_TujianMain;
    private List<ConfigPetCodexUnit> _petCodexUnits;

    private const int NO_PET = 11000;
    public TuJianMainView()
    {
        this.name = "TuJianMain";
        this.package = "Tujian";
        this.component = "TujianMain";
        this.removePackage = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        TujianBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();

        _tujianMain.tujianList.itemRenderer = TujianListItemRender;
        _tujianMain.tujianList.SetVirtual();
        // _tujianMain.closeBtn.onClick.Add(this.Hide);
        _tujianMain.closeBtn.onClick.Add(this.HideWithSoundEffect);

        _petCodexUnits = ConfigDataGroup.GetInstance<ConfigPetCodex>().Data.Values.ToList();
        _tujianMain.tujianList.numItems = _petCodexUnits.Count;
    }

    protected override void OnShow()
    {
        base.OnShow();
        this._tujianMain.tujianList.ScrollToView(0);
    }
    

    private void TujianListItemRender(int index, GObject item)
    {
        ((UI_TujianPetModelItem) item).petList.itemRenderer = this.PetListItemRender;
        ((UI_TujianPetModelItem) item).bgCtrl.selectedIndex = index % 4;

        ((UI_TujianPetModelItem) item).petList.data = _petCodexUnits[index].PetId.ToList();
        ((UI_TujianPetModelItem) item).petList.numItems = 3;
    }

    private void PetListItemRender(int index, GObject item)
    {
        List<int> lstPet = item.parent.data as List<int>;
        if (lstPet.Count < 3 && index == 2)
        {
            ((UI_TujianPetItem) item).petName.text = "";
            Utils.SetSpineModelOnFGUI(((UI_TujianPetItem) item).spine, "Pet_11000", 75, "idle", (o) =>
            {
            },false);
            return;
        }
        ConfigPetBasisUnit petBasisUnit = ConfigUtils.GetPetById(lstPet[index]);
        bool hasPet = PetInfoManager.Instance.GetPetByPetId(lstPet[index]) != null;
        if (petBasisUnit != null)
        {
            // ((UI_TujianPetItem) item).petName.text = petBasisUnit.Name;
            ((UI_TujianPetItem) item).petName.text = ConfigUtils.GetTextById(petBasisUnit.Name);
            ((UI_TujianPetItem) item).spine.name = "model" + index;
            Utils.SetSpineModelOnFGUI(((UI_TujianPetItem) item).spine, petBasisUnit.PetModel, 75, "idle", (o) =>
            {
                if(o is SkeletonAnimation animation) 
                {
                    if (!hasPet)
                    {
                        Utils.SetSpineColor(animation);// "#24150E");
                    }
                    else
                    {
                        Utils.SetSpineColor(animation, "#ffffff");
                    }
                }
            },false);

        }
     

    }
}
