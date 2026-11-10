using System;
using System.Collections.Generic;
using Scripts.Economy;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Enchantment
{
    public class EnchantmentPurchase : MonoBehaviour
    {
        public event Action OnPurchaseSucceeded;
        public event Action OnPurchaseFailed;

        [SerializeField] private InputActionReference _purchaseInput;
        [SerializeField, Min(0)] private int _price = 10;
        [SerializeField] private EnchantmentsDeckConfig _deckConfig = new EnchantmentsDeckConfig(false)
        {
            EnchantmentsTypesAvailable = new List<EnchantmentType> { EnchantmentType.STATUS },
            Rolls = 0
        };

        private bool _isPurchasing;

        private void OnEnable()
        {
            if (_purchaseInput == null || _purchaseInput.action == null) return;
            _purchaseInput.action.performed += Purchase_Performed;
            _purchaseInput.action.Enable();
        }

        private void OnDisable()
        {
            if (_purchaseInput != null && _purchaseInput.action != null)
                _purchaseInput.action.performed -= Purchase_Performed;
        }

        private void Purchase_Performed(InputAction.CallbackContext context) => TryPurchase();

        public bool TryPurchase()
        {
            if (_isPurchasing) 
                return false;

            bool succeeded;

            _isPurchasing = true;

            try { 
                succeeded = TryOpenPurchasedOffer(); 
            }
            finally { 
                _isPurchasing = false; 
            }

            if (succeeded) 
                OnPurchaseSucceeded?.Invoke();
            else 
                OnPurchaseFailed?.Invoke();

            return succeeded;
        }

        private bool TryOpenPurchasedOffer()
        {
            MN_Coins coins = MN_Coins.Instance;
            MN_Enchantments enchantments = MN_Enchantments.Instance;

            int price = Mathf.Max(0, _price);

            if (coins == null || enchantments == null || _deckConfig == null || Time.timeScale <= 0f ||
                enchantments.CurrentState != EnchantmentSelectionState.NONE || !coins.CanPay(price))
                return false;

            int previousCoins = coins.GetCoins();
            coins.RemoveCoins(price);
            int paidCoins = previousCoins - coins.GetCoins();

            if (enchantments.TryOfferEnchantments(_deckConfig)) return true;

            if (paidCoins > 0) 
                coins.AddCoins(paidCoins);

            return false;
        }
    }
}
