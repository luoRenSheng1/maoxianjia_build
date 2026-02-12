using System.Collections.Generic;
using Config;
using EngineBase;
using msg;

namespace Engine
{

    public class ItemInfoManager : TSingleton<ItemInfoManager>
    {
        public ConfigCommonUnit common300008;
        private bool showTipsing = false;
        public void OnInit()
        {
            common300008 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(300008);
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, this.OnUpdateItem);
        }

        public override void Dispose()
        {
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ITEM_UPDATE, this.OnUpdateItem);
            base.Dispose();
        }

        private void OnUpdateItem()
        {
            //todo 需要道具刷新在调用
        }

        public void Clear()
        {
            _allItemList.Clear();
            _allVillageItemList.Clear();
        }

        #region 道具

        private List<ItemData> _allItemList = new List<ItemData>();
        public void AddItemData(ItemData itemData)
        {
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemData.id);
            if(itemData.id == ConstDefine.PetPieceId && !showTipsing)
            {
                var loading = UIManager.Instance.FindByName("Loading");
                var login = UIManager.Instance.FindByName("Login");
                var pet = UIManager.Instance.FindByName("PetSystem");
                if(loading != null && !loading.IsShow() || login != null && !login.IsShow() || pet != null && !pet.IsShow())
                {
                    if (PetInfoManager.Instance.GetAllHavePetList().Count >= int.Parse(common300008.Param1))
                    {
                        showTipsing = true;
                        UIManager.Instance.ToastByKey(10203);
                        GameManager.Instance.TimerManager.SetTimer(1f, () =>
                        {
                            showTipsing = false;
                        });
                    }
                }
                //UnityEngine.Debug.Log("宠物碎片获得");
            }
            if (itemTypeUnit.Type == (int)eItemType.eItemType_HometownItem)
            {
                AddVillageItem(itemData);
                return;
            }
            ItemData data = GetItemData(itemData.id);
            if (data != null)
            {
                if (data.count == 0 || itemTypeUnit.Type == (int)eItemType.eItemType_Rune)
                    _allItemList.Add(itemData);
                else
                {
                    data.count += itemData.count;
                }


            }
            else
            {
                _allItemList.Add(itemData);
            }

        }

        public ItemData GetItemData(int id)
        {
            for (int i = _allItemList.Count - 1; i >= 0; i--)
            {
                if (_allItemList[i].id == id)
                {
                    return _allItemList[i];
                }
            }

            ItemData itemData = new ItemData() { id = id, count = 0 };
            _allItemList.Add(itemData);
            return itemData;
        }

        public int GetItemCount(int id)
        {
            return (int)GetItemData(id).count;
        }

        public void ReduceItem(int itemId, double num)
        {
            ItemData itemData = ItemInfoManager.Instance.GetItemData(itemId);
            itemData.count -= num;
            if (itemData.count <= 0)
            {
                _allItemList.Remove(itemData);
            }
        }

        #endregion

        #region 家园道具

        private List<ItemData> _allVillageItemList = new List<ItemData>();
        private void AddVillageItem(ItemData itemData)
        {
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemData.id);
            if (itemTypeUnit.Type == (int)eItemType.eItemType_HometownItem)
            {
                ItemData data = GetVillageItem(itemData.id);
                if (data != null)
                {
                    if (data.count == 0)
                        _allVillageItemList.Add(itemData);
                    data.count += itemData.count;
                }
                else
                {
                    _allVillageItemList.Add(itemData);
                }
            }

        }

        public bool HasThisVillageItem(int itemId)
        {
            ItemData data = GetVillageItem(itemId);
            if (data != null && data.count > 0)
            {
                return false;
            }

            return true;
        }

        public ItemData GetVillageItem(int id)
        {
            for (int i = _allVillageItemList.Count - 1; i >= 0; i--)
            {
                if (_allVillageItemList[i].id == id)
                {
                    return _allVillageItemList[i];
                }
            }

            return new ItemData() { id = id, count = 0 };
        }


        #endregion

    }
}