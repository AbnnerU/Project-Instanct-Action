using UnityEngine;
using System.Threading.Tasks;

namespace Scripts.Saver
{
    public abstract class SaverBase : MonoBehaviour
    {
        //[Header("Base Configuration")]
        //[SerializeField] protected TerrariumLoadSO _terrariumData;

        protected bool _webGL = false;

        protected string _terrariumID => SIS_GameSession.Instance != null ? SIS_GameSession.Instance.ID : "";

        protected virtual void Start()
        {
#if UNITY_WEBGL 
            _webGL = true;
#endif
            LoadInitial();
        }

        public abstract Task SaveAsync();

        public abstract void SaveSync();

        public abstract void LoadInitial();
    }
}