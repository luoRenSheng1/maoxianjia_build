using System;
using AutoUnpack;
using Engine;
using EngineBase;
using FairyGUI;
using UnityEngine;

public class AutoUnpackView : UIViewBase
{
    private UI_Main autoUpackUI => this.main as UI_Main;

    private UI_openCountList _popCountList;
    private int[] _openKeyNumList = new[] {1, 2, 3, 4, 5};
    private UI_openEntryList _popEntry1List;
    private UI_openEntryList _popEntry2List;
    private UI_openEntryList _popEntry3List;
    private UI_openEntryList _popEntry4List;

    private UI_openEquipQualityList _popEquipList;

    private AutoOpenEquipStruct _openEquipStruct;
    public AutoUnpackView()
    {
        this.name = "AutoUnpack";
        this.package = "AutoUnpack";
        this.component = "Main";
        this.removePackage = true;
        this.safeAreaInset = false;
    }

    protected override void OnInit()
    {
        base.OnInit();

        // this.autoUpackUI.panel.closeBtn.onClick.Add(this.Hide);
        this.autoUpackUI.panel.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.autoUpackUI.panel.btnStart.onClick.Set(OnClickStart);
        _popCountList = UI_openCountList.CreateInstance();
        _popCountList.list.itemRenderer = ItemPopupRenderer;
        _popCountList.list.onClickItem.Add(this.OnClickOpenNumList);
        this.autoUpackUI.panel.openNumBtn.onClick.Add(this.OnClickOpenNumBtn);

        _popEntry1List = UI_openEntryList.CreateInstance();
        _popEntry1List.list.onClickItem.Add(this.OnClickOpenEntry1List);
        _popEntry2List = UI_openEntryList.CreateInstance();
        _popEntry2List.list.onClickItem.Add(this.OnClickOpenEntry2List);
        _popEntry3List = UI_openEntryList.CreateInstance();
        _popEntry3List.list.onClickItem.Add(this.OnClickOpenEntry3List);
        _popEntry4List = UI_openEntryList.CreateInstance();
        _popEntry4List.list.onClickItem.Add(this.OnClickOpenEntry4List);
        this.autoUpackUI.panel.entry1.onClick.Add(this.OnClickEntry1Btn);
        this.autoUpackUI.panel.entry2.onClick.Add(this.OnClickEntry2Btn);
        this.autoUpackUI.panel.entry3.onClick.Add(this.OnClickEntry3Btn);
        this.autoUpackUI.panel.entry4.onClick.Add(this.OnClickEntry4Btn);
        
        _popEquipList = UI_openEquipQualityList.CreateInstance();
        _popEquipList.list.onClickItem.Add(this.OnClickOpenEquipList);
        this.autoUpackUI.panel.equipBtn.onClick.Add(this.OnClickEquipBtn);
        
        this.autoUpackUI.panel.checkBoxFilter1.onChanged.Add(this.OnCheckBox1);
        this.autoUpackUI.panel.checkBoxFilter2.onChanged.Add(this.OnCheckBox2);
        this.autoUpackUI.panel.checkBoxFullTickets.onChanged.Add(this.OnCheckBox3);
        
        Stage.inst.onTouchEnd.AddCapture(__stageTouchEnd);
    }

    private void __stageTouchEnd(EventContext context)
    {
        DisplayObject mc = Stage.inst.touchTarget as DisplayObject;
        bool handled = false;
        while (mc != Stage.inst && mc != null)
        {
            if (mc.gOwner == this.autoUpackUI.panel.entry1 || mc.gOwner == this.autoUpackUI.panel.entry2
            || mc.gOwner == this.autoUpackUI.panel.entry3 || mc.gOwner == this.autoUpackUI.panel.entry4 || mc.gOwner == this.autoUpackUI.panel.equipBtn
            || mc.gOwner == this.autoUpackUI.panel.openNumBtn)
            {
                handled = true;
                break;
            }

            mc = mc.parent;
        }

        if (!handled)
        {
            this.autoUpackUI.panel.entry1.selected = false;
            this.autoUpackUI.panel.entry2.selected = false;
            this.autoUpackUI.panel.entry3.selected = false;
            this.autoUpackUI.panel.entry3.selected = false;
            this.autoUpackUI.panel.entry4.selected = false;
            this.autoUpackUI.panel.openNumBtn.selected = false;
            this.autoUpackUI.panel.equipBtn.selected = false;
        }
    }

    private void ItemPopupRenderer(int index, GObject item)
    {
        ((UI_MessageBox_item) item).suoCtrl.selectedIndex = index > 3 ? 1 : 0;
    }

    private void OnClickOpenEquipList(EventContext context)
    {
        GButton item = context.data as GButton;
        int index = this._popEquipList.list.GetChildIndex(item);
        this.autoUpackUI.panel.equipBtn.text = item?.text;
        _openEquipStruct.equipQuality = index+1;
        GRoot.inst.HidePopup(context.sender as UI_openEquipQualityList);
    }

    private void OnClickOpenNumList(EventContext context)
    {
        UI_MessageBox_item item = context.data as UI_MessageBox_item;
        int index = this._popCountList.list.GetChildIndex(item);
        if (item.suoCtrl.selectedIndex == 1)
        {
            UIManager.Instance.Toast("神灯等级达到.....开启");
            return;
        }
        this.autoUpackUI.panel.openNumBtn.text = item?.text;
        _openEquipStruct.openKeyNumIndex =index;
        _openEquipStruct.openKeyNum = _openKeyNumList[index];
        GRoot.inst.HidePopup(context.sender as UI_openCountList);
    }

    private void OnClickOpenEntry1List(EventContext context)
    {
        UI_MessageBox_item item = context.data as UI_MessageBox_item; 
        this.autoUpackUI.panel.entry1.text = item?.text;
        int index = this._popEntry1List.list.GetChildIndex(item);
        _openEquipStruct.entryId1 = GetEntryIdByIndex(index);
        GRoot.inst.HidePopup(context.sender as UI_openEntryList);
    }
    
    private void OnClickOpenEntry2List(EventContext context)
    {
        UI_MessageBox_item item = context.data as UI_MessageBox_item;
        this.autoUpackUI.panel.entry2.text = item?.text;
        int index = this._popEntry2List.list.GetChildIndex(item);
        _openEquipStruct.entryId2 = GetEntryIdByIndex(index);
        GRoot.inst.HidePopup(context.sender as UI_openEntryList);
    }
    
    private void OnClickOpenEntry3List(EventContext context)
    {
        UI_MessageBox_item item = context.data as UI_MessageBox_item; 
        this.autoUpackUI.panel.entry3.text = item?.text;
        int index = this._popEntry3List.list.GetChildIndex(item);
        _openEquipStruct.entryId3 = GetEntryIdByIndex(index);
        GRoot.inst.HidePopup(context.sender as UI_openEntryList);
    }
    
    private void OnClickOpenEntry4List(EventContext context)
    {
        UI_MessageBox_item item = context.data as UI_MessageBox_item;
        this.autoUpackUI.panel.entry4.text = item?.text;
        int index = this._popEntry4List.list.GetChildIndex(item);
        _openEquipStruct.entryId4 = GetEntryIdByIndex(index);
        GRoot.inst.HidePopup(context.sender as UI_openEntryList);
    }

    private int GetEntryIdByIndex(int index)
    {
        if (index == 0)
            return 0;
        return (int) EN_BUFF_ADD_TYPE.Recovery + index;
    }
    
    private int GetIndexByEntryId(int entryId)
    {
        if (entryId == 0)
            return 0;
        return Mathf.Max(0, entryId - (int) EN_BUFF_ADD_TYPE.Recovery);
    }

    private void OnClickEntry1Btn(EventContext context)
    {
        if(context.sender is GButton sender && sender.selected)
            GRoot.inst.ShowPopup(_popEntry1List, sender, PopupDirection.Auto,true);
        else
        {
            GRoot.inst.HidePopup(_popEntry1List);
        }
    }
    
    private void OnClickEntry2Btn(EventContext context)
    {
        if(context.sender is GButton sender && sender.selected)
            GRoot.inst.ShowPopup(_popEntry2List, sender, PopupDirection.Auto, true);
        else
        {
            GRoot.inst.HidePopup(_popEntry2List);
        }
    }
    
    private void OnClickEntry3Btn(EventContext context)
    {
        if(context.sender is GButton sender && sender.selected)
            GRoot.inst.ShowPopup(_popEntry3List, sender, PopupDirection.Auto,true);
        else
        {
            GRoot.inst.HidePopup(_popEntry3List);
        }
    }
    
    private void OnClickEntry4Btn(EventContext context)
    {
        if(context.sender is GButton sender && sender.selected)
            GRoot.inst.ShowPopup(_popEntry4List, sender, PopupDirection.Auto,true);
        else
        {
            GRoot.inst.HidePopup(_popEntry4List);
        }
    }

    private void OnClickEquipBtn(EventContext context)
    {
        if(context.sender is GButton sender && sender.selected)
            GRoot.inst.ShowPopup(_popEquipList, sender, PopupDirection.Auto,true);
        else
        {
            GRoot.inst.HidePopup(_popEquipList);
        }
    }

    private void OnClickOpenNumBtn(EventContext context)
    {
        if(context.sender is GButton sender && sender.selected)
            GRoot.inst.ShowPopup(_popCountList, sender, PopupDirection.Auto,true);
        else
        {
            GRoot.inst.HidePopup(_popCountList);
        }
    }

    public override void BindAll()
    {
        base.BindAll();

        AutoUnpackBinder.BindAll();
    }

    protected override void OnShow()
    {
        base.OnShow();

        _openEquipStruct = EquipManager.Instance.OpenEquipStruct;
        this.autoUpackUI.panel.checkBoxFilter1.selected = _openEquipStruct.isEntry1;
        this.autoUpackUI.panel.checkBoxFilter2.selected = _openEquipStruct.isEntry2;
        this.autoUpackUI.panel.checkBoxFullTickets.selected = _openEquipStruct.isTZQFull;
        this.autoUpackUI.panel.entry1.text = (this._popEntry1List.list.GetChildAt(GetIndexByEntryId(_openEquipStruct.entryId1)) as UI_MessageBox_item)?.text;
        this.autoUpackUI.panel.entry2.text = (this._popEntry2List.list.GetChildAt(GetIndexByEntryId(_openEquipStruct.entryId2)) as UI_MessageBox_item)?.text;
        this.autoUpackUI.panel.entry3.text = (this._popEntry3List.list.GetChildAt(GetIndexByEntryId(_openEquipStruct.entryId3)) as UI_MessageBox_item)?.text;
        this.autoUpackUI.panel.entry4.text = (this._popEntry4List.list.GetChildAt(GetIndexByEntryId(_openEquipStruct.entryId4)) as UI_MessageBox_item)?.text;
        this.autoUpackUI.panel.equipBtn.text = (this._popEquipList.list.GetChildAt(Math.Max(0, _openEquipStruct.equipQuality-1)) as GButton)?.text;
        this.autoUpackUI.panel.openNumBtn.text = (this._popCountList.list.GetChildAt(_openEquipStruct.openKeyNumIndex) as UI_MessageBox_item)?.text;
    }

    private void OnClickStart()
    {
        UIManager.Instance.ToastByKey(StringDefine.STRING_OPEN_ERROR_FAILD);
        this.SetVisible(false);
        EquipManager.Instance.AutoUnpack = true;
        TreasureChesManager.Instance.AutoTreasure();
    }

    protected override void OnHide()
    {
        base.OnHide();
        EquipManager.Instance.OpenEquipStruct = _openEquipStruct;
        SaveAutoPackData();
        Stage.inst.onTouchEnd.RemoveCapture(__stageTouchEnd);
    }
    
    public void SaveAutoPackData()
    {
        JsonObject itemJson = new JsonObject();
        itemJson = EquipManager.Instance.OpenEquipStruct.ToJsonObject();
        LocalSave.SetStringWithAccount(DataManager.Instance.GetRoleData().userID.ToString(), itemJson.ToString());
        LocalSave.Save();
    }

    private void OnCheckBox1()
    {
        _openEquipStruct.isEntry1 = this.autoUpackUI.panel.checkBoxFilter1.selected;
    }
    private void OnCheckBox2()
    {
        _openEquipStruct.isEntry2 = this.autoUpackUI.panel.checkBoxFilter2.selected;
    }
    private void OnCheckBox3()
    {
        _openEquipStruct.isTZQFull = this.autoUpackUI.panel.checkBoxFullTickets.selected;
    }
}