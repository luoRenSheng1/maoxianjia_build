
using Common;
using Config;
using Engine;
using FairyGUI;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

namespace CommonEx
{
    public partial class UI_ComUserInfo : GButton
    {
        public UI_ComUserInfo()
        {
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CHANGENAMECOUNTER, this.ChangePlayerName);
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, this.UpdateUserInfo);
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_AVATARS_UPDATE, this.UpdateUserInfo);
        }
        public void UpdateUserInfo()
        {
            var roleData = DataManager.Instance.GetRoleData();

            // this.txtLv.text = roleData.lv.ToString();
            this.txtName.text = roleData.userName;
            this.headIcon.icon = UIResource.GetItemUrl(roleData.GetAvatarUrl());
            // this.expPro.value = roleData.exp;
            // this.expPro.max = ConfigUtils.GetConfigExpLevelUnitById(roleData.lv).Exp;
            var comCurrency = (UI_ComCurrency) (this.comCurrency);
            comCurrency.btnGold.txtValue.text = StringUtils.FormatCurrency(roleData.gold);
            comCurrency.btnDia.txtValue.text = StringUtils.FormatCurrency(roleData.dia);

            this.headIcon.onClick.Set(this.OnClickHead);
            // 关闭商业化
            // comCurrency.btnDia.onClick.Set(this.OnClickDia);
            // comCurrency.btnGold.onClick.Set(this.OnClickGold);
        }

        private void ChangePlayerName()
        {
            var roleData = DataManager.Instance.GetRoleData();
            this.txtName.text = roleData.userName;
        }

        private void OnClickHead()
        {
            UIManager.Instance.ShowUIPanel("SettingMain");
        }

        private void OnClickDia()
        {
            Utils.OpenBuyDiamondView();
        }

        private void OnClickGold()
        {
            Utils.OpenBuyGoldView();
        }
        
    }
}
