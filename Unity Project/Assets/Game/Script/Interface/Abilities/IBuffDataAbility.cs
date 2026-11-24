using Scripts.Combat;
using System.Collections.Generic;

namespace Scripts.Abilities
{
    public interface IBuffDataAbility
    {
        public void SetBuffData(IReadOnlyList<BuffData> list_BuffData);
    }
}