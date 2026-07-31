namespace Scripts.Combat
{
    public readonly struct AttackExecutionData
    {
        public readonly int ExecutionId;
        public readonly int ComboIndex;
        public readonly ComboAttackData AttackData;
        public readonly UnityEngine.GameObject Owner;

        public AttackExecutionData(int executionId, int comboIndex, ComboAttackData attackData, UnityEngine.GameObject owner)
        {
            ExecutionId = executionId;
            ComboIndex = comboIndex;
            AttackData = attackData;
            Owner = owner;
        }
    }
}
