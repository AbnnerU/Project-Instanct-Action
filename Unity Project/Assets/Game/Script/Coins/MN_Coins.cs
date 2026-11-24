using System;
using UnityEngine;

namespace Scripts.Economy
{
    public class MN_Coins : SceneSingleton<MN_Coins>
    {
        public Action<int> OnCoinsUpdate;
        public Action<int> OnAdd;
        public Action<int> OnRemove;
        public Action<float> OnInflationUpdate;

        [SerializeField] private int _coins;
        [SerializeField] private float _inflation=1;

        public int GetCoins() { return _coins; }

        public void AddCoins(int addValue)
        {
            _coins += addValue;
            OnCoinsUpdate?.Invoke(_coins);
            OnAdd?.Invoke(addValue);
        }

        public void RemoveCoins(int removeValue)
        {
            int finalValue = Mathf.RoundToInt(removeValue * _inflation);

            if (finalValue > _coins)
                return;

            _coins -= finalValue;
            _coins = Mathf.Max(0, _coins); 

            OnCoinsUpdate?.Invoke(_coins);
            OnRemove?.Invoke(removeValue);
        }

        public void SetCoin(int coin)
        {
            _coins = coin;
            OnCoinsUpdate?.Invoke(_coins);
        }

        public bool CanPay(int value)
        {
            if (_inflation < 0)
                _inflation = 0;
           
            return _coins >= (value* _inflation);
        }

        public void UpdateInflation(float inflation)
        {
            if (inflation < 0)
                inflation = 0;

            _inflation = inflation;

            OnInflationUpdate?.Invoke(_inflation);
        }

        public float GetInflation()
        {
            return _inflation;
        }
    }
}