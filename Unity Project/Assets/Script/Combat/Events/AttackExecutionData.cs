using System;

namespace Scripts.Combat
{
    public readonly struct AttackExecutionData
    {
        public readonly Guid ExecutionId;
        public readonly int ComboIndex;
        public readonly ComboAttackData AttackData;
        public readonly UnityEngine.GameObject Owner;

        public AttackExecutionData(Guid executionId, int comboIndex, ComboAttackData attackData, UnityEngine.GameObject owner)
        {
            ExecutionId = executionId;
            ComboIndex = comboIndex;
            AttackData = attackData;
            Owner = owner;
        }
    }
}
