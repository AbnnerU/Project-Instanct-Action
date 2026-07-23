using UnityEngine;
using System.Threading.Tasks;

namespace Scripts.LoadScene
{
    public abstract class LoadingTaskSOBase : ScriptableObject
    {
        [TextArea] public string DevDescription;
       
        public abstract Task ExecuteAsync();

        public abstract void ExecuteSync();

        public virtual TaskLoadingRestriction GetLoadingRestriction()
        {
            return TaskLoadingRestriction.NONE;
        }
    }

    public enum TaskLoadingRestriction
    {
        NONE,
        ASYNC_ONLY,
        SYNC_ONLY
    }
}