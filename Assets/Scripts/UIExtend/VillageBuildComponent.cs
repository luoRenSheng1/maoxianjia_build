using msg;

namespace Village
{
    using System.Collections.Generic;
    using Config;
    using Engine;
    using FairyGUI;
    using UnityEngine;
    using UnityTimer;
    using Village;

    public partial class UI_VillageBuildItem : GLabel
    {
        private UI_VillageBuildItem _build;
        private CityInfo _buildInfo;
        private ConfigBuildUnit _buildUnit;
        private bool _isShowOperation = false;
        private Timer _timerBuild;
        private int _leftTotalCd;
        private VillageBuildType _buildType;
        private List<Vector2> _operationPosList;
        private bool isInGuiding = false;

        public void InitVillageBuild(VillageBuildType buildType)
        {
            this._buildType = buildType;
            this._build = this;
            _build.buildItem.onClick.Add(this.OnClickBuild);
        }

        public void UpdateBuildInfo()
        {
            _buildInfo = VillageInfoManager.Instance.GetBuildInfoByType(_buildType);
            _buildUnit = ConfigUtils.GetBuildUnitByType(_buildType);
            // this._build.title = _buildUnit?.BuildName;
            this._build.title = ConfigUtils.GetTextById(_buildUnit?.BuildName);
            this.buildItem.icon = UIResource.GetVillageBuildImg(_buildUnit?.BuildIcon);

            if (!_buildInfo.IsUnLock && _buildUnit != null) //未解锁
            {
                this._build.lockCtrl.selectedIndex = 0;
            }
            else
            {
                this._build.lockCtrl.selectedIndex = 1;
            }

            this._build.redCtrl.selectedIndex = VillageInfoManager.Instance.BuildRedDot(_buildType) ? 1 : 0;
        }

        public void OnClickBuild()
        {
            FuncOpenType funcId = VillageInfoManager.Instance.GetFuncIdByBuildType(_buildType);
            var stateMap =  FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType) funcId);
            if(stateMap.Item1)
                UIManager.Instance.ShowUIPanel("BuildInfo", this._buildInfo);
            else
            {
                UIManager.Instance.Toast(stateMap.Item2);
            }
        }
        
    }
}
