using System.Collections;
using UnityEngine;


namespace Scripts.Combat
{
    public class HitReaction_CancelAndBlockComboController: MonoBehaviour
    {
        [SerializeField] private GameObject _hittableReference;
        [SerializeField] private ComboControllerBase _comboControllerComponent;
        [SerializeField] private string _hittedParameter = "Hitted";
        [SerializeField] private bool _blockAttacks;
        [SerializeField] private float _blockTime;

        private Coroutine _blockAttackRoutine;
        private IHittable _hittableHandler;


        private void Start()
        {
            _hittableHandler = _hittableReference.GetComponent<IHittable>();

            if (_hittableHandler != null)
                _hittableHandler.OnHitted += OnHitted;
        }

        private void OnHitted(int obj)
        {
            _comboControllerComponent.CancelAttack(false);

            if (_blockAttacks)
            {
                if(_blockAttackRoutine!=null)
                StopCoroutine(_blockAttackRoutine);

                _blockAttackRoutine = StartCoroutine(BlockAttacks());
            }
        }


        IEnumerator BlockAttacks()
        {
            float currentTime = 0;

            _comboControllerComponent.Disable();

            do
            {
                currentTime += Time.deltaTime;
                yield return null;

            } while (currentTime < _blockTime);

            _blockAttackRoutine = null;

            _comboControllerComponent.Enable();
        }
    }


}