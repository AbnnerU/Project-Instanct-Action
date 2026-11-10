using UnityEngine;

namespace Scripts.Economy
{
    public class OnDeath_AddCoin : LifeCycleActionsBase
    {
        [SerializeField, Min(0)] private int _coinAmount = 1;
        private bool _hasRewarded;

        private void OnEnable() => _hasRewarded = false;

        protected override void HealthEvents_OnRespawn() => _hasRewarded = false;

        protected override void HealthEvents_OnDeath()
        {
            if (_hasRewarded || MN_Coins.Instance == null) return;
            _hasRewarded = true;
            MN_Coins.Instance.AddCoins(Mathf.Max(0, _coinAmount));
        }
    }
}
