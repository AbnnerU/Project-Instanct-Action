using Scripts.Combat;
using Scripts.Managers;
using Scripts.Player;
using Scripts.Status;
using UnityEngine;

namespace Scripts.Abilities
{
    public class AbilityContext
    {
        public GameObject OwnerObject { get; }
        public Transform OwnerTransform { get; }
        public MN_PlayerStatus PlayerStatus { get; }
        public ComboControllerBase ComboController { get; }
        public AttackHitbox AttackHitbox { get; }
        public PlayerDash PlayerDash { get; }
        public StatusInfoSourceBase StatusSource { get; }

        public AbilityContext(
            GameObject ownerObject,
            MN_PlayerStatus playerStatus,
            ComboControllerBase comboController,
            AttackHitbox attackHitbox,
            PlayerDash playerDash,
            StatusInfoSourceBase statusSource)
        {
            OwnerObject = ownerObject;
            OwnerTransform = ownerObject != null ? ownerObject.transform : null;
            PlayerStatus = playerStatus;
            ComboController = comboController;
            AttackHitbox = attackHitbox;
            PlayerDash = playerDash;
            StatusSource = statusSource;
        }
    }
}
