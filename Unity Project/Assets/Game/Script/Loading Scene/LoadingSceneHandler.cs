using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Scripts.LoadScene
{
    public class LoadingSceneHandler : MonoBehaviour
    {
        public Action<float> OnProgressPercentageUpdate;
        public Func<bool> OnCleanSessionRequired;

        // public List<AssetReferenceLoadingTaskSOBase> List_tasksQueue = new List<AssetReferenceLoadingTaskSOBase>();
        private async void Start()
        {
            Debug.Log("<<<<LoadingSceneHandler>>>>>");

            if (SIS_LoadingSceneChannel.Instance == null)
            {
                Debug.LogError($"Cant find any SIS_LoadingSceneChannel!");
                return;
            }

            var sceneKey = SIS_LoadingSceneChannel.Instance.SceneToLoad;
            if (sceneKey == null)
            {
                Debug.LogError("Null scene to load");
                return;
            }

            LoadingProgressInfo loadingProgressInfo = new LoadingProgressInfo();

            #region Clean Session
            if (SIS_LoadingSceneChannel.Instance.TryCleanSession)
            {
                Debug.Log("<<<<Clean Session>>>>>");

                if (OnCleanSessionRequired != null)
                {
                    bool cleanSessionDone = OnCleanSessionRequired.Invoke();
                    if (cleanSessionDone == false)
                    {
                        Debug.LogError("[LoadingSceneHandler] Clean Session Fail");
                        return;
                    }
                }

                Debug.Log("<<<<Clean Session Done>>>>>");
            }
            #endregion


            #region LoadTasks
            Debug.Log("<<<<Tasks Load Started>>>>>");

            List<AsyncOperationHandle<LoadingTaskSOBase>> list_LoadedHandles = new List<AsyncOperationHandle<LoadingTaskSOBase>>();
            List<LoadingTaskSOBase> list_TasksToDo = new List<LoadingTaskSOBase>();

            foreach (var taskRef in SIS_LoadingSceneChannel.Instance.GetTasks())
            {
                var handle = Addressables.LoadAssetAsync<LoadingTaskSOBase>(taskRef);
                list_LoadedHandles.Add(handle);

                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    list_TasksToDo.Add(handle.Result);
                }
                else
                {
                    Debug.LogError($"Load Task fail: {handle.OperationException}");
                    return;
                }

            }

            loadingProgressInfo.LoadTasksProgress = 1f;
            NotifyProgress(loadingProgressInfo);

            Debug.Log("<<<<ALL Tasks Loaded>>>>>");
            #endregion


            #region TasksLoadType

            Debug.Log("<<<<Separating Tasks>>>>>");
            bool webGL = false;

#if UNITY_WEBGL
            webGL = true;
#endif

            List<LoadingTaskSOBase> list_SyncTasks = new List<LoadingTaskSOBase>();
            List<LoadingTaskSOBase> list_AsyncTasks = new List<LoadingTaskSOBase>();

            if (webGL)
            {
                foreach (var taskToDo in list_TasksToDo)
                {
                    if (taskToDo.GetLoadingRestriction() != TaskLoadingRestriction.ASYNC_ONLY)
                        list_SyncTasks.Add(taskToDo);
                    else
                        list_AsyncTasks.Add(taskToDo);
                }
            }
            else
            {
                foreach (var taskToDo in list_TasksToDo)
                {
                    if (taskToDo.GetLoadingRestriction() != TaskLoadingRestriction.SYNC_ONLY)
                        list_AsyncTasks.Add(taskToDo);
                    else
                        list_SyncTasks.Add(taskToDo);
                }
            }

            loadingProgressInfo.SeparatingTasksProgress = 1f;
            NotifyProgress(loadingProgressInfo);
            #endregion


            #region ExecuteTasks
            int executionAmount = list_SyncTasks.Count + list_AsyncTasks.Count;

            if (list_SyncTasks.Count > 0)
            {
                await RunSyncTasksOnePerFrame(list_SyncTasks);

                loadingProgressInfo.ExecuteSyncTasksProgress = 1;
                NotifyProgress(loadingProgressInfo);

                Debug.Log("<<<<Sync Tasks Done>>>>>");
            }

            if (list_AsyncTasks.Count > 0)
            {
                foreach (var taskToDo in list_AsyncTasks)
                    await taskToDo.ExecuteAsync();

                loadingProgressInfo.ExecuteAsyncTasksProgress = 1;
                NotifyProgress(loadingProgressInfo);

                Debug.Log("<<<<Async Tasks Done>>>>>");
            }
            #endregion


            #region LoadingScene
            Debug.Log("<<<<Start loading Scene ...>>>>>");

            var sceneHandle = Addressables.LoadSceneAsync(sceneKey, LoadSceneMode.Single, false);

            await UpdateAddressableProgress(sceneHandle);

            loadingProgressInfo.LoadingSceneTaskProgress = 1;
            NotifyProgress(loadingProgressInfo);

            Debug.Log("<<<<LOADING SCENE TASK DONE>>>>>");
            #endregion


            foreach (var handle in list_LoadedHandles)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }

            SIS_LoadingSceneChannel.Instance.ClearTasks();

            list_TasksToDo.Clear();
            list_SyncTasks.Clear();
            list_AsyncTasks.Clear();

            //Resources.UnloadUnusedAssets();
            //System.GC.Collect();

            if (sceneHandle.Status == AsyncOperationStatus.Succeeded)
            {
                await sceneHandle.Result.ActivateAsync();
            }
            else
                Debug.LogError($"Erro Addressable: {sceneHandle.OperationException}");
        }

        private async Task UpdateAddressableProgress(AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance> handle)
        {
            while (!handle.IsDone)
            {
                await Task.Yield();
            }
        }

        private async Task RunSyncTasksOnePerFrame(List<LoadingTaskSOBase> tasks)
        {
            foreach (var task in tasks)
            {
                task.ExecuteSync();
                await Task.Yield();
            }
        }

        private void NotifyProgress(LoadingProgressInfo progressInfo)
        {
            OnProgressPercentageUpdate?.Invoke(progressInfo.GetProgressPercentage());
        }
    }

    public class LoadingProgressInfo
    {
        private float _totalStages = 5;

        public float LoadTasksProgress;
        public float SeparatingTasksProgress;
        public float ExecuteSyncTasksProgress;
        public float ExecuteAsyncTasksProgress;
        public float LoadingSceneTaskProgress;

        public LoadingProgressInfo()
        {
            LoadTasksProgress = 0;
            SeparatingTasksProgress = 0;
            ExecuteSyncTasksProgress = 0;
            ExecuteAsyncTasksProgress = 0;
            LoadingSceneTaskProgress = 0;
        }

        public float GetProgressPercentage()
        {
            float progressTotal = LoadTasksProgress +
                       SeparatingTasksProgress +
                       ExecuteSyncTasksProgress +
                       ExecuteAsyncTasksProgress +
                       LoadingSceneTaskProgress;

            return Mathf.Clamp01(progressTotal / _totalStages);
        }
    }
}
