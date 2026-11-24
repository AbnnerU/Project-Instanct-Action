using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Scripts.LoadScene
{
    public class SIS_LoadingSceneChannel : Singleton<SIS_LoadingSceneChannel>
    {
        private string _loadingSceneName = "LoadingScene";

        public bool TryCleanSession;
        public AssetReference SceneToLoad { get; private set; }
        public List<AssetReferenceLoadingTaskSOBase> List_tasksQueue = new List<AssetReferenceLoadingTaskSOBase>();

        public void AddTask(AssetReferenceLoadingTaskSOBase task)
        {
            if (!List_tasksQueue.Contains(task)) List_tasksQueue.Add(task);
        }

        public void ClearTasks() => List_tasksQueue.Clear();

        public List<AssetReferenceLoadingTaskSOBase> GetTasks() => new List<AssetReferenceLoadingTaskSOBase>(List_tasksQueue);

        public void Load(AssetReference targetScene)
        {
            TryCleanSession = false;
            SceneToLoad = targetScene;
            SceneManager.LoadScene(_loadingSceneName);
        }

        public void LoadAndCleanSession(AssetReference targetScene)
        {
            TryCleanSession = true;
            SceneToLoad = targetScene;
            SceneManager.LoadScene(_loadingSceneName);
        }
    }
}