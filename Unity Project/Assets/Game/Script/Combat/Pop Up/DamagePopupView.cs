using System.Collections;
using Scripts.ObjectPooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Combat
{
    public class DamagePopupView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _damageText;
        [SerializeField] private bool _boldTextOnCritical = true;
        [SerializeField] private Image _criticalImage;
        [SerializeField] private float _releaseDelay = 0.75f;

        private Coroutine _releaseRoutine;
        private FontStyles _defaultFontStyle;

        private void Awake()
        {
            if (_damageText == null)
                _damageText = GetComponentInChildren<TMP_Text>();

            if (_damageText != null)
                _defaultFontStyle = _damageText.fontStyle;
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
            {
                _damageText.text = Mathf.CeilToInt(damageResult.FinalDamage).ToString();
                _damageText.fontStyle = damageResult.IsCritical && _boldTextOnCritical
                    ? _defaultFontStyle | FontStyles.Bold
                    : _defaultFontStyle;
            }

            if (_criticalImage != null)
                _criticalImage.gameObject.SetActive(damageResult.IsCritical);

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
