using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Abilities.UI
{
    public class PlayerAbilitySlotsView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private MN_PlayerAbilities _playerAbilities;
        [SerializeField] private List<AbilitySlotView> _list_AbilitySlotViews = new List<AbilitySlotView>();

        #endregion

        #region Unity

        private void Awake()
        {
            if (_playerAbilities == null)
                _playerAbilities = MN_PlayerAbilities.Instance != null ? MN_PlayerAbilities.Instance : FindAnyObjectByType<MN_PlayerAbilities>();
        }

        private void OnEnable()
        {
            SubscribeEvents();
            RefreshSlots();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        #region Events

        private void SubscribeEvents()
        {
            if (_playerAbilities == null)
                return;

            _playerAbilities.OnActiveAbilitySlotUpdated += PlayerAbilities_OnActiveAbilitySlotUpdated;
        }

        private void UnsubscribeEvents()
        {
            if (_playerAbilities == null)
                return;

            _playerAbilities.OnActiveAbilitySlotUpdated -= PlayerAbilities_OnActiveAbilitySlotUpdated;
        }

        private void PlayerAbilities_OnActiveAbilitySlotUpdated(int slotIndex, MN_PlayerAbilities.ActiveAbilitySlot slot)
        {
            if (slotIndex < 0 || slotIndex >= _list_AbilitySlotViews.Count)
                return;

            _list_AbilitySlotViews[slotIndex]?.Setup(slot);
        }

        #endregion

        #region Refresh

        private void RefreshSlots()
        {
            if (_playerAbilities == null)
                return;

            IReadOnlyList<MN_PlayerAbilities.ActiveAbilitySlot> list_ActiveAbilitySlots = _playerAbilities.List_ActiveAbilitySlots;

            for (int i = 0; i < _list_AbilitySlotViews.Count; i++)
            {
                if (_list_AbilitySlotViews[i] == null)
                    continue;

                if (i < list_ActiveAbilitySlots.Count)
                    _list_AbilitySlotViews[i].Setup(list_ActiveAbilitySlots[i]);
                else
                    _list_AbilitySlotViews[i].Clear();
            }
        }

        #endregion
    }
}
