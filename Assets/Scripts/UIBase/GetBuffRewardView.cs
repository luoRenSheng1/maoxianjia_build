using System.Collections;
using System.Collections.Generic;
using System.Text;
using Common;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Shop;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

public class GetBuffRewardView : UIViewBase
{
    private UI_GetBuffReward buffRewardUI => this.main as UI_GetBuffReward;
    
    private ClaimBuff claimBuff;
    
    private bool isShowAni = false;
    
    private Coroutine timeCoroutine;
    private int currentTime; //倒计时
    
    public GetBuffRewardView()
    {
        this.name = "GetBuffReward";
        this.package = "Common";
        this.component = "GetBuffReward";
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
        // this.buffRewardUI.sortingOrder = -11;
        this.buffRewardUI.getBtn.onClick.Add(this.Hide);
        this.buffRewardUI.list.itemRenderer = ItemRender;
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        if (values.Length > 0)
        {
            isShowAni = values[0] is bool;
        }
        
        // UpdateUI();
    }
    
    // private void UpdateUI()
    // {
    // }
    
    protected override void OnShow()
    {
        base.OnShow();
        
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralGainRewardSE);

        claimBuff = DataManager.Instance.GetLimitBuff();
        if (claimBuff.buffInfo.battleAttrList.Count <= 0)
        {
            this.Hide();
            return;
        }
        currentTime = (int)(claimBuff.buffInfo.endTime - ServerTimeManager.Instance.CurServerTime);
        this.buffRewardUI.list.numItems = claimBuff.buffInfo.battleAttrList.Count;
        timeCoroutine = GameManager.Instance.StartCoroutine(StartCountDown());
        
        // 界面已经存在，直接置顶,放在最前面
        PowerUpView powerUp = UIManager.Instance.FindByName("PowerUp") as PowerUpView;
        if (powerUp != null && powerUp.IsShow())
        {
            UIManager.Instance.TopController(powerUp, true);
        }
    }
    
    protected override void OnHide()
    {
        base.OnHide();
        StopTimeCoroutine();
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_GAIN_BUFF_ANIMATION, isShowAni);
        isShowAni = false;
    }
    
    private void ItemRender(int index, GObject item)
    {
        BuffData buffData = claimBuff.buffInfo.battleAttrList[index];

        ConfigAttrEnumerationUnit attrEnum = ConfigUtils.GetAttrEnumerationById((int)buffData.battleAttr);
        ((UI_AttrUpItem2)item).icon.url = UIResource.GetAttrIconById(attrEnum.Icon);

        ((UI_AttrUpItem2)item).desc.text = ConfigUtils.GetTextById(attrEnum.AttrName);
        
        // int time = (int)(claimBuff.buffInfo.endTime - claimBuff.buffInfo.startTime)/60;
        string fen = (currentTime / 60).ToString("D2");
        string miao = (currentTime % 60).ToString("D2");
        // ((UI_AttrUpItem2)item).time.text = (fen+":"+miao);
        ((UI_AttrUpItem2)item).time.SetVar("value",(fen+":"+miao).ToString()).FlushVars();

        bool isShowInt = false;
        switch (buffData.battleAttr)
        {
            case eBattleAttr.eBattleAttr_HP:
                isShowInt = true;
                break;
            // case eBattleAttr.eBattleAttr_Atk:
            case eBattleAttr.eBattleAttr_FinalAttack:
                isShowInt = true;
                break;
            case eBattleAttr.eBattleAttr_HP_Recovery:
                isShowInt = true;
                break;
        }

        if (isShowInt)
        {
            ((UI_AttrUpItem2)item).num.text = "+" + buffData.attrValue.ToString(); //SetVar("value",buffData.attrValue.ToString()).FlushVars();
        }
        else
        {
            int rate = (int)((buffData.attrValue / 10000)*100);
            ((UI_AttrUpItem2)item).num.SetVar("value",rate.ToString()).FlushVars();
        }
    }
    
    /// <summary>
    /// 开启倒计时协程
    /// </summary>
    /// <returns></returns>
    IEnumerator StartCountDown()
    {
        while (currentTime >= 0)
        {
            for (int i = 0; i < claimBuff.buffInfo.battleAttrList.Count; i++)
            {
                ItemRender(i, this.buffRewardUI.list.GetChildAt(i));
            }
            yield return GameManager.Instance.waitSec1;
            currentTime = (int)(claimBuff.buffInfo.endTime - ServerTimeManager.Instance.CurServerTime);
            // currentTime--;
        }
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
