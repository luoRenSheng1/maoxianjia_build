using UnityEngine.SceneManagement;
using EngineBase;

namespace Engine
{
    public class SceneHelper : TSingleton<SceneHelper>
    {
  
        public void OnInit()
        {
            //SceneManager.sceneLoaded += this.OnSceneLoaded;
            SceneManager.sceneUnloaded += this.OnSceneUnloaded;
        }
        
        public void OnDispose()
        {
            //SceneManager.sceneLoaded -= this.OnSceneLoaded;
            SceneManager.sceneUnloaded -= this.OnSceneUnloaded;
        }
        
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
           
        }
        
        private void OnSceneUnloaded(Scene scene)
        {
        }
    }
}