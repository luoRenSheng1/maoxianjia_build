using FairyGUI;
using UnityEngine;

namespace Engine
{
    public class UIGLoader : GLoader
    {
        override protected void LoadExternal()
        {
            // LogUtils.LogLocal("LoadExternal " + this.GID + " " + this.url);

            if (UIGLoaderManager.Instance != null)
            {
                UIGLoaderManager.Instance.AsyncLoadUIRes(this, this.url, bPriority: priority, bFixMode: fixMode);
            }
        }

        override protected void FreeExternal(NTexture texture)
        {
            // LogUtils.LogLocal("FreeExternal " + this.GID + " " + this.url);

            if (UIGLoaderManager.Instance != null)
            {
                UIGLoaderManager.Instance.DelTextureCount(texture);
            }
        }

        public void OnLoadSuccess(Texture texture)
        {
            // LogUtils.LogLocal("OnLoadSuccess " + this.GID + " " + this.url);

            if (this.isDisposed)
            {
                return;
            }

            try
            {
                if (texture != null)
                {
                    if (UIGLoaderManager.Instance != null)
                    {
                        UIGLoaderManager.Instance.AddTextureCount(texture);
                    }
                    NTexture nTexture = new NTexture(texture);
                    nTexture.destroyMethod = DestroyMethod.None;
                    onExternalLoadSuccess(nTexture);
                }
                else
                {
                    onExternalLoadFailed();
                }
            }
            catch (System.Exception ex)
            {
                LogUtils.LogException(ex);
            }
        }

        public void OnLoadFail()
        {
            // LogUtils.LogLocal("LoadTexture Fail: " + this.GID + " " + this.url);

            onExternalLoadFailed();
        }
    }
}