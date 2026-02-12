
using AdventureCave;
using BigMap;
using Common;
using Engine;
using FairyGUI;
using msg;
using BatchStuff = Engine.BatchStuff;
using Debug = UnityEngine.Debug;
using EventDispatcher = EngineBase.EventDispatcher;

/// <summary>
/// 奇遇商人商店
/// </summary>
public class AdventureCaveShopView : UIViewBase
{
    /// <summary>
    /// 商店界面
    /// </summary>
    private UI_AdventureCaveShop view => this.main as UI_AdventureCaveShop;

    /// <summary>
    /// 货币ID
    /// </summary>
    private const int currencyId = 1000;

    /// <summary>
    /// 事件数据
    /// </summary>
    private RandomEventData shopData = null;

    //是否可以购买
    private bool canPurchased = true;
    /// <summary>
    /// 服务端的事件ID
    /// </summary>
    private ulong guid = 0;

    public AdventureCaveShopView()
    {
        this.package = "AdventureCave";
        this.name = "AdventureCaveShop";
        this.component = "AdventureCaveShop";
        this.removePackage = true;
        this.safeAreaInset = false;
    }
    public override void BindAll()
    {
        base.BindAll();
        AdventureCaveBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        this.canPurchased = true;
        if (values.Length > 0)
        {
            this.canPurchased = (bool)values[0];
        }
        if (values.Length > 1)
        {
            this.guid = (ulong)values[1];
        }
    }
    protected override void OnInit()
    {
        base.OnInit();

        // this.view.titIcon.url = "Sokoban/general_reward_bg31";
        //this.view.titIcon.url = "ui://2nawooiyrjj21d";

        //this.view.title.text = "奇遇商店";

        //退出事件
        this.view.close?.onClick.Add(OnExit);
    }

    protected override void OnShow()
    {
        base.OnShow();
        RefListData();

        EventDispatcher.GameWorld.Regist<eErrCode>(EventDefine.EVENT_ADVENTURE_CAVE_SHOP, RefList);
    }
    protected override void OnHide()
    {
        base.OnHide();

        EventDispatcher.GameWorld.UnRegist<eErrCode>(EventDefine.EVENT_ADVENTURE_CAVE_SHOP, RefList);

        //this.Destroy();//强制销毁
    }

    private void RefList(eErrCode e)
    {
        if(!IsShow() || !IsOnStage() || this.view == null || this.view.list == null) { return; }

        if(e != eErrCode.eErrCode_Success)
        {//T出副本
             MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
             {
                 OkCallBack = () =>
                 {
                     //退出
                     OnExit();
                 }
             };
            UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.GetStringByKey(5111), param, false);
            return;
        }

        RefListData();
    }

    /// <summary>
    /// 设置列表商品数据
    /// </summary>
    private void RefListData()
    {
        //刷新钻石数量
        var dim = (UI_NewCurrency)this.view.dim;
        dim.txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().dia);

        //根据服务端下发的数据进行刷新
        //List<RandomEventData> treasureRandomEventDatas = MapChapterManager.Instance.GetRandomEventListByType((int)eRandomEventType.eRandomEventType_AdventureBusinessMan);
        this.shopData = MapChapterManager.Instance.GetRandomEventDataByGuid(this.guid);//目前规则是只有一个
        if(this.shopData == null)
        {
            MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
            {
                OkCallBack = () => {
                    //退出
                    OnExit();
                }
            };
            UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.GetStringByKey(5119), param, false);
            return;
        }
        Debug.Log($"商店数据数量={this.shopData.batchStuffList.Count}");

        this.view.list.itemRenderer = ItemRenderer;
        this.view.list.numItems = this.shopData.batchStuffList.Count;
    }
    /// <summary>
    /// 刷新单个商品节点
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    private void ItemRenderer(int index, GObject item)
    {
        BatchStuff bs = this.shopData.batchStuffList[index];
        var traderData = ConfigUtils.GetEventTraderById(bs.cfgId);
        var sItem = item as UI_HuntingShopItem;
        //通用设置商品数据
        UIExtensions.SetHuntingShopItem(sItem, traderData.ItemId, traderData.Number, currencyId, traderData.Score);
        //是否售罄
        sItem.statuCtrl.selectedIndex = bs.amount <= 0 ? 1 : 0;
        //是否可以点击
        sItem.touchable = bs.amount > 0;

        //数据
        sItem.data = bs;
        //点击事件
        sItem.onClick.Clear();
        //可以购买才能点击
        if(this.canPurchased)
        {
            sItem.onClick.Add(OnBuyItem);
        }
    }
    /// <summary>
    /// 购买事件
    /// </summary>
    /// <param name="context"></param>
    private void OnBuyItem(EventContext context)
    {
        GButton button = context.sender as GButton;
        var d = button.data as BatchStuff;
        //次数足够？
        if(d.amount <= 0)
        {
            return;
        }
        var traderData = ConfigUtils.GetEventTraderById(d.cfgId);
        //钻石足够？
        if (DataManager.Instance.GetRoleData().dia < traderData.Score)
        {
            UIManager.Instance.ToastByKey(10073);
            return;
        }

        //检测事件是否过期
        //int eventType = MapChapterManager.Instance.GetEventTypeByGuid(shopData.guid);
        //if (eventType == -1)
        //{
        //    MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
        //    {
        //        OkCallBack = () =>
        //        {
        //            //退出
        //            OnExit();
        //        }
        //    };
        //    UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.GetStringByKey(5119), param, false);
        //    return;
        //}

        //发送给服务端
        var msg = PurchaseInCave_CS.CreateBuilder();
        msg.EventGuid = shopData.guid;//事件guid
        msg.StuffId = (uint)d.id;
        msg.Amount = 1;

        this.shopData = MapChapterManager.Instance.GetRandomEventDataByGuid(this.guid);//目前规则是只有一个
        if(this.shopData == null)//实在没数据只能让玩家退出
        {
            MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
            {
                OkCallBack = () => {
                    //退出
                    OnExit();
                }
            };
            UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.GetStringByKey(5119), param, false);
            return;
        }
        foreach(var item in this.shopData.batchStuffList)
        {
            if(d.id == item.id)
            {
                item.amount--;
                break;
            }
        }

        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_PurchaseInCave_CS, msg.Build());
        //
        //this.view.list.itemRenderer = ItemRenderer;
    }
    /// <summary>
    /// 退出界面
    /// </summary>
    private void OnExit()
    {
        UIManager.Instance.CloseUIPanel("AdventureCaveShop");
    }
}
