using UnityEngine;

namespace Engine
{
    public enum RenderType
    {
        NORMAL = 0,
        TRANSPARENT,
        HIDE,
    }
    
    //render in pawn
    abstract public class RenderComponent
    {
        protected Renderer ren;
        protected RenderType rType;

        abstract public void SetRenderType(RenderType type);

        public RenderComponent(Renderer r)
        {
            rType = RenderType.NORMAL;
            this.SetRenderer(r);
        }

        public virtual void SetRenderer(Renderer r)
        {
            this.ren = r;
        }

        public void Reset()
        {
            //TODO: reset render state when reconnect
        }

        public void DisableRender()
        {
            if (this.ren != null && this.ren.gameObject != null)
            {
                this.ren.gameObject.SetActive(false);
            }
        }

        public void EnableRender()
        {
            if (this.ren != null && this.ren.gameObject != null)
            {
                this.ren.gameObject.SetActive(true);
            }
        }

        public virtual void Tick(float deltaSeconds)
        {
        }

        public virtual void Dispose()
        {
            this.ren = null;
        }
    }
}
