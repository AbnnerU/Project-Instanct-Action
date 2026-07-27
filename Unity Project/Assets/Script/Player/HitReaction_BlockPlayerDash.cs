using UnityEngine;
using System.Collections;

namespace Scripts.Player
{
    public class HitReaction_BlockPlayerDash : MonoBehaviour
    {
        [SerializeField] private GameObject _hittableReference;
        [SerializeField] private PlayerDash _playerDashComponent;
        [SerializeField] private float _blockTime;

        private Coroutine _blockDashRoutine;
        private IHittable _hittableHandler;


        private void Start()
        {
            _hittableHandler = _hittableReference.GetComponent<IHittable>();

            if (_hittableHandler != null)
                _hittableHandler.OnHitted += OnHitted;
        }

        private void OnHitted(int obj)
        {
            if (_blockDashRoutine != null)
                StopCoroutine(_blockDashRoutine);

            _blockDashRoutine = StartCoroutine(BlockDash());
        }


        IEnumerator BlockDash()
        {
            float currentTime = 0;

            _playerDashComponent.Disable();

            do
            {
                currentTime += Time.deltaTime;
                yield return null;

            } while (currentTime < _blockTime);

            _blockDashRoutine = null;

            _playerDashComponent.Enable();
        }
    }

}