using UnityEngine;

namespace Engine
{
    public struct ColorKeyFrame
    {
        public float time;
        public Color color;
    }

    //render in pawn
    public class PawnRenderComponent : RenderComponent
    {
        static ColorKeyFrame[] hitWhite = new ColorKeyFrame[]
        {
            new ColorKeyFrame() { time = 0f,color = new Color(0,0,0) },
            new ColorKeyFrame() { time = 0.2f,color = new Color(0.7f, 0.7f, 0.7f) },
            new ColorKeyFrame() { time = 0.4f,color = new Color(0.85f, 0.85f, 0.85f) },
            new ColorKeyFrame() { time = 0.6f,color = new Color(0.8f, 0.4f, 0.4f) },
            new ColorKeyFrame() { time = 0.8f,color = new Color(0.8f, 0f, 0f) },
            new ColorKeyFrame() { time = 1.0f,color = new Color(0,0,0)}
        };

        protected Material mat;
        protected bool bTintColorInit = false;
        protected Color tintColorInit = Color.black;
        // 受击闪白
        private float fTimeWhiteTime = -1;
        private float fTimeWhiteTimeTotal = 0.0f;
        // 受击闪红
        private static Color s_hitRed = new Color(0.6f, 0f, 0f);
        private static float s_hitRedTime = 0.1f;
        private Color colorRedBegin;
        private Color colorRedEnd;
        private float fTimeRedBegin = 0.0f;
        private float fTimeRedTotal = 0.0f;

        public PawnRenderComponent(Renderer r) : base(r)
        {
        }

        public Material Mat
        {
            get
            {
                if (mat == null && ren != null)
                {
                    mat = ren.material;
                }

                if (!bTintColorInit)
                {
                    if (mat != null && mat.HasProperty("_TintColor"))
                    {
                        tintColorInit = mat.GetColor("_TintColor");
                    }

                    bTintColorInit = true;
                }

                return mat;
            }
        }

        public override void SetRenderType(RenderType type)
        {
            if (this.rType != type)
            {
                rType = type;

                switch (type)
                {
                    case RenderType.NORMAL:
                        {
                            if (this.Mat != null)
                            {
                                this.Mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                                this.Mat.SetFloat("_Opaqueness", 1.0f);
                                this.Mat.SetFloat("_FxSrcFactor", (int)UnityEngine.Rendering.BlendMode.One);
                                this.Mat.SetFloat("_FxDstFactor", (int)UnityEngine.Rendering.BlendMode.Zero);
                            }
                        }
                        break;

                    case RenderType.TRANSPARENT:
                        {
                            if (this.Mat != null)
                            {
                                this.Mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                                this.Mat.SetFloat("_Opaqueness", 0.5f);
                                this.Mat.SetFloat("_FxSrcFactor", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                                this.Mat.SetFloat("_FxDstFactor", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            }
                        }
                        break;

                    case RenderType.HIDE:
                        {
                            if (this.Mat != null)
                            {
                                this.Mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                                this.Mat.SetFloat("_Opaqueness", 0);
                                this.Mat.SetFloat("_FxSrcFactor", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                                this.Mat.SetFloat("_FxDstFactor", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        public override void Dispose()
        {
            SetColor(tintColorInit);
            this.mat = null;
            base.Dispose();
        }

        public override void Tick(float deltaSeconds)
        {
            base.Tick(deltaSeconds);

            this.TickByHitWhite(deltaSeconds);
            this.TickByHitRed(deltaSeconds);
        }

        public void SetColor(Color color)
        {
            if (this.Mat != null && this.Mat.HasProperty("_TintColor"))
            {
                this.Mat.SetColor("_TintColor", color);
            }
        }

        public void TriggerHitWhite(float fTime)
        {
            fTimeWhiteTime = 0;
            fTimeWhiteTimeTotal = fTime;
        }

        public void TickByHitWhite(float deltaSeconds)
        {
            if (this.fTimeWhiteTime >= 0 && this.Mat != null)
            {
                fTimeWhiteTime += deltaSeconds;
                int i = 0;
                int n = hitWhite.Length;

                for (; i < n; ++i)
                {
                    if (fTimeWhiteTime < hitWhite[i].time * fTimeWhiteTimeTotal)
                    {
                        break;
                    }
                }

                if (i == n)
                {
                    fTimeWhiteTime = -1;
                    SetColor(tintColorInit);
                }
                else
                {
                    float t = (fTimeWhiteTime - hitWhite[i].time * fTimeWhiteTimeTotal) /
                        (hitWhite[i].time * fTimeWhiteTimeTotal - hitWhite[i - 1].time * fTimeWhiteTimeTotal);
                    Color c = Color.Lerp(hitWhite[i - 1].color, hitWhite[i].color, t);
                    SetColor(c);
                }
            }
        }

        public void TriggerHitRed(float fTime)
        {
            // 受击中不重复设置时间
            if (this.fTimeRedTotal != 0 || fTime == 0)
            {
                return;
            }

            if (this.Mat == null || !this.Mat.HasProperty("_TintColor"))
            {
                return;
            }

            this.fTimeRedBegin = ThirdParty.RealTime.time;
            this.fTimeRedTotal = fTime;

            Color tintColor = this.Mat.GetColor("_TintColor");
            colorRedBegin = s_hitRed;
            colorRedBegin.a = tintColor.a;
            colorRedEnd = tintColorInit;
            colorRedEnd.a = tintColor.a;
            SetColor(colorRedBegin);
        }

        public void TickByHitRed(float deltaSeconds)
        {
            if (this.fTimeRedTotal != 0 && this.Mat != null)
            {
                float fTimePass = ThirdParty.RealTime.time - this.fTimeRedBegin;

                if (fTimePass >= this.fTimeRedTotal || this.fTimeRedTotal <= 0)
                {
                    this.fTimeRedTotal = 0;
                    SetColor(colorRedEnd);
                    return;
                }

                if (fTimePass >= s_hitRedTime)
                {
                    Color c = Color.Lerp(colorRedBegin, colorRedEnd, (fTimePass - s_hitRedTime) / (this.fTimeRedTotal - s_hitRedTime));
                    SetColor(c);
                }
            }
        }

        public void SetBlackdropFactor(bool enable)
        {
            if (this.Mat != null)
            {
                this.Mat.SetFloat("_IgnoreBlackFactor", enable ? 1.0f : 0.0f);
            }
        }
    }
}
