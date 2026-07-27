using System.Collections;
using Scripts.ObjectPooling;
using TMPro;
using UnityEngine;

namespace Scripts.Combat
{
    public class DamagePopupView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _damageText;
        [SerializeField] private float _releaseDelay = 0.75f;

        private Coroutine _releaseRoutine;

        private void Awake()
        {
            if (_damageText == null)
                _damageText = GetComponentInChildren<TMP_Text>();
        }

        private void OnDisable()
        {
            if (_releaseRoutine != null)
            {
                StopCoroutine(_releaseRoutine);
                _releaseRoutine = null;
            }
        }

        public void Setup(DamageResult damageResult)
        {
            if (_damageText != null)
                _damageText.text = Mathf.CeilToInt(damageResult.FinalDamage).ToString();

            if (_releaseRoutine != null)
                StopCoroutine(_releaseRoutine);

            _releaseRoutine = StartCoroutine(ReleaseAfterDelay());
        }

        private IEnumerator ReleaseAfterDelay()
        {
            float currentTime = 0f;

            while (currentTime < _releaseDelay)
            {
                currentTime += Time.deltaTime;
                yield return null;
            }

            _releaseRoutine = null;
            PoolManager.ReleaseObject(gameObject);
        }
    }
}
