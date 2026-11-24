using Scripts.Combat;
using Scripts.Managers;
using Scripts.DashHandler;
using Scripts.Status;
using System.Collections.Generic;
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
        public DashHandler.DashBase PlayerDash { get; }
        public StatusInfoSourceBase StatusSource { get; }
        public IReadOnlyList<DamageData> Lits_DamageData { get; }
        public IReadOnlyList<BuffData> Lits_BuffData { get; }

        public AbilityContext(
            GameObject ownerObject,
            MN_PlayerStatus playerStatus,
            ComboControllerBase comboController,
            AttackHitbox attackHitbox,
            DashHandler.DashBase playerDash,
            StatusInfoSourceBase statusSource,
            IReadOnlyList<DamageData> list_damageData,
            IReadOnlyList<BuffData> lits_buffData)
        {
            OwnerObject = ownerObject;
            OwnerTransform = ownerObject != null ? ownerObject.transform : null;
            PlayerStatus = playerStatus;
            ComboController = comboController;
            AttackHitbox = attackHitbox;
            PlayerDash = playerDash;
            StatusSource = statusSource;
            Lits_DamageData = list_damageData;
            Lits_BuffData = lits_buffData;
        }
    }
}
