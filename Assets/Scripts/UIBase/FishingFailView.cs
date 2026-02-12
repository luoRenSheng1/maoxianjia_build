using BigMap;
using Engine;
using Spine.Unity;

public class FishingFailView : UIViewBase
    {
        private UI_FishingFail _fishingFail => this.main as UI_FishingFail;

        public FishingFailView()
        {
            this.name = "FishingFail";
            this.package = "BigMap";
            this.component = "FishingFail";
            this.removePackage = true;
            this.safeAreaInset = true;
        }
        
        public override void BindAll()
        {
            base.BindAll();
            BigMapBinder.BindAll();
        }

        protected override void OnInit()
        {
            base.OnInit();
            this._fishingFail.againFishingBtn.onClick.Add(this.AgainFishing);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
        
        protected override void OnUpdateParams(params object[] values)
        {
            base.OnUpdateParams(values);
        }
        
        protected override void OnShow()
        {
            base.OnShow();
            GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralLoseSE);
        }

        private void AgainFishing()
        {
            UIManager.Instance.CloseUIPanel("FishingFail");
            
            //点击再次垂钓，再次抛竿
            var view = UIManager.Instance.FindByName("ChapterMap") as  ChapterMapView;
            if (view != null)
            {
                view?.StartFishingAnimation(false);
            }
            
            //开始挥杆申请
            // MapChapterManager.Instance.SendBeginFishing();
            
            
        }

        protected override void OnHide()
        {
            base.OnHide();
            var view = UIManager.Instance.FindByName("ChapterMap") as  ChapterMapView;
            if (view != null)
            {
                view?.StopFishingCoroutine();
                view?.CheckHeroNearFishingPos(true);
                view.curFishingPosData = null;  // 防止状态残留
            }
        }
    }
