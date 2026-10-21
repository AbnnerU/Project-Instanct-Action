using UnityEngine;
using System.Collections;

namespace Scripts.Combat
{
    public class HitReaction_DisableCollider : MonoBehaviour
    {
        [SerializeField] private GameObject _hittableReference;
        [SerializeField] private Collider _colliderComponent;
        [SerializeField] private float _disableTime;

        private Coroutine _disableColliderRoutine;
        private IHittable _hittableHandler;


        private void Start()
        {
            _hittableHandler = _hittableReference.GetComponent<IHittable>();

            if (_hittableHandler != null)
                _hittableHandler.OnHitted += OnHitted;
        }

        private void OnHitted(int obj)
        {
            if (_disableColliderRoutine != null)
                StopCoroutine(_disableColliderRoutine);

            _disableColliderRoutine = StartCoroutine(BlockDash());
        }


        IEnumerator BlockDash()
        {
            float currentTime = 0;

            _colliderComponent.enabled = false;

            do
            {
                currentTime += Time.deltaTime;
                yield return null;

            } while (currentTime < _disableTime);

            _disableColliderRoutine = null;

            _colliderComponent.enabled = true;
        }
    }
}