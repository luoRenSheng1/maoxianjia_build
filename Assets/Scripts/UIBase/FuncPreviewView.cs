
using System.Collections.Generic;
using CommonEx;
using Config;
using Engine;
using FairyGUI;
using FunctionPreview;
using msg;
using EventDispatcher = EngineBase.EventDispatcher;
/// <summary>
/// 功能解锁奖励列表
/// </summary>
public class FuncPreviewView : UIViewBase
{
    private UI_FuncPreview FuncPreview => this.main as UI_FuncPreview;

    private List<FunPrevInfo> _funPrevInfos;
    public FuncPreviewView()
    {
        this.name = "FuncPreview";
        this.package = "FunctionPreview";
        this.component = "FuncPreview";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        FunctionPreviewBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.FuncPreview.funcList.itemRenderer = FuncItemListRender;
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateFunPreview);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateFunPreview);
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateFunPreview();
    }

    private void FuncItemListRender(int index, GObject item)
    {
        FunPrevInfo info = _funPrevInfos[index];
        ConfigSystemUnit systemUnit = ConfigUtils.GetFunPreInfo(info.FuncId);
        //((UI_FuncOpenItem) item).funcIcon.url = UIResource.GetFuncPreIcon(systemUnit.Icon);
        ((UI_FuncOpenItem) item).funcName.text = systemUnit.Name;
        ((UI_FuncOpenItem) item).funcDesc.text = systemUnit.Desc;
        //string[] rewardArr = systemUnit.Reward.Split(',');
        // ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(rewardArr[0]));
        // ((UI_ItemCom) ((UI_FuncOpenItem) item).rwItem).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
        // ((UI_ItemCom) ((UI_FuncOpenItem) item).rwItem).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
        // ((UI_ItemCom) ((UI_FuncOpenItem) item).rwItem).txtLv.text = rewardArr[1];
        // ((UI_FuncOpenItem) item).canGet.selectedIndex = info.CanGetRw;
        
        ((UI_FuncOpenItem) item).rwItem.data = info;
        ((UI_FuncOpenItem) item).rwItem.onClick.Set(this.OnClickRwItem);
    }

    private void OnClickRwItem(EventContext context)
    {
        FunPrevInfo info = ((UI_ItemCom)context.sender).data as FunPrevInfo;
        // if (info.CanGetRw == 0)
        // {
        //     var builder = FunctionClaimAward_CS.CreateBuilder();
        //     builder.FunctionId = (uint) info.FuncId;
        //     FunctionClaimAward_CS functionClaimAwardCs = builder.Build();
        //     GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_FunctionClaimAward_CS, functionClaimAwardCs);
        // }
        // else if(info.CanGetRw == 1)
        // {
        //     UIManager.Instance.ToastByKey(10149);
        // }

    }
    
    private void UpdateFunPreview()
    {
        _funPrevInfos = FuncPreviewManger.Instance.GetFunPreList();
        _funPrevInfos.Sort(((info, prevInfo) =>
        {
            int result = info.CanGetRw > prevInfo.CanGetRw ? 1 : (info.CanGetRw == prevInfo.CanGetRw) ? 0 : -1;
            //if(result == 0)
                //result = info.SystemUnit.Sort > prevInfo.SystemUnit.Sort ? 1 : (info.SystemUnit.Sort == prevInfo.SystemUnit.Sort) ? 0 : -1;
            if (result == 0)
                result = info.SystemUnit.Id > prevInfo.SystemUnit.Id ? 1 : -1;
            return result;
        }));
        this.FuncPreview.funcList.numItems = _funPrevInfos.Count;
    }
}
