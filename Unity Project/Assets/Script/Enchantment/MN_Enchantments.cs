using System;
using System.Collections.Generic;
using Scripts.Managers;
using Scripts.Progression;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Enchantment
{
    public class MN_Enchantments : SceneSingleton<MN_Enchantments>
    {
        public event Action<EnchantmentSOBase> OnEnchantmentApplied;
        public event Action<IReadOnlyList<EnchantmentSOBase>> OnEnchantmentsOffered;

        [SerializeField] private GameObject _playerObject;
        [SerializeField] private bool _offerEnchantmentsOnLevelUp = true;
        [SerializeField] private int _optionsPerLevelUp = 3;
        [SerializeField] private List<EnchantmentSOBase> _list_AvailableEnchantments = new List<EnchantmentSOBase>();
        [SerializeField] private List<EnchantmentSOBase> _list_AppliedEnchantments = new List<EnchantmentSOBase>();

        private readonly List<EnchantmentSOBase> _list_CurrentOfferedEnchantments = new List<EnchantmentSOBase>();

        public IReadOnlyList<EnchantmentSOBase> List_AvailableEnchantments => _list_AvailableEnchantments;
        public IReadOnlyList<EnchantmentSOBase> List_AppliedEnchantments => _list_AppliedEnchantments;
        public IReadOnlyList<EnchantmentSOBase> List_CurrentOfferedEnchantments => _list_CurrentOfferedEnchantments;

        private void Start()
        {
            if (_offerEnchantmentsOnLevelUp && MN_Xp.Instance != null)
                MN_Xp.Instance.OnLevelUp += Xp_OnLevelUp;
        }

        private void OnDestroy()
        {
            if (MN_Xp.Instance != null)
                MN_Xp.Instance.OnLevelUp -= Xp_OnLevelUp;
        }

        public void ApplyEnchantment(EnchantmentSOBase enchantment)
        {
            if (enchantment == null)
                return;

            if (!enchantment.CanRepeat && HasEnchantment(enchantment))
                return;

            EnchantmentContext context = CreateContext();
            enchantment.Apply(context);

            _list_AppliedEnchantments.Add(enchantment);
            OnEnchantmentApplied?.Invoke(enchantment);
        }

        public void OfferEnchantments()
        {
            BuildOfferedEnchantments();
            OnEnchantmentsOffered?.Invoke(_list_CurrentOfferedEnchantments);
        }

        public EnchantmentContext CreateContext()
        {
            return new EnchantmentContext(MN_PlayerStatus.Instance, _playerObject);
        }

        public bool HasEnchantment(string enchantmentID)
        {
            if (string.IsNullOrEmpty(enchantmentID))
                return false;

            for (int i = 0; i < _list_AppliedEnchantments.Count; i++)
            {
                if (_list_AppliedEnchantments[i] != null && _list_AppliedEnchantments[i].ID == enchantmentID)
                    return true;
            }

            return false;
        }

        public bool HasEnchantment(EnchantmentSOBase enchantment)
        {
            if (enchantment == null)
                return false;

            for (int i = 0; i < _list_AppliedEnchantments.Count; i++)
            {
                EnchantmentSOBase appliedEnchantment = _list_AppliedEnchantments[i];

                if (appliedEnchantment == null)
                    continue;

                if (appliedEnchantment == enchantment)
                    return true;

                if (!string.IsNullOrEmpty(enchantment.ID) && appliedEnchantment.ID == enchantment.ID)
                    return true;
            }

            return false;
        }

        private void Xp_OnLevelUp(int currentLevel)
        {
            OfferEnchantments();
        }

        private void BuildOfferedEnchantments()
        {
            _list_CurrentOfferedEnchantments.Clear();

            if (_list_AvailableEnchantments == null || _list_AvailableEnchantments.Count == 0)
                return;

            List<EnchantmentSOBase> list_OptionsPool = new List<EnchantmentSOBase>();

            for (int i = 0; i < _list_AvailableEnchantments.Count; i++)
            {
                EnchantmentSOBase enchantment = _list_AvailableEnchantments[i];

                if (enchantment == null)
                    continue;

                if (!enchantment.CanRepeat && HasEnchantment(enchantment))
                    continue;

                list_OptionsPool.Add(enchantment);
            }

            if (list_OptionsPool.Count == 0)
                return;

            int optionsCount = Mathf.Min(Mathf.Max(1, _optionsPerLevelUp), list_OptionsPool.Count);

            for (int i = 0; i < optionsCount; i++)
            {
                int randomIndex = Random.Range(0, list_OptionsPool.Count);
                _list_CurrentOfferedEnchantments.Add(list_OptionsPool[randomIndex]);
                list_OptionsPool.RemoveAt(randomIndex);
            }
        }
    }
}
