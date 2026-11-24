using System;

namespace Scripts.Combat
{
    public readonly struct AttackExecutionData
    {
        public readonly Guid ExecutionId;
        public readonly int ComboIndex;
        public readonly int HitIndex;
        public readonly AttackHitData HitData;
        public readonly ComboAttackData AttackData;
        public readonly UnityEngine.GameObject Owner;

        public AttackExecutionData(Guid executionId, int comboIndex, ComboAttackData attackData, UnityEngine.GameObject owner, int hitIndex = 0)
        {
            ExecutionId = executionId;
            ComboIndex = comboIndex;
            AttackData = attackData;
            Owner = owner;
            HitIndex = hitIndex;
            HitData = attackData?.GetHit(hitIndex);
        }
    }
}
