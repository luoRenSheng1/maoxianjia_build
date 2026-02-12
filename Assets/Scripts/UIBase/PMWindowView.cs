
using System.Collections.Generic;
using System.Linq;
using Common;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;

public class PMWindowView : UIViewBase
{
    private UI_PMWindow pmView => this.main as UI_PMWindow;
    private List<ConfigItemTypeUnit> _lstItemUnits = new List<ConfigItemTypeUnit>();
    private List<ConfigItemTypeUnit> _lstPetUnits = new List<ConfigItemTypeUnit>();
    private List<ConfigItemTypeUnit> _equipUnits = new List<ConfigItemTypeUnit>();
    private List<ConfigSkillUnit> _skillUnits = new List<ConfigSkillUnit>();
    private List<ConfigItemTypeUnit> _lstHeroUnits = new List<ConfigItemTypeUnit>();
    public PMWindowView()
    {
        this.name = "PMWindow";
        this.package = "Common";
        this.component = "PMWindow";
        this.removePackage = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.pmView.closeBtn.onClick.Add(this.Hide);
        this.pmView.closeBtn.onClick.Add(this.HideWithSoundEffect);
        for(int i = 1; i <= 18; i++)
        {
            this.pmView.GetChild("sendBtn" + i).onClick.Add(this.OnClickSendBtn);

        }

        List<string> itemNameList = new List<string>();
        var items = ConfigUtils.GetConfigItemTypeUnits();
        foreach (var item in items)
        {
            if (item.Value.Type != (int) eItemType.eItemType_Equip && item.Value.Type != (int) eItemType.eItemType_Pet)
            {
                // itemNameList.Add(item.Value.Name);
                itemNameList.Add(ConfigUtils.GetTextById(item.Value.Name));
                _lstItemUnits.Add(item.Value);
            }
        }
        this.pmView.itemPop.items = itemNameList.ToArray();

        List<string> petNameList = new List<string>();
        foreach (var item in items)
        {
            if (item.Value.Type == (int) eItemType.eItemType_Pet)
            {
                // petNameList.Add(item.Value.Name);
                petNameList.Add(ConfigUtils.GetTextById(item.Value.Name));
                _lstPetUnits.Add(item.Value);
            }
        }
        this.pmView.petPop.items = petNameList.ToArray();
        
        List<string> equipNameList = new List<string>();
        foreach (var item in items)
        {
            if (item.Value.Type == 2)
            {
                // equipNameList.Add(item.Value.Name);
                equipNameList.Add(ConfigUtils.GetTextById(item.Value.Name));
                _equipUnits.Add(item.Value);
            }
        }
        this.pmView.itemEquip.items = equipNameList.ToArray();
        
        List<string> skillNameList = new List<string>();
        foreach (var item in SkillInfoManager.Instance.GetAllSkillInfoList())
        {
            // skillNameList.Add(item.SkillUnit.Name);
            skillNameList.Add(ConfigUtils.GetTextById(item.SkillUnit.Name));
            _skillUnits.Add(item.SkillUnit);
            
        }
        this.pmView.skillpop.items = skillNameList.ToArray();
        
        List<string> heroNameList = new List<string>();
        foreach (var item in items)
        {
            if (item.Value.Type == (int) eItemType.eItemType_HeroRole)
            {
                // heroNameList.Add(item.Value.Name);
                heroNameList.Add(ConfigUtils.GetTextById(item.Value.Name));
                _lstHeroUnits.Add(item.Value);
            }
        }
        this.pmView.heroPop.items = heroNameList.ToArray();
    }

    private void OnClickSendBtn(EventContext context)
    {
        // if (!GuideManager.Instance.NotShowThisGuide((int) GuideID.Click_EquipPetItem))
        // {
        //     UIManager.Instance.Toast("新手引导期间不要作弊!");
        //     return;
        // }
        GButton send = context.sender as GButton;
        int index = int.Parse(send.name.Substring(7));
        string pmStr = "";
        switch (index)
        {
            case 1:
                if (string.IsNullOrEmpty(this.pmView.pmIpt1.text))
                {
                    return;
                }
                pmStr = "addlevel " + int.Parse(this.pmView.pmIpt1.text);
                break;
            case 2:
                if (string.IsNullOrEmpty(this.pmView.pmIpt2.text))
                {
                    return;
                }
                pmStr = "addgold " + double.Parse(this.pmView.pmIpt2.text);
                break;
            case 3:
                if (string.IsNullOrEmpty(this.pmView.pmIpt3.text))
                {
                    return;
                }
                pmStr = "adddiamond " + int.Parse(this.pmView.pmIpt3.text);
                break;
            case 4:
                if (string.IsNullOrEmpty(this.pmView.pmIpt4.text))
                {
                    return;
                }
                pmStr = "addfdiamond " + int.Parse(this.pmView.pmIpt4.text);
                break;
            case 5:
                if (string.IsNullOrEmpty(this.pmView.pmIpt5.text))
                {
                    return;
                }
                pmStr = "addboxlevel " + int.Parse(this.pmView.pmIpt5.text);
                break;
            case 6:
                if (string.IsNullOrEmpty(this.pmView.pmIpt6.text))
                {
                    return;
                }
                pmStr = "subboxtime " + int.Parse(this.pmView.pmIpt6.text);
                break;
            case 7:
                if (string.IsNullOrEmpty(this.pmView.pmIpt7.text))
                {
                    return;
                }
                pmStr = "additem " + _lstItemUnits[this.pmView.itemPop.selectedIndex].Id + ","+int.Parse(this.pmView.pmIpt7.text);
                break;
            case 8:
                pmStr = "addpet " + _lstPetUnits[this.pmView.petPop.selectedIndex].Id  + ","+int.Parse(this.pmView.pmIpt8.text);
                break;
            case 9:
                if (string.IsNullOrEmpty(this.pmView.pmIpt.text))
                {
                    return;
                }
                string[] pmStrArr = this.pmView.pmIpt.text.Split(';');
                foreach (var pmIptStr in pmStrArr)
                {
                    if(string.IsNullOrEmpty(pmIptStr)) return;
                    var builderPm = Pm_CS.CreateBuilder();
                    builderPm.StrParam = pmIptStr;
                    Pm_CS pm = builderPm.Build();
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_Pm_CS, pm);
                }
                return;
                break;
            case 10:
                if (string.IsNullOrEmpty(this.pmView.pmIptQ.text))
                {
                    return;
                }

                int quality = this.pmView.itemQuality.selectedIndex + 1;
                int level = int.Parse(this.pmView.pmIptQ.text);
                pmStr = "addequip " + _equipUnits[this.pmView.itemEquip.selectedIndex].Id + "," +
                        _equipUnits[this.pmView.itemEquip.selectedIndex].Parts + "," + quality + "," + level;
                break;
            case 11:
                pmStr = "addskill " + _skillUnits[this.pmView.skillpop.selectedIndex].Id + ","+int.Parse(this.pmView.pmIpt11.text);
                break;
            case 12:
                pmStr = "dailyreset";
                break;
            case 13:
                var systemUnits = ConfigDataGroup.GetInstance<ConfigSystem>().Data.ToList();
                foreach (var item in systemUnits)
                {
                    if(item.Value.Condition != "0")
                        pmStr += "function " + item.Value.Id + '\n';
                }
                break;
            case 14:
                pmStr = "kickself -s";
                break;
            case 15:
                pmStr = "addhero" + _lstHeroUnits[this.pmView.heroPop.selectedIndex].Id;
                break;
            case 16:
                MapObjectManager.Instance.Speed *= 2f;
                return;
            case 17:
                //关闭新手引导
                GuideManager.Instance.CloseAllGuide();
                break;
            case 18:
                //跑得快
                var view = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
                if(view != null)
                    view.moveSpeed = 0.01f;
                break;
        }
        if(string.IsNullOrEmpty(pmStr)) return;
        var builder = Pm_CS.CreateBuilder();
        builder.StrParam = pmStr;
        Pm_CS pmCs = builder.Build();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_Pm_CS, pmCs);
        
        // SetVisible(false);
    }
}
