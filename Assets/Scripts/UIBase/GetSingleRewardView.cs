using System.Collections;
using System.Collections.Generic;
using Common;
using CommonEx;
using Config;
using Engine;
using FairyGUI;
using FairyGUI.Utils;
using msg;
using UnityEngine;

public class GetSingleRewardView : UIViewBase
{
    private UI_SingleReward rewardUI => this.main as UI_SingleReward;

    private ulong guid;
    private uint stuffId;
    private List<ItemListNode> itemList;
    private int selectIndex = 1;
    
    private Coroutine timeCoroutine;
    private int currentTime; //倒计时
    
    public GetSingleRewardView()
    {
        this.name = "GetSingleReward";
        this.package = "Common";
        this.component = "SingleReward";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.type = UIType.Tip;
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        
        this.rewardUI.getBtn.onClick.Add(this.OnClickGetButton);
        this.rewardUI.rwList.itemRenderer = ItemRender;
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);

        if (values.Length > 0)
        {
            guid = (ulong)values[0];
            stuffId = (uint)values[1];
            if (values[2] != null)
            {
                itemList = values[2] as List<ItemListNode>;
            }
        }
    }
    
    protected override void OnShow()
    {
        base.OnShow();
        
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralGainRewardSE);

        selectIndex = 1;
        if (itemList.Count <= 0)
        {
            this.Hide();
            return;
        }

        currentTime = 15;
        this.rewardUI.rwList.numItems = itemList.Count;
        timeCoroutine = GameManager.Instance.StartCoroutine(StartCountDown());
    }
    
    protected override void OnHide()
    {
        base.OnHide();
        StopTimeCoroutine();
    }

    private void OnClickGetButton()
    {
        var msg = ClaimWildPetAward_CS.CreateBuilder();
        msg.EventGuid = guid;
        msg.BatchStuffId = stuffId;
        msg.ItemIndex = (uint)selectIndex;
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ClaimWildPetAward_CS, msg.Build());
        
        this.Hide();
    }
    
    private void ItemRender(int index, GObject itemOjb)
    {
        ItemListNode data = itemList[index];
        
        ((UI_RewardItem) itemOjb).data = (index + 1);
        ((UI_RewardItem) itemOjb).onClick.Add(OnItemClick);
        
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById((int)data.ItemId);

        ((UI_ItemCom)((UI_RewardItem) itemOjb).item).txtLv.text = data.Num.ToString();
        ((UI_ItemCom) ((UI_RewardItem) itemOjb).item).itemSpineEff.visible = false;
        ((UI_ItemCom) ((UI_RewardItem) itemOjb).item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
        ((UI_ItemCom) ((UI_RewardItem) itemOjb).item).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;

        ((UI_RewardItem) itemOjb).name.text = ConfigUtils.GetTextById(itemTypeUnit.Name);
        
        ((UI_RewardItem)itemOjb).name.color = ToolSet.ColorFromRGB(0x4C2911);
        ((UI_RewardItem)itemOjb).name.stroke = 0;
        ((UI_RewardItem) itemOjb).state.selectedIndex = 0;
        if ((selectIndex - 1) == index)
        {
            ((UI_RewardItem)itemOjb).name.stroke = 2;
            ((UI_RewardItem)itemOjb).name.strokeColor = ToolSet.ColorFromRGB(0x000000);
            ((UI_RewardItem)itemOjb).name.color = ToolSet.ColorFromRGB(0xFFFFFF);
            
            ((UI_RewardItem) itemOjb).state.selectedIndex = 1;
        }
    }

    private void OnItemClick(EventContext context)
    {
        int index = (int)(context.sender as UI_RewardItem).data;
        selectIndex = index;
        
        this.rewardUI.rwList.numItems = itemList.Count;
    }
    
    /// <summary>
    /// 开启倒计时协程
    /// </summary>
    /// <returns></returns>
    IEnumerator StartCountDown()
    {
        while (currentTime >= 0)
        {
            rewardUI.time.SetVar("value", "00:" + currentTime.ToString("00")).FlushVars();
            yield return GameManager.Instance.waitSec1;
            currentTime--;
        }
        
        selectIndex = 1;
        OnClickGetButton();
    }
    
    /// <summary>
    /// 停止倒计时 协程
    /// </summary>
    private void StopTimeCoroutine()
    {
        if (timeCoroutine!=null)
        {
            GameManager.Instance.StopCoroutine(timeCoroutine);
        }
    }
}
